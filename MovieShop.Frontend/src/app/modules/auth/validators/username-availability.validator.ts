import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { Observable, catchError, map, of, switchMap, timer } from 'rxjs';
import { AuthApiService } from '../../../api-services/auth/auth-api.service';

export function usernameAvailabilityValidator(authApi: AuthApiService): AsyncValidatorFn {
  return (control: AbstractControl): Observable<ValidationErrors | null> => {
    const username = String(control.value ?? '').trim();

    if (!username || username.length < 3 || username.length > 100 || !/^[a-zA-Z0-9._-]+$/.test(username)) {
      return of(null);
    }

    return timer(350).pipe(
      switchMap(() => authApi.checkUsernameAvailability(username)),
      map(result => result.available ? null : { usernameTaken: true }),
      catchError(() => of(null))
    );
  };
}
