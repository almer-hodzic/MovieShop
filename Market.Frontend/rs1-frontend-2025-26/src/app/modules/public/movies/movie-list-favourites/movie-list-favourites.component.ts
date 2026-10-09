import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  GetMyFavouriteMoviesQueryDto,
} from '../../../../api-services/favourite-movies/favourite-movies-api.model';
import { FavouriteMoviesApiService } from '../../../../api-services/favourite-movies/favourite-movies-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-movie-list-favourites',
  standalone: false,
  templateUrl: './movie-list-favourites.component.html',
  styleUrl: './movie-list-favourites.component.scss',
})
export class MovieListFavouritesComponent implements OnInit {
  private favouritesApi = inject(FavouriteMoviesApiService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private confirm = inject(MovieShopConfirmService);
  readonly imageResolver = inject(MovieShopImageService);

  favouriteMovies: GetMyFavouriteMoviesQueryDto[] = [];
  isLoading = false;
  errorMessage = '';
  removingMovieIds = new Set<number>();

  ngOnInit(): void {
    this.getFavouriteMovies();
  }

  getFavouriteMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.favouritesApi.getMine().subscribe({
      next: (movies) => {
        this.favouriteMovies = movies;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Load favourite movies error:', err);
        this.errorMessage = err?.status === 401 || err?.status === 403
          ? 'You must be logged in to view favourites.'
          : 'Failed to load favourite movies.';
        this.isLoading = false;
      },
    });
  }

  removeFromFavorites(event: MouseEvent, movie: GetMyFavouriteMoviesQueryDto): void {
    event.preventDefault();
    event.stopPropagation();

    if (this.removingMovieIds.has(movie.movieId)) {
      return;
    }

    this.confirm.confirm({
      title: 'Remove Favourite',
      message: `Remove "${movie.title}" from favourites?`,
      confirmText: 'Remove',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.removingMovieIds.add(movie.movieId);

      this.favouritesApi.remove(movie.movieId).subscribe({
        next: () => {
          this.favouriteMovies = this.favouriteMovies.filter((item) => item.movieId !== movie.movieId);
          this.removingMovieIds.delete(movie.movieId);
          this.toaster.success('Movie removed from favourites.');
          this.router.navigate(['/movies/Favourites']);
        },
        error: (err) => {
          console.error('Remove favourite movie error:', err);
          this.removingMovieIds.delete(movie.movieId);
          if (err?.status === 404) {
            this.favouriteMovies = this.favouriteMovies.filter((item) => item.movieId !== movie.movieId);
            this.toaster.warning('Movie is not in favourites.');
          } else if (err?.status === 401 || err?.status === 403) {
            this.toaster.error('You must be logged in to manage favourites.');
          } else {
            this.toaster.error('Failed to remove movie from favourites.');
          }
        },
      });
    });
  }

  isRemoving(movieId: number): boolean {
    return this.removingMovieIds.has(movieId);
  }

  getMovieImage(movie: GetMyFavouriteMoviesQueryDto): string {
    return this.imageResolver.resolveMovieImage(movie);
  }
}
