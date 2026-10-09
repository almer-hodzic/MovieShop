export interface GetMyNotificationsQueryDto {
  notificationId: number;
  isRead: boolean;
  notificationText: string;
  notificationDate: string;
  creatorId: number;
  movieId: number;
}

export interface CreateNotificationCommand {
  notificationText: string;
  movieId: number;
}
