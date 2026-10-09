import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AddFavouriteMovieCommand,
  GetMyFavouriteMoviesQueryDto,
} from './favourite-movies-api.model';

@Injectable({
  providedIn: 'root',
})
export class FavouriteMoviesApiService {
  private readonly baseUrl = `${environment.apiUrl}/FavouriteMovies`;
  private http = inject(HttpClient);

  getMine(): Observable<GetMyFavouriteMoviesQueryDto[]> {
    return this.http.get<GetMyFavouriteMoviesQueryDto[]>(`${this.baseUrl}/my`);
  }

  add(payload: AddFavouriteMovieCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  remove(movieId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${movieId}`);
  }
}
