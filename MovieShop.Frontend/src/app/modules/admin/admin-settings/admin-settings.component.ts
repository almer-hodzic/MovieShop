import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { AdminApiService } from '../../../api-services/admin/admin-api.service';
import { AdminSettingsDto } from '../../../api-services/admin/admin-api.model';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-admin-settings',
  standalone: false,
  templateUrl: './admin-settings.component.html',
  styleUrl: './admin-settings.component.scss',
})
export class AdminSettingsComponent implements OnInit {
  private fb = inject(FormBuilder);
  private adminApi = inject(AdminApiService);
  private toaster = inject(ToasterService);

  settings: AdminSettingsDto | null = null;
  isLoading = false;
  isSaving = false;
  errorMessage = '';

  readonly form = this.fb.group({
    firstname: ['', [Validators.required, Validators.maxLength(100)]],
    lastname: ['', [Validators.required, Validators.maxLength(100)]],
  });

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adminApi.getSettings().subscribe({
      next: settings => {
        this.settings = settings;
        this.form.patchValue({
          firstname: settings.firstname,
          lastname: settings.lastname,
        });
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = err.error?.message || 'Failed to load admin settings.';
        this.isLoading = false;
      },
    });
  }

  save(): void {
    if (this.form.invalid || this.isSaving) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.isSaving = true;

    this.adminApi.updateSettings({
      firstname: value.firstname?.trim() ?? '',
      lastname: value.lastname?.trim() ?? '',
    }).subscribe({
      next: settings => {
        this.settings = settings;
        this.form.patchValue({
          firstname: settings.firstname,
          lastname: settings.lastname,
        });
        this.isSaving = false;
        this.toaster.success('Settings saved successfully.');
      },
      error: (err: HttpErrorResponse) => {
        this.isSaving = false;
        this.toaster.error(err.error?.message || 'Failed to save settings.');
      },
    });
  }
}
