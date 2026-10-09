import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateMovieCommand,
  GetMovieByIdQueryDto,
  ListMoviesRequest,
  ListMoviesResponse,
  UpdateMovieCommand,
} from './movies-api.model';

@Injectable({
  providedIn: 'root',
})
export class MoviesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Movies`;
  private http = inject(HttpClient);

  list(request?: ListMoviesRequest): Observable<ListMoviesResponse> {
    const params = request ? buildHttpParams(request as unknown as Record<string, unknown>) : undefined;
    return this.http.get<ListMoviesResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetMovieByIdQueryDto> {
    return this.http.get<GetMovieByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateMovieCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateMovieCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
