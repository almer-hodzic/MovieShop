import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  GetMySavedForLaterItemDto,
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
  savingItemIds = new Set<number>();
  movingSavedItemIds = new Set<number>();
  removingSavedItemIds = new Set<number>();
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

  saveForLater(item: GetMyShoppingCartItemDto): void {
    if (this.savingItemIds.has(item.itemId)) {
      return;
    }

    this.savingItemIds.add(item.itemId);

    this.shoppingCartApi.moveItemToSavedForLater(item.itemId).subscribe({
      next: () => {
        this.savingItemIds.delete(item.itemId);
        this.toaster.success('Item saved for later.');
        this.loadCartItems();
      },
      error: (err) => {
        console.error('Error saving item for later', err);
        this.savingItemIds.delete(item.itemId);
        this.toaster.error('Error saving item for later.');
      },
    });
  }

  moveSavedToCart(item: GetMySavedForLaterItemDto): void {
    if (this.movingSavedItemIds.has(item.itemId)) {
      return;
    }

    this.movingSavedItemIds.add(item.itemId);

    this.shoppingCartApi.moveSavedItemToCart(item.itemId).subscribe({
      next: () => {
        this.movingSavedItemIds.delete(item.itemId);
        this.toaster.success('Item moved to cart.');
        this.loadCartItems();
      },
      error: (err) => {
        console.error('Error moving saved item to cart', err);
        this.movingSavedItemIds.delete(item.itemId);
        this.toaster.error('Error moving saved item to cart.');
      },
    });
  }

  removeSavedItem(item: GetMySavedForLaterItemDto): void {
    if (this.removingSavedItemIds.has(item.itemId)) {
      return;
    }

    this.confirm.confirm({
      title: 'Remove Saved Item',
      message: `Remove "${item.movieTitle}" from saved items?`,
      confirmText: 'Remove',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.removingSavedItemIds.add(item.itemId);

      this.shoppingCartApi.removeSavedItem(item.itemId).subscribe({
        next: () => {
          this.removingSavedItemIds.delete(item.itemId);
          this.toaster.success('Saved item removed.');
          this.loadCartItems();
        },
        error: (err) => {
          console.error('Error removing saved item', err);
          this.removingSavedItemIds.delete(item.itemId);
          this.toaster.error('Error removing saved item.');
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

  isSaving(itemId: number): boolean {
    return this.savingItemIds.has(itemId);
  }

  isMovingSaved(itemId: number): boolean {
    return this.movingSavedItemIds.has(itemId);
  }

  isRemovingSaved(itemId: number): boolean {
    return this.removingSavedItemIds.has(itemId);
  }

  get cartItems(): GetMyShoppingCartItemDto[] {
    return this.cart?.items ?? [];
  }

  get savedItems(): GetMySavedForLaterItemDto[] {
    return this.cart?.savedForLaterItems ?? [];
  }

  getTotal(): number {
    return this.cart?.totalAmount ?? this.cartItems.reduce((total, cartItem) => total + cartItem.totalPrice, 0);
  }

  getMovieImage(item: GetMyShoppingCartItemDto): string {
    return this.imageResolver.resolveMovieImage(item);
  }

  getSavedMovieImage(item: GetMySavedForLaterItemDto): string {
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
