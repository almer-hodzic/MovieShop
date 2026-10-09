import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit, inject } from '@angular/core';
import { AdminApiService } from '../../../api-services/admin/admin-api.service';
import { AdminDashboardDto } from '../../../api-services/admin/admin-api.model';

@Component({
  selector: 'app-admin-launcher',
  standalone: false,
  templateUrl: './admin-launcher.component.html',
  styleUrl: './admin-launcher.component.scss',
})
export class AdminLauncherComponent implements OnInit {
  private adminApi = inject(AdminApiService);

  dashboard: AdminDashboardDto | null = null;
  isLoading = false;
  errorMessage = '';

  readonly navigationItems = [
    { label: 'Manage Movies', icon: 'movie', route: '/admin/movies' },
    { label: 'Manage Categories', icon: 'category', route: '/admin/categories' },
    { label: 'New Reviews', icon: 'rate_review', route: '/admin/reviews' },
    { label: 'Add Actor', icon: 'person', route: '/admin/actors' },
    { label: 'Manage Directors', icon: 'people', route: '/admin/directors' },
    { label: 'Settings', icon: 'settings', route: '/admin/settings' },
  ];

  get statCards(): Array<{ label: string; value: number; icon: string; route?: string }> {
    if (!this.dashboard) return [];

    return [
      { label: 'Movies', value: this.dashboard.moviesCount, icon: 'movie', route: '/admin/movies' },
      { label: 'Categories', value: this.dashboard.categoriesCount, icon: 'category', route: '/admin/categories' },
      { label: 'Actors', value: this.dashboard.actorsCount, icon: 'person', route: '/admin/actors' },
      { label: 'Directors', value: this.dashboard.directorsCount, icon: 'people', route: '/admin/directors' },
      { label: 'Reviews', value: this.dashboard.reviewsCount, icon: 'rate_review', route: '/admin/reviews' },
      { label: 'Users', value: this.dashboard.usersCount, icon: 'group' },
      { label: 'Favourites', value: this.dashboard.favouritesCount, icon: 'favorite', route: '/admin/favourite-movies' },
      { label: 'Cart Items', value: this.dashboard.activeCartItemsCount, icon: 'shopping_cart', route: '/admin/shopping-cart' },
      { label: 'Notifications', value: this.dashboard.notificationsCount, icon: 'notifications', route: '/admin/notifications' },
    ];
  }

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.adminApi.getDashboard().subscribe({
      next: dashboard => {
        this.dashboard = dashboard;
        this.isLoading = false;
      },
      error: (err: HttpErrorResponse) => {
        this.errorMessage = err.error?.message || 'Failed to load dashboard data.';
        this.isLoading = false;
      },
    });
  }
}
