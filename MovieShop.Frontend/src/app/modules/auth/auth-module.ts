import {NgModule} from '@angular/core';

import {AuthRoutingModule} from './auth-routing-module';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './login/login.component';
import {RegisterComponent} from './register/register.component';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {LogoutComponent} from './logout/logout.component';
import {SharedModule} from '../shared/shared-module';
import {ResetPasswordComponent} from './reset-password/reset-password.component';
import {ConfirmEmailComponent} from './confirm-email/confirm-email.component';
import {VerifyTwoFactorComponent} from './verify-two-factor/verify-two-factor.component';


@NgModule({
  declarations: [
    AuthLayoutComponent,
    LoginComponent,
    RegisterComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ConfirmEmailComponent,
    VerifyTwoFactorComponent,
    LogoutComponent
  ],
  imports: [
    AuthRoutingModule,
    SharedModule
  ]
})
export class AuthModule { }
