import { inject, Injectable } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GetCategoryByIdQueryDto } from '../../../../../api-services/categories/categories-api.model';

@Injectable()
export class CategoryFormService {
  private fb = inject(FormBuilder);

  createForm(model?: Partial<GetCategoryByIdQueryDto>): FormGroup {
    return this.fb.group({
      categoryName: [
        model?.categoryName ?? '',
        [Validators.required, Validators.maxLength(100)],
      ],
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return 'Category name is required.';
    }
    if (control.errors['maxlength']) {
      return 'Category name can be at most 100 characters.';
    }

    return 'Invalid value.';
  }
}
