// === COMMANDS (WRITE) ===

/**
 * Command for POST /Auth/login
 * Corresponds to: LoginCommand.cs
 */
export interface LoginCommand {
  email: string;
  password: string;
  fingerprint?: string | null;
}

/**
 * Response for POST /Auth/login
 * Corresponds to: LoginCommandDto.cs
 */
export interface LoginCommandDto {
  userId: number;
  requiresTwoFactor: boolean;
  emailDeliveryFallbackUsed?: boolean;
  emailDeliveryMessage?: string | null;
  accessToken: string;
  refreshToken: string;
  /**
   * ISO string (UTC) returned by backend
   * Example: "2025-12-02T23:59:59Z"
   */
  expiresAtUtc: string;
}

export interface VerifyTwoFactorCommand {
  userId: number;
  code: string;
  password: string;
  fingerprint?: string | null;
}

/**
 * Command for POST /Auth/refresh
 * Corresponds to: RefreshTokenCommand.cs
 */
export interface RefreshTokenCommand {
  refreshToken: string;
  fingerprint?: string | null;
}

/**
 * Response for POST /Auth/refresh
 * Corresponds to: RefreshTokenCommandDto.cs
 */
export interface RefreshTokenCommandDto {
  accessToken: string;
  refreshToken: string;
  /**
   * ISO string (UTC) when access token expires
   */
  accessTokenExpiresAtUtc: string;
  /**
   * ISO string (UTC) when refresh token expires
   */
  refreshTokenExpiresAtUtc: string;
}

/**
 * Command for POST /Auth/logout
 * Corresponds to: LogoutCommand.cs
 */
export interface LogoutCommand {
  refreshToken: string;
}

export interface RegisterCommand {
  firstname: string;
  lastname: string;
  email: string;
  password: string;
}

export interface RegisterCommandDto {
  id: number;
  email: string;
  firstname: string;
  lastname: string;
  isEnabled: boolean;
  isEmployee: boolean;
  isEmailConfirmed: boolean;
  emailDeliveryFallbackUsed?: boolean;
  emailDeliveryMessage?: string | null;
}

export interface ForgotPasswordCommand {
  email: string;
}

export interface ForgotPasswordCommandDto {
  email: string;
  message: string;
  emailDeliveryFallbackUsed?: boolean;
  emailDeliveryMessage?: string | null;
}

export interface ResetPasswordCommand {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface ResetPasswordCommandDto {
  email: string;
  message: string;
}

export interface ConfirmEmailCommand {
  email: string;
  token: string;
}

export interface ConfirmEmailCommandDto {
  id: number;
  email: string;
  isEmailConfirmed: boolean;
  emailConfirmedAtUtc: string;
}
