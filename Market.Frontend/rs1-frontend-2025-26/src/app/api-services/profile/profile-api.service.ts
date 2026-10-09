import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ChangeMyEmailCommand,
  ChangeMyPasswordCommand,
  GetMyProfileQueryDto,
  UpdateMyProfileImageCommand,
} from './profile-api.model';

@Injectable({
  providedIn: 'root',
})
export class ProfileApiService {
  private readonly baseUrl = `${environment.apiUrl}/Profile`;
  private http = inject(HttpClient);

  getMine(): Observable<GetMyProfileQueryDto> {
    return this.http.get<GetMyProfileQueryDto>(`${this.baseUrl}/my`);
  }

  changeEmail(payload: ChangeMyEmailCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/email`, payload);
  }

  changePassword(payload: ChangeMyPasswordCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/password`, payload);
  }

  updateProfileImage(payload: UpdateMyProfileImageCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/profile-image`, payload);
  }
}
