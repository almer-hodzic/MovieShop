import { AfterViewInit, Component, NgZone, OnDestroy, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  GetMyShoppingCartItemDto,
  GetMyShoppingCartQueryDto,
} from '../../../../api-services/shopping-cart/shopping-cart-api.model';
import { ShoppingCartApiService } from '../../../../api-services/shopping-cart/shopping-cart-api.service';
import { PayPalApiService } from '../../../../api-services/paypal/paypal-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';
import { environment } from '../../../../../environments/environment';

type PaymentMethod = 'paypal';

interface PayPalButtons {
  render: (selector: string) => Promise<void>;
}

interface PayPalSdk {
  Buttons: (config: {
    createOrder: () => string;
    onApprove: (data: { orderID: string }) => void;
    onCancel: () => void;
    onError: (err: unknown) => void;
  }) => PayPalButtons;
}

declare global {
  interface Window {
    paypal?: PayPalSdk;
  }
}

@Component({
  selector: 'app-client-checkout',
  standalone: false,
  templateUrl: './client-checkout.component.html',
  styleUrl: './client-checkout.component.scss',
})
export class ClientCheckoutComponent implements OnInit, AfterViewInit, OnDestroy {
  private shoppingCartApi = inject(ShoppingCartApiService);
  private payPalApi = inject(PayPalApiService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private zone = inject(NgZone);
  readonly imageResolver = inject(MovieShopImageService);

  cart: GetMyShoppingCartQueryDto | null = null;
  selectedPaymentMethod: PaymentMethod | null = null;
  isLoading = false;
  isPayPalButtonRendered = false;
  isCreatingPayPalOrder = false;
  isCapturingPayment = false;
  errorMessage = '';

  private viewReady = false;
  private payPalOrderId: string | null = null;
  private readonly payPalScriptId = 'paypal-sdk-script';
  private readonly payPalClientId = environment.payPalClientId;

  ngOnInit(): void {
    this.loadCartItems();
  }

  ngAfterViewInit(): void {
    this.viewReady = true;
  }

  ngOnDestroy(): void {
    this.clearPayPalButton();
  }

  loadCartItems(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.shoppingCartApi.getMine().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading cart items', err);
        this.errorMessage = 'Error loading cart items.';
        this.isLoading = false;
      },
    });
  }

  selectPaymentMethod(method: PaymentMethod): void {
    this.selectedPaymentMethod = method;
    if (method === 'paypal') {
      this.initPayPalButton();
    }
  }

  initPayPalButton(): void {
    if (this.isPayPalButtonRendered || this.isCreatingPayPalOrder || !this.viewReady || this.totalPrice <= 0) {
      return;
    }

    this.isCreatingPayPalOrder = true;
    this.errorMessage = '';

    this.payPalApi.createOrder().subscribe({
      next: (response) => {
        this.payPalOrderId = response.orderId;
        this.loadPayPalSdk()
          .then(() => this.renderPayPalButton())
          .catch((err) => {
            console.error('Error loading PayPal SDK', err);
            this.errorMessage = 'Error loading PayPal payment button.';
            this.toaster.error('Error loading PayPal payment button.');
          })
          .finally(() => {
            this.zone.run(() => {
              this.isCreatingPayPalOrder = false;
            });
          });
      },
      error: (err) => {
        console.error('Error creating PayPal order', err);
        this.isCreatingPayPalOrder = false;
        this.errorMessage = err?.error?.message || 'Error creating PayPal order.';
        this.toaster.error(this.errorMessage);
      },
    });
  }

  get cartItems(): GetMyShoppingCartItemDto[] {
    return this.cart?.items ?? [];
  }

  get totalPrice(): number {
    return this.cart?.totalAmount ?? this.cartItems.reduce((total, item) => total + item.totalPrice, 0);
  }

  getMovieImage(item: GetMyShoppingCartItemDto): string {
    return this.imageResolver.resolveMovieImage(item);
  }

  private renderPayPalButton(): Promise<void> {
    if (!window.paypal || !this.payPalOrderId) {
      return Promise.reject(new Error('PayPal SDK or order id is missing.'));
    }

    this.clearPayPalButton();

    return window.paypal.Buttons({
      createOrder: () => this.payPalOrderId ?? '',
      onApprove: (data) => {
        this.zone.run(() => this.capturePayPalOrder(data.orderID));
      },
      onCancel: () => {
        this.zone.run(() => {
          this.isCapturingPayment = false;
          this.toaster.info('PayPal payment was cancelled.');
        });
      },
      onError: (err) => {
        console.error('Error with PayPal Buttons', err);
        this.zone.run(() => this.toaster.error('Error with PayPal Buttons.'));
      },
    }).render('#paypal-button-container').then(() => {
      this.zone.run(() => {
        this.isPayPalButtonRendered = true;
      });
    });
  }

  private capturePayPalOrder(orderId: string): void {
    if (this.isCapturingPayment) {
      return;
    }

    if (this.payPalOrderId && orderId !== this.payPalOrderId) {
      this.toaster.error('Error during order processing.');
      return;
    }

    this.isCapturingPayment = true;

    this.payPalApi.captureOrder({ orderId }).subscribe({
      next: (captureResponse) => {
        if (captureResponse.status === 'SUCCESS') {
          this.toaster.success('Payment successfully completed!');
          this.clearCartAfterPayment();
          return;
        }

        this.isCapturingPayment = false;
        this.toaster.error(captureResponse.message || 'Order capture failed or not completed.');
      },
      error: (err) => {
        console.error('Error capturing PayPal order', err);
        this.isCapturingPayment = false;
        this.toaster.error(err?.error?.message || 'Error capturing PayPal order.');
      },
    });
  }

  private clearCartAfterPayment(): void {
    this.shoppingCartApi.clear().subscribe({
      next: () => {
        this.isCapturingPayment = false;
        this.router.navigate(['/shopping-cart']);
      },
      error: (err) => {
        console.error('Payment succeeded, but cart clear failed', err);
        this.isCapturingPayment = false;
        this.toaster.warning('Payment succeeded, but the cart was not cleared.');
        this.router.navigate(['/shopping-cart']);
      },
    });
  }

  private loadPayPalSdk(): Promise<void> {
    if (!this.payPalClientId) {
      return Promise.reject(new Error('PayPal ClientId is not configured.'));
    }

    if (window.paypal) {
      return Promise.resolve();
    }

    const existingScript = document.getElementById(this.payPalScriptId) as HTMLScriptElement | null;
    if (existingScript) {
      return new Promise((resolve, reject) => {
        existingScript.addEventListener('load', () => resolve(), { once: true });
        existingScript.addEventListener('error', () => reject(new Error('PayPal SDK failed to load.')), { once: true });
      });
    }

    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.id = this.payPalScriptId;
      script.src = `https://www.paypal.com/sdk/js?client-id=${this.payPalClientId}&currency=USD`;
      script.async = true;
      script.onload = () => resolve();
      script.onerror = () => reject(new Error('PayPal SDK failed to load.'));
      document.body.appendChild(script);
    });
  }

  private clearPayPalButton(): void {
    const paypalButtonContainer = document.getElementById('paypal-button-container');
    if (paypalButtonContainer) {
      paypalButtonContainer.innerHTML = '';
    }
  }
}
