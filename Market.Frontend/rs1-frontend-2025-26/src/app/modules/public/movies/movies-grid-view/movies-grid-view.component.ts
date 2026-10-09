import { Component, Input } from '@angular/core';
import { ListMoviesQueryDto } from '../../../../api-services/movies/movies-api.model';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-movies-grid-view',
  standalone: false,
  templateUrl: './movies-grid-view.component.html',
  styleUrl: './movies-grid-view.component.scss',
})
export class MoviesGridViewComponent {
  @Input() movies: ListMoviesQueryDto[] = [];

  constructor(readonly imageResolver: MovieShopImageService) {}

  getMovieImage(movie: ListMoviesQueryDto): string {
    return this.imageResolver.resolveMovieImage(movie);
  }

  getLegacyMovieImage(movie: ListMoviesQueryDto): string {
    return this.imageResolver.resolveLegacyMovieImage(movie) ?? this.imageResolver.movieFallback;
  }
}
