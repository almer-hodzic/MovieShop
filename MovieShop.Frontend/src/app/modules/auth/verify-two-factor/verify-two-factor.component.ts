import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { TwoFactorChallengeService } from '../../../core/services/auth/two-factor-challenge.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-verify-two-factor',
  standalone: false,
  templateUrl: './verify-two-factor.component.html',
  styleUrl: './verify-two-factor.component.scss'
})
export class VerifyTwoFactorComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly auth = inject(AuthFacadeService);
  private readonly currentUser = inject(CurrentUserService);
  private readonly challengeService = inject(TwoFactorChallengeService);
  private readonly router = inject(Router);
  private readonly toaster = inject(ToasterService);

  isLoading = false;
  errorMessage = '';

  readonly form = this.fb.group({
    code: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
  });

  ngOnInit(): void {
    if (!this.challengeService.snapshot) {
      this.toaster.error('Invalid session. Please try logging in again.');
      this.router.navigate(['/auth/login']);
    }
  }

  onSubmit(): void {
    const challenge = this.challengeService.snapshot;
    if (!challenge || this.form.invalid || this.isLoading) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.auth.verifyTwoFactor({
      userId: challenge.userId,
      code: this.form.controls.code.value ?? '',
      password: challenge.password,
      fingerprint: challenge.fingerprint
    }).subscribe({
      next: () => {
        this.isLoading = false;
        this.challengeService.clear();
        this.toaster.success('Two-factor authentication successful!');
        this.router.navigate([this.currentUser.getDefaultRoute()]);
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage = getAuthError(error, 'Invalid or expired verification code.');
        this.toaster.error(this.errorMessage);
      }
    });
  }

  cancel(): void {
    this.challengeService.clear();
    this.router.navigate(['/auth/login']);
  }
}

function getAuthError(error: unknown, fallback: string): string {
  if (error instanceof HttpErrorResponse) {
    return error.error?.detail ?? error.error?.title ?? error.error?.message ?? fallback;
  }
  return fallback;
}
