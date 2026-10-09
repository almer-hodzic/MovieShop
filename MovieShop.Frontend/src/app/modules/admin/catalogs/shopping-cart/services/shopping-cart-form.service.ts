import { inject, Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Injectable()
export class ShoppingCartFormService {
  private fb = inject(FormBuilder);

  createAddItemForm(initialMovieId?: number | null): FormGroup {
    return this.fb.group({
      movieId: [initialMovieId ?? null, [Validators.required, Validators.min(1)]],
      quantity: [1, [Validators.required, Validators.min(1), Validators.max(100)]],
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      if (controlName === 'movieId') return 'Movie is required.';
      if (controlName === 'quantity') return 'Quantity is required.';
    }

    if (control.errors['min']) {
      if (controlName === 'movieId') return 'Movie is required.';
      if (controlName === 'quantity') return 'Quantity must be at least 1.';
    }

    if (control.errors['max'] && controlName === 'quantity') {
      return 'Quantity can be at most 100.';
    }

    return 'Invalid value.';
  }
}
