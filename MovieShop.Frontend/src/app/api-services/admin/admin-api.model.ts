export interface AdminDashboardDto {
  moviesCount: number;
  categoriesCount: number;
  actorsCount: number;
  directorsCount: number;
  reviewsCount: number;
  usersCount: number;
  favouritesCount: number;
  activeCartItemsCount: number;
  notificationsCount: number;
}

export interface AdminSettingsDto {
  id: number;
  email: string;
  firstname: string;
  lastname: string;
  isAdmin: boolean;
  isEnabled: boolean;
}

export interface UpdateAdminSettingsCommand {
  firstname: string;
  lastname: string;
}
