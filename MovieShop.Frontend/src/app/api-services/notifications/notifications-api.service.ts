import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CreateNotificationCommand,
  GetMyNotificationsQueryDto,
} from './notifications-api.model';

@Injectable({
  providedIn: 'root',
})
export class NotificationsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Notifications`;
  private http = inject(HttpClient);

  getMine(): Observable<GetMyNotificationsQueryDto[]> {
    return this.http.get<GetMyNotificationsQueryDto[]>(`${this.baseUrl}/my`);
  }

  markAsRead(notificationId: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${notificationId}/read`, {});
  }

  delete(notificationId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${notificationId}`);
  }

  create(payload: CreateNotificationCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }
}
