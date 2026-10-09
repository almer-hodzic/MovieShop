import { Component, inject, OnInit } from '@angular/core';
import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import {
  GetMyNotificationsQueryDto,
} from '../../../../api-services/notifications/notifications-api.model';
import { NotificationsApiService } from '../../../../api-services/notifications/notifications-api.service';

@Component({
  selector: 'app-notifications',
  standalone: false,
  templateUrl: './notifications.component.html',
  styleUrl: './notifications.component.scss',
})
export class NotificationsComponent extends BaseListComponent<GetMyNotificationsQueryDto> implements OnInit {
  private api = inject(NotificationsApiService);
  private toaster = inject(ToasterService);

  displayedColumns: string[] = ['notificationId', 'notificationText', 'movieId', 'notificationDate', 'isRead', 'actions'];

  ngOnInit(): void {
    this.initList();
  }

  protected loadData(): void {
    this.startLoading();
    this.api.getMine().subscribe({
      next: (result) => {
        this.items = result;
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load notifications.');
        console.error('Load notifications error:', err);
      },
    });
  }

  onMarkAsRead(item: GetMyNotificationsQueryDto): void {
    if (item.isRead) {
      return;
    }

    this.api.markAsRead(item.notificationId).subscribe({
      next: () => {
        item.isRead = true;
        this.toaster.success('Notification marked as read.');
        this.loadData();
      },
      error: (err) => {
        console.error('Mark notification as read error:', err);
        this.toaster.error('Failed to mark notification as read.');
      },
    });
  }

  onDelete(item: GetMyNotificationsQueryDto): void {
    this.api.delete(item.notificationId).subscribe({
      next: () => {
        this.toaster.success('Notification deleted.');
        this.loadData();
      },
      error: (err) => {
        console.error('Delete notification error:', err);
        this.toaster.error('Failed to delete notification.');
      },
    });
  }
}
