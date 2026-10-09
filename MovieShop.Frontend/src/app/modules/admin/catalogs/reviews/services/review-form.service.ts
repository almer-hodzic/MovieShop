import { inject, Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Injectable()
export class ReviewFormService {
  private fb = inject(FormBuilder);

  createForm(initialMovieId?: number | null): FormGroup {
    return this.fb.group({
      movieId: [initialMovieId ?? null, [Validators.required, Validators.min(1)]],
      score: [null, [Validators.required, Validators.min(0.1), Validators.max(10)]],
      comment: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(81)]],
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      if (controlName === 'movieId') return 'Movie is required.';
      if (controlName === 'score') return 'Score is required.';
      if (controlName === 'comment') return 'Comment is required.';
    }

    if (control.errors['min']) {
      if (controlName === 'movieId') return 'Movie is required.';
      if (controlName === 'score') return 'Score must be greater than 0.';
    }

    if (control.errors['max'] && controlName === 'score') {
      return 'Score must be less than or equal to 10.';
    }

    if (control.errors['minlength'] && controlName === 'comment') {
      return 'Comment must be at least 4 characters.';
    }

    if (control.errors['maxlength'] && controlName === 'comment') {
      return 'Comment can be at most 81 characters.';
    }

    return 'Invalid value.';
  }
}
