import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  GetMyShoppingCartItemDto,
  GetMyShoppingCartQueryDto,
} from '../../../api-services/shopping-cart/shopping-cart-api.model';
import { ShoppingCartApiService } from '../../../api-services/shopping-cart/shopping-cart-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../shared/services/movieshop-confirm.service';
import { MovieShopImageService } from '../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-client-shopping-cart',
  standalone: false,
  templateUrl: './client-shopping-cart.component.html',
  styleUrl: './client-shopping-cart.component.scss',
})
export class ClientShoppingCartComponent implements OnInit {
  private shoppingCartApi = inject(ShoppingCartApiService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private confirm = inject(MovieShopConfirmService);
  readonly imageResolver = inject(MovieShopImageService);

  cart: GetMyShoppingCartQueryDto | null = null;
  isLoading = false;
  errorMessage = '';
  quantityDraftByItemId: Record<number, number> = {};
  updatingItemIds = new Set<number>();
  removingItemIds = new Set<number>();
  isClearing = false;

  ngOnInit(): void {
    this.loadCartItems();
  }

  loadCartItems(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.shoppingCartApi.getMine().subscribe({
      next: (cart) => {
        this.cart = cart;
        this.syncQuantityDraft(cart.items);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading cart items', err);
        this.errorMessage = 'Error loading cart items.';
        this.isLoading = false;
      },
    });
  }

  updateQuantity(item: GetMyShoppingCartItemDto): void {
    const quantity = Number(this.quantityDraftByItemId[item.itemId] ?? item.quantity);
    if (!Number.isInteger(quantity) || quantity < 1 || quantity > 100) {
      this.toaster.error('Quantity must be between 1 and 100.');
      this.quantityDraftByItemId[item.itemId] = item.quantity;
      return;
    }

    if (quantity === item.quantity || this.updatingItemIds.has(item.itemId)) {
      return;
    }

    this.updatingItemIds.add(item.itemId);

    this.shoppingCartApi.updateItemQuantity(item.itemId, { quantity }).subscribe({
      next: () => {
        this.updatingItemIds.delete(item.itemId);
        this.toaster.success('Cart quantity updated.');
        this.loadCartItems();
      },
      error: (err) => {
        console.error('Error updating item quantity', err);
        this.updatingItemIds.delete(item.itemId);
        this.quantityDraftByItemId[item.itemId] = item.quantity;
        this.toaster.error('Error updating item quantity.');
      },
    });
  }

  removeItem(item: GetMyShoppingCartItemDto): void {
    if (this.removingItemIds.has(item.itemId)) {
      return;
    }

    this.confirm.confirm({
      title: 'Remove Cart Item',
      message: `Remove "${item.movieTitle}" from cart?`,
      confirmText: 'Remove',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.removingItemIds.add(item.itemId);

      this.shoppingCartApi.removeItem(item.itemId).subscribe({
        next: () => {
          this.removingItemIds.delete(item.itemId);
          if (this.cart) {
            this.cart = {
              ...this.cart,
              items: this.cart.items.filter((cartItem) => cartItem.itemId !== item.itemId),
              totalQuantity: this.cart.totalQuantity - item.quantity,
              totalAmount: this.cart.totalAmount - item.totalPrice,
            };
          }
          this.toaster.success('Item removed from cart.');
          this.loadCartItems();
        },
        error: (err) => {
          console.error('Error removing item', err);
          this.removingItemIds.delete(item.itemId);
          this.toaster.error('Error removing item.');
        }
      });
    });
  }

  clearCart(): void {
    if (!this.cart?.items.length || this.isClearing) {
      return;
    }

    this.confirm.confirm({
      title: 'Clear Cart',
      message: 'Clear all items from your cart?',
      confirmText: 'Clear',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.isClearing = true;

      this.shoppingCartApi.clear().subscribe({
        next: () => {
          this.isClearing = false;
          this.toaster.success('Cart cleared.');
          this.loadCartItems();
        },
        error: (err) => {
          console.error('Error clearing cart', err);
          this.isClearing = false;
          this.toaster.error('Error clearing cart.');
        },
      });
    });
  }

  isUpdating(itemId: number): boolean {
    return this.updatingItemIds.has(itemId);
  }

  isRemoving(itemId: number): boolean {
    return this.removingItemIds.has(itemId);
  }

  get cartItems(): GetMyShoppingCartItemDto[] {
    return this.cart?.items ?? [];
  }

  getTotal(): number {
    return this.cart?.totalAmount ?? this.cartItems.reduce((total, cartItem) => total + cartItem.totalPrice, 0);
  }

  getMovieImage(item: GetMyShoppingCartItemDto): string {
    return this.imageResolver.resolveMovieImage(item);
  }

  navigateToCheckout(): void {
    this.router.navigate(['/shopping-cart/checkout']);
  }

  private syncQuantityDraft(items: GetMyShoppingCartItemDto[]): void {
    this.quantityDraftByItemId = {};
    for (const item of items) {
      this.quantityDraftByItemId[item.itemId] = item.quantity;
    }
  }
}
