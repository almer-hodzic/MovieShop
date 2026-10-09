import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './login/login.component';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {RegisterComponent} from './register/register.component';
import {LogoutComponent} from './logout/logout.component';
import {ResetPasswordComponent} from './reset-password/reset-password.component';
import {ConfirmEmailComponent} from './confirm-email/confirm-email.component';
import {VerifyTwoFactorComponent} from './verify-two-factor/verify-two-factor.component';

const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'register', component: RegisterComponent },
      { path: 'forgot-password', component: ForgotPasswordComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      { path: 'confirm-email', component: ConfirmEmailComponent },
      { path: 'verify-two-factor', component: VerifyTwoFactorComponent },
      { path: 'verify-2fa', component: VerifyTwoFactorComponent },
      { path: 'forgotPass', redirectTo: 'forgot-password' },
      { path: 'restartPass', redirectTo: 'reset-password' },
      { path: 'confirmEmail', redirectTo: 'confirm-email' },
      { path: 'verifyTwoFactor', redirectTo: 'verify-two-factor' },
      { path: 'logout', component: LogoutComponent },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
