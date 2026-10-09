import { Component, Input } from '@angular/core';
import { ListMoviesQueryDto } from '../../../../api-services/movies/movies-api.model';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-movie-list-card',
  standalone: false,
  templateUrl: './movie-list-card.component.html',
  styleUrl: './movie-list-card.component.scss',
})
export class MovieListCardComponent {
  @Input({ required: true }) movie!: ListMoviesQueryDto;

  constructor(readonly imageResolver: MovieShopImageService) {}

  getMovieImage(): string {
    return this.imageResolver.resolveMovieImage(this.movie);
  }

  get categoriesLabel(): string {
    if (!this.movie?.categories?.length) return 'Uncategorized';
    return this.movie.categories.map((x) => x.categoryName).join(', ');
  }

  get formattedDuration(): string {
    const hours = Math.floor(this.movie.duration / 60);
    const minutes = this.movie.duration % 60;
    return hours ? `${hours}h ${minutes}m` : `${minutes}m`;
  }

  get trailerId(): string | null {
    const raw = this.movie.trailerLink?.trim();
    if (!raw) return null;
    if (!raw.includes('http')) return raw;
    try {
      const url = new URL(raw);
      return url.hostname.includes('youtu.be') ? url.pathname.slice(1) : url.searchParams.get('v');
    } catch {
      return null;
    }
  }
}
