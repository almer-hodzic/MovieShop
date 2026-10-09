import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateReviewCommand,
  GetReviewByIdQueryDto,
  ListReviewsByMovieRequest,
  ListReviewsByMovieResponse,
} from './reviews-api.model';

@Injectable({
  providedIn: 'root',
})
export class ReviewsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Reviews`;
  private http = inject(HttpClient);

  listByMovie(request: ListReviewsByMovieRequest): Observable<ListReviewsByMovieResponse> {
    const params = buildHttpParams(request as unknown as Record<string, unknown>);
    return this.http.get<ListReviewsByMovieResponse>(`${this.baseUrl}/by-movie`, { params });
  }

  getById(id: number): Observable<GetReviewByIdQueryDto> {
    return this.http.get<GetReviewByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateReviewCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
