import { MediaMatcher } from '@angular/cdk/layout';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { GetMyNotificationsQueryDto } from '../../../api-services/notifications/notifications-api.model';
import { NotificationsApiService } from '../../../api-services/notifications/notifications-api.service';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { MovieShopNotificationDialogComponent } from '../notification-dialog/movieshop-notification-dialog.component';

declare const google: any;

@Component({
  selector: 'app-movieshop-menu',
  standalone: false,
  templateUrl: './movieshop-menu.component.html',
  styleUrl: './movieshop-menu.component.scss',
})
export class MovieshopMenuComponent implements OnInit, OnDestroy {
  private auth = inject(AuthFacadeService);
  private currentUser = inject(CurrentUserService);
  private notificationsApi = inject(NotificationsApiService);
  private dialog = inject(MatDialog);
  private router = inject(Router);
  private changeDetector = inject(ChangeDetectorRef);
  private media = inject(MediaMatcher);

  readonly pageTitle = 'MovieShop';
  readonly mobileQuery = this.media.matchMedia('(max-width: 880px)');
  notifications: GetMyNotificationsQueryDto[] = [];
  private readonly mobileQueryListener = () => this.changeDetector.detectChanges();

  ngOnInit(): void {
    this.mobileQuery.addEventListener('change', this.mobileQueryListener);
    this.loadNotifications();
    this.loadGoogleTranslate();
  }

  ngOnDestroy(): void {
    this.mobileQuery.removeEventListener('change', this.mobileQueryListener);
  }

  isAuthenticated(): boolean {
    return this.currentUser.isAuthenticated();
  }

  isAdmin(): boolean {
    return this.currentUser.isAdmin();
  }

  get userEmail(): string {
    return this.currentUser.snapshot?.email ?? '';
  }

  get unreadNotificationCount(): number {
    return this.notifications.filter(notification => !notification.isRead).length;
  }

  openNotifications(): void {
    this.dialog.open(MovieShopNotificationDialogComponent, {
      data: [...this.notifications],
    }).afterClosed().subscribe(() => this.loadNotifications());
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => this.router.navigate(['/welcome']),
      error: () => this.router.navigate(['/welcome']),
    });
  }

  private loadNotifications(): void {
    if (!this.isAuthenticated()) {
      this.notifications = [];
      return;
    }

    this.notificationsApi.getMine().subscribe({
      next: notifications => this.notifications = notifications,
      error: () => this.notifications = [],
    });
  }

  private loadGoogleTranslate(): void {
    if (!document.getElementById('google-translate-script')) {
      const script = document.createElement('script');
      script.id = 'google-translate-script';
      script.src = 'https://translate.google.com/translate_a/element.js?cb=googleTranslateElementInit';
      script.async = true;
      document.body.appendChild(script);
    }

    // Keep the old generated translator widget initialization and placement.
    (window as any).googleTranslateElementInit = function() {
      if (google && google.translate) {
        new google.translate.TranslateElement(
          { pageLanguage: 'hr' },
          'google_translate_element',
        );
      }

      setTimeout(() => {
        const translateDropdown = document.querySelector('.goog-te-combo') as HTMLSelectElement;
        if (translateDropdown) {
          translateDropdown.addEventListener('click', event => {
            event.preventDefault();
            event.stopPropagation();
          });
        }
      }, 1000);
    };
  }
}
