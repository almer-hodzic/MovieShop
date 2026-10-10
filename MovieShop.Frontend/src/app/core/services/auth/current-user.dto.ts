export interface CurrentUserDto {
  userId: number;
  username: string;
  email: string;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  tokenVersion: number;
}
