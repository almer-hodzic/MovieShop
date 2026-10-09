import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateCategoryCommand,
  GetCategoryByIdQueryDto,
  ListCategoriesRequest,
  ListCategoriesResponse,
  UpdateCategoryCommand,
} from './categories-api.model';

@Injectable({
  providedIn: 'root',
})
export class CategoriesApiService {
  private readonly baseUrl = `${environment.apiUrl}/Categories`;
  private http = inject(HttpClient);

  list(request?: ListCategoriesRequest): Observable<ListCategoriesResponse> {
    const params = request ? buildHttpParams(request as unknown as Record<string, unknown>) : undefined;
    return this.http.get<ListCategoriesResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetCategoryByIdQueryDto> {
    return this.http.get<GetCategoryByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateCategoryCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateCategoryCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
