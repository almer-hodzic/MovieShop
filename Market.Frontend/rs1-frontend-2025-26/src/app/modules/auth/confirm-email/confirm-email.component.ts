import { HttpErrorResponse } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-confirm-email',
  standalone: false,
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.scss'
})
export class ConfirmEmailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly authApi = inject(AuthApiService);
  private readonly toaster = inject(ToasterService);

  isLoading = true;
  isConfirmed = false;
  errorMessage = '';

  ngOnInit(): void {
    const email = this.route.snapshot.queryParamMap.get('email') ?? '';
    const token = this.route.snapshot.queryParamMap.get('token') ?? '';

    if (!email || !token) {
      this.isLoading = false;
      this.errorMessage = 'The email confirmation link is incomplete.';
      return;
    }

    this.authApi.confirmEmail({ email, token }).subscribe({
      next: response => {
        this.isLoading = false;
        this.isConfirmed = response.isEmailConfirmed;
      },
      error: error => {
        this.isLoading = false;
        this.errorMessage = getAuthError(error, 'There was an error confirming your email. Please try again.');
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
