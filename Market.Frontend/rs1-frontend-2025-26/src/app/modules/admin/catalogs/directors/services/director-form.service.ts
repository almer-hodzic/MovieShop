import { inject, Injectable } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { GetDirectorByIdQueryDto } from '../../../../../api-services/directors/directors-api.model';

@Injectable()
export class DirectorFormService {
  private fb = inject(FormBuilder);

  createForm(model?: Partial<GetDirectorByIdQueryDto>): FormGroup {
    return this.fb.group({
      firstName: [
        model?.firstName ?? '',
        [Validators.required, Validators.maxLength(100)],
      ],
      lastName: [
        model?.lastName ?? '',
        [Validators.required, Validators.maxLength(100)],
      ],
      birthDate: [
        model?.birthDate ? new Date(model.birthDate) : null,
        [Validators.required, this.pastDateValidator],
      ],
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      if (controlName === 'firstName') {
        return 'First name is required.';
      }
      if (controlName === 'lastName') {
        return 'Last name is required.';
      }
      if (controlName === 'birthDate') {
        return 'Birth date is required.';
      }
    }

    if (control.errors['maxlength']) {
      if (controlName === 'firstName') {
        return 'First name can be at most 100 characters.';
      }
      if (controlName === 'lastName') {
        return 'Last name can be at most 100 characters.';
      }
    }

    if (control.errors['futureDate']) {
      return 'Birth date must be in the past.';
    }

    return 'Invalid value.';
  }

  private pastDateValidator(control: AbstractControl): ValidationErrors | null {
    const rawValue = control.value;
    if (!rawValue) {
      return null;
    }

    const value = new Date(rawValue);
    if (isNaN(value.getTime())) {
      return { futureDate: true };
    }

    const today = new Date();
    const todayStart = new Date(today.getFullYear(), today.getMonth(), today.getDate());
    const valueStart = new Date(value.getFullYear(), value.getMonth(), value.getDate());

    return valueStart < todayStart ? null : { futureDate: true };
  }
}

