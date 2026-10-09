import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateDirectorCommand,
  GetDirectorByIdQueryDto,
  ListDirectorsRequest,
  ListDirectorsResponse,
  UpdateDirectorCommand,
} from './directors-api.model';

@Injectable({
  providedIn: 'root',
})
export class DirectorsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Directors`;
  private http = inject(HttpClient);

  list(request?: ListDirectorsRequest): Observable<ListDirectorsResponse> {
    const params = request ? buildHttpParams(request as unknown as Record<string, unknown>) : undefined;
    return this.http.get<ListDirectorsResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetDirectorByIdQueryDto> {
    return this.http.get<GetDirectorByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateDirectorCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateDirectorCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
