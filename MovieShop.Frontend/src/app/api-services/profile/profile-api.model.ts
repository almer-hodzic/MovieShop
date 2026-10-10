export interface GetMyProfileQueryDto {
  id: number;
  username: string;
  email: string;
  firstname: string;
  lastname: string;
  isAdmin: boolean;
  isManager: boolean;
  isEmployee: boolean;
  isEnabled: boolean;
  profileImage?: string | null;
}

export interface ChangeMyEmailCommand {
  email: string;
}

export interface ChangeMyPasswordCommand {
  currentPassword: string;
  newPassword: string;
}

export interface UpdateMyProfileImageCommand {
  profileImageBase64: string;
}
