import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpErrorResponse } from '@angular/common/http';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { LoginCommand } from '../../../api-services/auth/auth-api.model';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { TwoFactorChallengeService } from '../../../core/services/auth/two-factor-challenge.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthFacadeService);
  private router = inject(Router);
  private currentUser = inject(CurrentUserService);
  private twoFactorChallenge = inject(TwoFactorChallengeService);
  private toaster = inject(ToasterService);
  hidePassword = true;

  form = this.fb.group({
    email: ['', [Validators.required]],
    password: ['', [Validators.required]],
  });

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;

    this.startLoading();
    this.twoFactorChallenge.clear();

    const payload: LoginCommand = {
      email: this.form.value.email ?? '',
      password: this.form.value.password ?? '',
      fingerprint: null,
    };

    this.auth.login(payload).subscribe({
      next: response => {
        this.stopLoading();

        if (response.requiresTwoFactor) {
          const message = response.emailDeliveryFallbackUsed
            ? 'Two-factor authentication required. Code was written to the backend development logs.'
            : 'Two-factor authentication required.';
          this.toaster.info(message);
          this.twoFactorChallenge.start({
            userId: response.userId,
            password: payload.password,
            fingerprint: payload.fingerprint ?? null
          });
          this.router.navigate(['/auth/verify-two-factor']);
          return;
        }

        const target = this.currentUser.getDefaultRoute();
        this.toaster.success('Logged in successfully!');
        this.router.navigate([target]);
      },
      error: (err) => {
        const message = getAuthError(err, 'Invalid credentials. Please try again.');
        this.stopLoading(message);
        this.toaster.error(message);
        console.error('Login error:', err);
      },
    });
  }
}

function getAuthError(error: unknown, fallback: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
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
