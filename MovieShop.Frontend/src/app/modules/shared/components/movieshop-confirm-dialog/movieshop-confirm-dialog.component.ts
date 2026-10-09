import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

export interface MovieShopConfirmDialogData {
  title: string;
  message: string;
  confirmText?: string;
  cancelText?: string;
  tone?: 'danger' | 'primary';
}

@Component({
  selector: 'app-movieshop-confirm-dialog',
  standalone: false,
  templateUrl: './movieshop-confirm-dialog.component.html',
  styleUrl: './movieshop-confirm-dialog.component.scss',
})
export class MovieShopConfirmDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: MovieShopConfirmDialogData) {}

  get confirmText(): string {
    return this.data.confirmText ?? 'Confirm';
  }

  get cancelText(): string {
    return this.data.cancelText ?? 'Cancel';
  }

  get tone(): 'danger' | 'primary' {
    return this.data.tone ?? 'danger';
  }
}
