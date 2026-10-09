import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateActorCommand,
  GetActorByIdQueryDto,
  ListActorsRequest,
  ListActorsResponse,
  UpdateActorCommand,
} from './actors-api.model';

@Injectable({
  providedIn: 'root',
})
export class ActorsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Actors`;
  private http = inject(HttpClient);

  list(request?: ListActorsRequest): Observable<ListActorsResponse> {
    const params = request ? buildHttpParams(request as unknown as Record<string, unknown>) : undefined;
    return this.http.get<ListActorsResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetActorByIdQueryDto> {
    return this.http.get<GetActorByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateActorCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateActorCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
