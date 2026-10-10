import { Component, inject } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { ToasterService } from '../../../core/services/toaster.service';
import { usernameAvailabilityValidator } from '../validators/username-availability.validator';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);

  isLoading = false;
  errorMessage = '';
  hidePassword = true;
  hideConfirmation = true;

  readonly form = this.fb.group(
    {
      firstname: ['', [Validators.required, Validators.maxLength(100)]],
      lastname: ['', [Validators.required, Validators.maxLength(100)]],
      username: [
        '',
        [Validators.required, Validators.minLength(3), Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._-]+$/)],
        [usernameAvailabilityValidator(this.authApi)]
      ],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
      password: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
      confirmPassword: ['', Validators.required],
      acceptTerms: [false, Validators.requiredTrue]
    },
    { validators: passwordsMatchValidator }
  );

  onSubmit(): void {
    if (this.form.invalid || this.form.pending || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authApi.register({
      firstname: this.form.controls.firstname.value ?? '',
      lastname: this.form.controls.lastname.value ?? '',
      username: this.form.controls.username.value ?? '',
      email: this.form.controls.email.value ?? '',
      password: this.form.controls.password.value ?? ''
    }).subscribe({
      next: response => {
        this.isLoading = false;
        const message = response.emailDeliveryFallbackUsed
          ? 'Your account was created successfully. Confirmation link was written to the backend development logs.'
          : 'Your account was created successfully. Check your email to confirm it.';
        this.toaster.success(message);
        this.router.navigate(['/auth/login']);
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage = getAuthError(error, 'Registration failed. Please try again.');
        this.toaster.error(this.errorMessage);
      }
    });
  }
}

const passwordsMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password')?.value;
  const confirmation = control.get('confirmPassword')?.value;
  return password && confirmation && password !== confirmation ? { passwordsMismatch: true } : null;
};

function getAuthError(error: unknown, fallback: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
  }

  if (error.status === 0) {
    return 'Unable to connect to the MovieShop API. Make sure the backend is running and its HTTPS certificate is trusted.';
  }

  const payload = parseErrorPayload(error.error);
  return payload?.message
    ?? payload?.detail
    ?? payload?.title
    ?? fallback;
}

function parseErrorPayload(value: unknown): { message?: string; detail?: string; title?: string } | null {
  if (value && typeof value === 'object') {
    return value as { message?: string; detail?: string; title?: string };
  }

  if (typeof value === 'string') {
    try {
      const parsed: unknown = JSON.parse(value);
      return parsed && typeof parsed === 'object'
        ? parsed as { message?: string; detail?: string; title?: string }
        : null;
    } catch {
      return value.trim() ? { message: value } : null;
    }
  }

  return null;
}
