import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AdminDashboardDto,
  AdminSettingsDto,
  UpdateAdminSettingsCommand,
} from './admin-api.model';

@Injectable({
  providedIn: 'root',
})
export class AdminApiService {
  private readonly baseUrl = `${environment.apiUrl}/Admin`;
  private http = inject(HttpClient);

  getDashboard(): Observable<AdminDashboardDto> {
    return this.http.get<AdminDashboardDto>(`${this.baseUrl}/Dashboard`);
  }

  getSettings(): Observable<AdminSettingsDto> {
    return this.http.get<AdminSettingsDto>(`${this.baseUrl}/Settings`);
  }

  updateSettings(payload: UpdateAdminSettingsCommand): Observable<AdminSettingsDto> {
    return this.http.put<AdminSettingsDto>(`${this.baseUrl}/Settings`, payload);
  }
}
