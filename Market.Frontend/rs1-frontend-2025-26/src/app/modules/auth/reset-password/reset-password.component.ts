import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-reset-password',
  standalone: false,
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly authApi = inject(AuthApiService);
  private readonly toaster = inject(ToasterService);

  isLoading = false;
  errorMessage = '';
  hidePassword = true;
  hideConfirmation = true;

  readonly form = this.fb.group(
    {
      email: ['', [Validators.required, Validators.email]],
      token: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8), Validators.maxLength(100)]],
      confirmPassword: ['', Validators.required]
    },
    { validators: passwordsMatchValidator }
  );

  ngOnInit(): void {
    this.form.patchValue({
      email: this.route.snapshot.queryParamMap.get('email') ?? '',
      token: this.route.snapshot.queryParamMap.get('token') ?? ''
    });
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    const value = this.form.getRawValue();

    this.authApi.resetPassword({
      email: value.email ?? '',
      token: value.token ?? '',
      newPassword: value.newPassword ?? '',
      confirmPassword: value.confirmPassword ?? ''
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.toaster.success('Password has been reset successfully!');
        this.router.navigate(['/auth/login'], { queryParams: { passwordReset: 'success' } });
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage = getAuthError(error, 'Password reset failed. Please request a new recovery token.');
        this.toaster.error(this.errorMessage);
      }
    });
  }
}

const passwordsMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('newPassword')?.value;
  const confirmation = control.get('confirmPassword')?.value;
  return password && confirmation && password !== confirmation ? { passwordsMismatch: true } : null;
};

function getAuthError(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    return error.error?.detail ?? error.error?.title ?? error.error?.message ?? fallback;
  }
  return fallback;
}
