import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ListMoviesQueryDto, ListMoviesRequest } from '../../../../api-services/movies/movies-api.model';
import { MoviesApiService } from '../../../../api-services/movies/movies-api.service';
import {
  AddItemToShoppingCartCommand,
  GetMyShoppingCartItemDto,
  GetMyShoppingCartQueryDto,
  UpdateShoppingCartItemQuantityCommand,
} from '../../../../api-services/shopping-cart/shopping-cart-api.model';
import { ShoppingCartApiService } from '../../../../api-services/shopping-cart/shopping-cart-api.service';
import { BaseComponent } from '../../../../core/components/base-classes/base-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';
import { ShoppingCartFormService } from './services/shopping-cart-form.service';

@Component({
  selector: 'app-shopping-cart',
  standalone: false,
  templateUrl: './shopping-cart.component.html',
  styleUrl: './shopping-cart.component.scss',
  providers: [ShoppingCartFormService],
})
export class ShoppingCartComponent extends BaseComponent implements OnInit {
  private api = inject(ShoppingCartApiService);
  private moviesApi = inject(MoviesApiService);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);
  private formService = inject(ShoppingCartFormService);

  cart: GetMyShoppingCartQueryDto | null = null;
  movies: ListMoviesQueryDto[] = [];
  displayedColumns: string[] = ['movieTitle', 'unitPrice', 'quantity', 'totalPrice', 'addedAt', 'actions'];
  isMoviesLoading = false;

  addItemForm: FormGroup = this.formService.createAddItemForm();
  quantityDraftByItemId: Record<number, number> = {};

  ngOnInit(): void {
    this.loadMovies();
    this.loadCart();
  }

  onAddItem(): void {
    this.addItemForm.markAllAsTouched();
    if (this.addItemForm.invalid) {
      return;
    }

    const payload: AddItemToShoppingCartCommand = {
      movieId: Number(this.addItemForm.value.movieId),
      quantity: Number(this.addItemForm.value.quantity),
    };

    this.startLoading();
    this.api.addItem(payload).subscribe({
      next: () => {
        this.toaster.success('Movie added to shopping cart.');
        this.addItemForm.patchValue({ quantity: 1 });
        this.loadCart();
      },
      error: (err) => {
        console.error('Add cart item error:', err);
        this.stopLoading();
        if (err?.status === 409) {
          this.toaster.warning('Movie is already in your shopping cart.');
          return;
        }
        if (err?.status === 401 || err?.status === 403) {
          this.toaster.error('You must be logged in to add movies to the shopping cart.');
          return;
        }
        this.toaster.error('Failed to add movie to shopping cart.');
      },
    });
  }

  onUpdateQuantity(item: GetMyShoppingCartItemDto): void {
    const quantity = Number(this.quantityDraftByItemId[item.itemId] ?? item.quantity);
    if (!Number.isInteger(quantity) || quantity < 1 || quantity > 100) {
      this.toaster.error('Quantity must be between 1 and 100.');
      return;
    }

    const payload: UpdateShoppingCartItemQuantityCommand = { quantity };

    this.startLoading();
    this.api.updateItemQuantity(item.itemId, payload).subscribe({
      next: () => {
        this.toaster.success('Item quantity updated.');
        this.loadCart();
      },
      error: (err) => {
        this.stopLoading('Failed to update item quantity.');
        console.error('Update cart item quantity error:', err);
      },
    });
  }

  onRemoveItem(item: GetMyShoppingCartItemDto): void {
    this.confirm.confirm({
      title: 'Remove Cart Item',
      message: `Remove "${item.movieTitle}" from cart?`,
      confirmText: 'Remove',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.removeItem(item.itemId).subscribe({
        next: () => {
          this.toaster.success('Item removed from cart.');
          this.loadCart();
        },
        error: (err) => {
          this.stopLoading('Failed to remove cart item.');
          console.error('Remove cart item error:', err);
        },
      });
    });
  }

  onClearCart(): void {
    this.confirm.confirm({
      title: 'Clear Cart',
      message: 'Clear all items from your cart?',
      confirmText: 'Clear',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.clear().subscribe({
        next: () => {
          this.toaster.success('Cart cleared.');
          this.loadCart();
        },
        error: (err) => {
          this.stopLoading('Failed to clear cart.');
          console.error('Clear cart error:', err);
        },
      });
    });
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.addItemForm, controlName);
  }

  private loadCart(): void {
    this.startLoading();
    this.api.getMine().subscribe({
      next: (result) => {
        this.cart = result;
        this.syncQuantityDraft(result.items);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load shopping cart.');
        console.error('Load shopping cart error:', err);
      },
    });
  }

  private loadMovies(): void {
    this.isMoviesLoading = true;
    const request = new ListMoviesRequest();
    request.paging.pageSize = 1000;

    this.moviesApi.list(request).subscribe({
      next: (result) => {
        this.movies = result.items;
        this.isMoviesLoading = false;

        if (!this.addItemForm.value.movieId && this.movies.length > 0) {
          this.addItemForm.patchValue({ movieId: this.movies[0].id });
        }
      },
      error: (err) => {
        this.isMoviesLoading = false;
        console.error('Load movies for cart actions error:', err);
      },
    });
  }

  private syncQuantityDraft(items: GetMyShoppingCartItemDto[]): void {
    this.quantityDraftByItemId = {};
    for (const item of items) {
      this.quantityDraftByItemId[item.itemId] = item.quantity;
    }
  }
}
