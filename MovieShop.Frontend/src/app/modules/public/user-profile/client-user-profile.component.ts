import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ProfileApiService } from '../../../api-services/profile/profile-api.service';
import { GetMyProfileQueryDto } from '../../../api-services/profile/profile-api.model';
import { ToasterService } from '../../../core/services/toaster.service';
import { MovieShopImageService } from '../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-client-user-profile',
  standalone: false,
  templateUrl: './client-user-profile.component.html',
  styleUrl: './client-user-profile.component.scss',
})
export class ClientUserProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private profileApi = inject(ProfileApiService);
  private toaster = inject(ToasterService);
  private imageResolver = inject(MovieShopImageService);

  profile: GetMyProfileQueryDto | null = null;
  imageUrl: string | ArrayBuffer | null = null;
  readerResult: string | null = null;
  selectedFile: File | null = null;
  showAlert = false;
  isLoading = false;
  isSavingEmail = false;
  isSavingPassword = false;
  isSavingPicture = false;
  errorMessage = '';

  passwordForm = this.fb.group({
    username: [{ value: '', disabled: true }],
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
  });

  emailForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.profileApi.getMine().subscribe({
      next: (profile) => {
        this.profile = profile;
        this.fillForm(profile);
        this.fillPicture(profile.profileImage);
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Load profile error:', err);
        this.errorMessage = 'Failed to load profile information.';
        this.isLoading = false;
      },
    });
  }

  onPasswordChange(): void {
    this.passwordForm.markAllAsTouched();
    if (this.passwordForm.invalid || this.isSavingPassword) {
      return;
    }

    this.isSavingPassword = true;
    this.profileApi.changePassword({
      currentPassword: this.passwordForm.getRawValue().currentPassword ?? '',
      newPassword: this.passwordForm.getRawValue().newPassword ?? '',
    }).subscribe({
      next: () => {
        this.toaster.success('Password has been changed successfully!');
        this.passwordForm.patchValue({ currentPassword: '', newPassword: '' });
        this.passwordForm.markAsPristine();
        this.passwordForm.markAsUntouched();
        this.isSavingPassword = false;
      },
      error: (err) => {
        console.error('Change password error:', err);
        this.toaster.error(this.getErrorMessage(err, 'Failed to change password.'));
        this.isSavingPassword = false;
      },
    });
  }

  onEmailChange(): void {
    this.emailForm.markAllAsTouched();
    if (this.emailForm.invalid || !this.emailForm.dirty || this.isSavingEmail) {
      return;
    }

    this.isSavingEmail = true;
    this.profileApi.changeEmail({
      email: this.emailForm.value.email ?? '',
    }).subscribe({
      next: () => {
        this.toaster.success('Email has been changed successfully!');
        this.emailForm.markAsPristine();
        this.loadProfile();
        this.isSavingEmail = false;
      },
      error: (err) => {
        console.error('Change email error:', err);
        this.toaster.error(this.getErrorMessage(err, 'Failed to change email.'));
        this.isSavingEmail = false;
      },
    });
  }

  onPictureChange(): void {
    if (!this.readerResult || this.isSavingPicture) {
      return;
    }

    this.isSavingPicture = true;
    this.profileApi.updateProfileImage({
      profileImageBase64: this.readerResult,
    }).subscribe({
      next: () => {
        this.toaster.success('Picture has been changed successfully!');
        this.readerResult = null;
        this.selectedFile = null;
        this.loadProfile();
        this.isSavingPicture = false;
      },
      error: (err) => {
        console.error('Update profile image error:', err);
        this.toaster.error(this.getErrorMessage(err, 'Failed to change picture.'));
        this.isSavingPicture = false;
      },
    });
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const selectedFile = input.files?.[0];

    if (selectedFile && selectedFile.type.startsWith('image/')) {
      this.selectedFile = selectedFile;
      this.readAndPreviewImage(selectedFile);
      this.showAlert = false;
      return;
    }

    this.showAlert = true;
    input.value = '';
  }

  closeAlert(): void {
    this.showAlert = false;
  }

  private readAndPreviewImage(file: File): void {
    const reader = new FileReader();
    reader.onload = () => {
      this.imageUrl = reader.result;
      this.readerResult = String(reader.result ?? '');
    };
    reader.readAsDataURL(file);
  }

  private fillPicture(base64Image: string | null | undefined): void {
    this.imageUrl = this.imageResolver.resolveUserImage(base64Image);
  }

  private fillForm(profile: GetMyProfileQueryDto): void {
    this.passwordForm.patchValue({ username: profile.username || profile.email });
    this.emailForm.patchValue({ email: profile.email });
    this.emailForm.markAsPristine();
  }

  private getErrorMessage(err: unknown, fallback: string): string {
    const maybeError = err as { error?: { message?: string } | string; message?: string };
    if (typeof maybeError.error === 'string' && maybeError.error.trim()) {
      return maybeError.error;
    }
    if (typeof maybeError.error === 'object' && maybeError.error?.message) {
      return maybeError.error.message;
    }
    return maybeError.message || fallback;
  }
}
