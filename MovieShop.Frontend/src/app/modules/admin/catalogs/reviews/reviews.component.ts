import { Component, OnInit, inject } from '@angular/core';
import { catchError, forkJoin, of } from 'rxjs';
import { ListMoviesRequest } from '../../../../api-services/movies/movies-api.model';
import { MoviesApiService } from '../../../../api-services/movies/movies-api.service';
import {
  GetReviewsByMovieIdQueryDto,
  ListReviewsByMovieRequest,
} from '../../../../api-services/reviews/reviews-api.model';
import { ReviewsApiService } from '../../../../api-services/reviews/reviews-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';

interface AdminReviewRow extends GetReviewsByMovieIdQueryDto {
  movieTitle: string;
}

@Component({
  selector: 'app-reviews',
  standalone: false,
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.scss',
})
export class ReviewsComponent implements OnInit {
  private reviewsApi = inject(ReviewsApiService);
  private moviesApi = inject(MoviesApiService);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);

  reviews: AdminReviewRow[] = [];
  isLoading = false;
  errorMessage = '';
  deletingReviewIds = new Set<number>();

  ngOnInit(): void {
    this.loadReviews();
  }

  deleteReview(id: number): void {
    if (this.deletingReviewIds.has(id)) return;

    this.confirm.confirm({
      title: 'Delete Review',
      message: 'Delete this review?',
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.deletingReviewIds.add(id);
      this.reviewsApi.delete(id).subscribe({
        next: () => {
          this.deletingReviewIds.delete(id);
          this.reviews = this.reviews.filter(review => review.id !== id);
          this.toaster.success('Review deleted successfully.');
        },
        error: err => {
          this.deletingReviewIds.delete(id);
          console.error('Delete review error:', err);
          this.toaster.error('Failed to delete review.');
        },
      });
    });
  }

  isDeletingReview(id: number): boolean {
    return this.deletingReviewIds.has(id);
  }

  private loadReviews(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const moviesRequest = new ListMoviesRequest();
    moviesRequest.paging.page = 1;
    moviesRequest.paging.pageSize = 1000;

    this.moviesApi.list(moviesRequest).subscribe({
      next: movieResult => {
        if (!movieResult.items.length) {
          this.reviews = [];
          this.isLoading = false;
          return;
        }

        const requests = movieResult.items.map(movie => {
          const request = new ListReviewsByMovieRequest();
          request.movieId = movie.id;
          request.paging.page = 1;
          request.paging.pageSize = 1000;

          return this.reviewsApi.listByMovie(request).pipe(
            catchError(err => {
              console.error(`Load reviews for movie ${movie.id} error:`, err);
              return of({ items: [], totalItems: 0, currentPage: 1, pageSize: 1000 });
            }),
          );
        });

        forkJoin(requests).subscribe({
          next: reviewResults => {
            this.reviews = reviewResults
              .flatMap((result, index) =>
                result.items.map(review => ({
                  ...review,
                  movieTitle: movieResult.items[index].title,
                })),
              )
              .sort((a, b) => new Date(b.reviewDate).getTime() - new Date(a.reviewDate).getTime());
            this.isLoading = false;
          },
          error: err => {
            console.error('Load reviews error:', err);
            this.errorMessage = 'Failed to load reviews.';
            this.isLoading = false;
          },
        });
      },
      error: err => {
        console.error('Load movies for reviews error:', err);
        this.errorMessage = 'Failed to load reviews.';
        this.isLoading = false;
      },
    });
  }
}
