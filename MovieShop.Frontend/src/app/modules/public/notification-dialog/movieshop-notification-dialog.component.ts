import { Component, Inject, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { GetMyNotificationsQueryDto } from '../../../api-services/notifications/notifications-api.model';
import { NotificationsApiService } from '../../../api-services/notifications/notifications-api.service';
import { ToasterService } from '../../../core/services/toaster.service';

@Component({
  selector: 'app-movieshop-notification-dialog',
  standalone: false,
  templateUrl: './movieshop-notification-dialog.component.html',
  styleUrl: './movieshop-notification-dialog.component.scss',
})
export class MovieShopNotificationDialogComponent {
  private notificationsApi = inject(NotificationsApiService);
  private dialogRef = inject(MatDialogRef<MovieShopNotificationDialogComponent>);
  private router = inject(Router);
  private toaster = inject(ToasterService);

  constructor(@Inject(MAT_DIALOG_DATA) public data: GetMyNotificationsQueryDto[]) {}

  get unreadNotifications(): GetMyNotificationsQueryDto[] {
    return this.data.filter(notification => !notification.isRead);
  }

  redirectToMovie(notification: GetMyNotificationsQueryDto): void {
    if (!notification.isRead) {
      this.notificationsApi.markAsRead(notification.notificationId).subscribe({
        next: () => {
          notification.isRead = true;
          this.dialogRef.close();
          this.router.navigate(['/movies', notification.movieId]);
        },
        error: () => this.toaster.error('Failed to mark notification as read.'),
      });
      return;
    }

    this.dialogRef.close();
    this.router.navigate(['/movies', notification.movieId]);
  }

  markAsRead(notification: GetMyNotificationsQueryDto): void {
    if (notification.isRead) return;

    this.notificationsApi.markAsRead(notification.notificationId).subscribe({
      next: () => {
        notification.isRead = true;
        this.toaster.success('Notification marked as read.');
      },
      error: () => this.toaster.error('Failed to mark notification as read.'),
    });
  }

  markAllAsRead(): void {
    if (!this.unreadNotifications.length) return;

    forkJoin(this.unreadNotifications.map(item => this.notificationsApi.markAsRead(item.notificationId))).subscribe({
      next: () => {
        this.data = this.data.map(item => ({ ...item, isRead: true }));
        this.toaster.success('Notifications marked as read.');
      },
      error: () => this.toaster.error('Failed to mark notifications as read.'),
    });
  }

  deleteNotification(notification: GetMyNotificationsQueryDto): void {
    this.notificationsApi.delete(notification.notificationId).subscribe({
      next: () => {
        this.data = this.data.filter(item => item.notificationId !== notification.notificationId);
        this.toaster.success('Notification deleted.');
      },
      error: () => this.toaster.error('Failed to delete notification.'),
    });
  }
}
