import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-forgot-password',
  standalone: false,
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authApi = inject(AuthApiService);
  private readonly toaster = inject(ToasterService);

  isLoading = false;
  errorMessage = '';

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]]
  });

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.authApi.forgotPassword({ email: this.form.controls.email.value ?? '' }).subscribe({
      next: response => {
        this.isLoading = false;
        const message = response.emailDeliveryFallbackUsed
          ? 'Recovery instructions were written to the backend development logs.'
          : `Recovery instructions sent at ${this.form.controls.email.value}`;
        this.toaster.success(message);
        this.form.disable();
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage = getAuthError(error, 'Recovery request failed. Please try again.');
        this.toaster.error(this.errorMessage);
      }
    });
  }
}

function getAuthError(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    return error.error?.detail ?? error.error?.title ?? error.error?.message ?? fallback;
  }
  return fallback;
}
