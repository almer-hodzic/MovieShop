import { Component, OnInit, inject } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Router } from '@angular/router';
import { PageEvent } from '@angular/material/paginator';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, Validators } from '@angular/forms';
import { MoviesApiService } from '../../../../api-services/movies/movies-api.service';
import { GetMovieByIdQueryDto } from '../../../../api-services/movies/movies-api.model';
import {
  CreateReviewCommand,
  GetReviewsByMovieIdQueryDto,
  ListReviewsByMovieRequest,
} from '../../../../api-services/reviews/reviews-api.model';
import { ReviewsApiService } from '../../../../api-services/reviews/reviews-api.service';
import { FavouriteMoviesApiService } from '../../../../api-services/favourite-movies/favourite-movies-api.service';
import { ShoppingCartApiService } from '../../../../api-services/shopping-cart/shopping-cart-api.service';
import { CurrentUserService } from '../../../../core/services/auth/current-user.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { ActorsApiService } from '../../../../api-services/actors/actors-api.service';
import { GetActorByIdQueryDto } from '../../../../api-services/actors/actors-api.model';
import { GetMovieByIdActorDto } from '../../../../api-services/movies/movies-api.model';
import { ActorDialogComponent } from '../../../shared/components/actor-dialog/actor-dialog.component';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';
import { catchError, forkJoin, of } from 'rxjs';

@Component({
  selector: 'app-movie-details',
  standalone: false,
  templateUrl: './movie-details.component.html',
  styleUrl: './movie-details.component.scss',
})
export class MovieDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private dialog = inject(MatDialog);
  private fb = inject(FormBuilder);
  private sanitizer = inject(DomSanitizer);
  private moviesApi = inject(MoviesApiService);
  private reviewsApi = inject(ReviewsApiService);
  private favouritesApi = inject(FavouriteMoviesApiService);
  private cartApi = inject(ShoppingCartApiService);
  private currentUser = inject(CurrentUserService);
  private toaster = inject(ToasterService);
  private actorsApi = inject(ActorsApiService);
  private confirm = inject(MovieShopConfirmService);
  readonly imageResolver = inject(MovieShopImageService);

  movieId = 0;
  movie: GetMovieByIdQueryDto | null = null;
  trailerUrl: SafeResourceUrl | null = null;

  isLoadingMovie = false;
  isLoadingReviews = false;
  isSubmittingReview = false;
  isTogglingFavourite = false;
  isAddingToCart = false;
  errorMessage = '';

  isFavorite = false;
  showTooltip = false;
  reviewRating = 0;
  actorDetails = new Map<number, GetActorByIdQueryDto>();
  deletingReviewIds = new Set<number>();

  reviews: GetReviewsByMovieIdQueryDto[] = [];
  totalReviews = 0;
  readonly reviewsRequest = new ListReviewsByMovieRequest();

  readonly reviewForm = this.fb.group({
    score: [0, [Validators.required, Validators.min(1), Validators.max(10)]],
    comment: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(81)]],
  });

  ngOnInit(): void {
    this.reviewsRequest.paging.page = 1;
    this.reviewsRequest.paging.pageSize = 3;

    this.route.paramMap.subscribe((params) => {
      const id = Number(params.get('id'));
      if (!id || Number.isNaN(id)) {
        this.errorMessage = 'Invalid movie id.';
        return;
      }

      this.movieId = id;
      this.reviewsRequest.movieId = id;
      this.reviewsRequest.paging.page = 1;

      this.loadMovie();
      this.loadReviews();
      this.loadFavouriteState();
    });
  }

  get isAuthenticated(): boolean {
    return this.currentUser.isAuthenticated();
  }

  get currentUserId(): number | null {
    return this.currentUser.snapshot?.userId ?? null;
  }

  get canCreateReview(): boolean {
    return this.isAuthenticated && !this.isSubmittingReview && !this.isLoadingMovie;
  }

  onReviewPageChange(event: PageEvent): void {
    this.reviewsRequest.paging.page = event.pageIndex + 1;
    this.reviewsRequest.paging.pageSize = event.pageSize;
    this.loadReviews();
  }

  onReviewSearch(value: string): void {
    this.reviewsRequest.userName = value.trim() || null;
    this.reviewsRequest.paging.page = 1;
    this.loadReviews();
  }

  onRatingChanged(rating: number): void {
    this.reviewRating = rating;
    this.reviewForm.patchValue({ score: rating * 2 });
  }

  onBack(): void {
    this.router.navigate(['/movies']);
  }

  openActorDialog(actor: GetMovieByIdActorDto): void {
    const details = this.actorDetails.get(actor.actorId);
    if (details) {
      this.dialog.open(ActorDialogComponent, { data: { ...details, characterName: actor.characterName } });
      return;
    }

    this.actorsApi.getById(actor.actorId).subscribe({
      next: result => {
        this.actorDetails.set(actor.actorId, result);
        this.dialog.open(ActorDialogComponent, { data: { ...result, characterName: actor.characterName } });
      },
      error: () => this.toaster.error('Failed to load actor details.'),
    });
  }

  getActorDetails(actorId: number): GetActorByIdQueryDto | undefined {
    return this.actorDetails.get(actorId);
  }

  submitReview(): void {
    this.reviewForm.markAllAsTouched();
    if (this.reviewForm.invalid) {
      return;
    }

    const userId = this.currentUserId;
    if (!userId) {
      this.toaster.error('You must be logged in to post a review.');
      return;
    }

    const payload: CreateReviewCommand = {
      movieId: this.movieId,
      score: Number(this.reviewForm.value.score ?? 0),
      comment: String(this.reviewForm.value.comment ?? '').trim(),
    };

    this.isSubmittingReview = true;
    this.reviewsApi.create(payload).subscribe({
      next: () => {
        this.toaster.success('Review posted successfully.');
        this.reviewRating = 0;
        this.reviewForm.patchValue({ score: 0, comment: '' });
        this.reviewForm.markAsPristine();
        this.reviewForm.markAsUntouched();
        this.loadMovie();
        this.loadReviews();
        this.isSubmittingReview = false;
      },
      error: (err) => {
        console.error('Create review error:', err);
        this.toaster.error('Failed to post review.');
        this.isSubmittingReview = false;
      },
    });
  }

  deleteReview(reviewId: number): void {
    if (this.deletingReviewIds.has(reviewId)) return;

    this.confirm.confirm({
      title: 'Delete Review',
      message: 'Delete your review?',
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.deletingReviewIds.add(reviewId);
      this.reviewsApi.delete(reviewId).subscribe({
        next: () => {
          this.deletingReviewIds.delete(reviewId);
          this.toaster.success('Review deleted successfully.');
          this.loadMovie();
          this.loadReviews();
        },
        error: (err) => {
          this.deletingReviewIds.delete(reviewId);
          console.error('Delete review error:', err);
          this.toaster.error('Failed to delete review.');
        },
      });
    });
  }

  canDeleteReview(review: GetReviewsByMovieIdQueryDto): boolean {
    return this.currentUserId === review.userId;
  }

  isDeletingReview(reviewId: number): boolean {
    return this.deletingReviewIds.has(reviewId);
  }

  getReviewUserName(userName: string): string {
    return userName.includes('@') ? userName.split('@')[0] : userName;
  }

  toggleFavourite(): void {
    if (!this.movie || this.isTogglingFavourite) return;

    if (!this.isAuthenticated) {
      this.toaster.error('You must be logged in to manage favourites.');
      return;
    }

    this.isTogglingFavourite = true;

    if (this.isFavorite) {
      this.confirm.confirm({
        title: 'Remove Favourite',
        message: `Remove "${this.movie.title}" from favourites?`,
        confirmText: 'Remove',
        tone: 'danger',
      }).subscribe((confirmed) => {
        if (!confirmed) {
          this.isTogglingFavourite = false;
          return;
        }

        this.favouritesApi.remove(this.movie!.id).subscribe({
          next: () => {
            this.isFavorite = false;
            this.toaster.success('Movie removed from favourites.');
            this.isTogglingFavourite = false;
          },
          error: (err) => {
            console.error('Remove favourite error:', err);
            if (err?.status === 404) {
              this.isFavorite = false;
              this.toaster.warning('Movie is not in favourites.');
            } else if (err?.status === 401 || err?.status === 403) {
              this.toaster.error('You must be logged in to manage favourites.');
            } else {
              this.toaster.error('Failed to remove movie from favourites.');
            }
            this.isTogglingFavourite = false;
          },
        });
      });
      return;
    }

    this.favouritesApi.add({ movieId: this.movie.id }).subscribe({
      next: () => {
        this.isFavorite = true;
        this.toaster.success('Movie added to favourites.');
        this.isTogglingFavourite = false;
      },
      error: (err) => {
        console.error('Add favourite error:', err);
        if (err?.status === 409) {
          this.isFavorite = true;
          this.toaster.warning('Movie is already in favourites.');
        } else if (err?.status === 401 || err?.status === 403) {
          this.toaster.error('You must be logged in to add favourites.');
        } else {
          this.toaster.error('Failed to add movie to favourites.');
        }
        this.isTogglingFavourite = false;
      },
    });
  }

  addToCart(): void {
    if (!this.movie || this.isAddingToCart) return;

    this.isAddingToCart = true;
    this.cartApi.addItem({ movieId: this.movie.id, quantity: 1 }).subscribe({
      next: () => {
        this.toaster.success('Movie added to shopping cart.');
        this.isAddingToCart = false;
      },
      error: (err) => {
        console.error('Add to cart error:', err);
        if (err?.status === 409) {
          this.toaster.warning('Movie is already in your shopping cart.');
        } else if (err?.status === 401 || err?.status === 403) {
          this.toaster.error('You must be logged in to add movies to the shopping cart.');
        } else {
          this.toaster.error('Failed to add movie to shopping cart.');
        }
        this.isAddingToCart = false;
      },
    });
  }

  getMovieImage(movie: GetMovieByIdQueryDto): string {
    return this.imageResolver.resolveMovieImage(movie);
  }

  getActorImage(actor: GetMovieByIdActorDto): string {
    return this.imageResolver.resolveActorImage({
      ...actor,
      photo: this.actorDetails.get(actor.actorId)?.photo,
    });
  }

  getScoreStars(score: number | null | undefined): string[] {
    const normalized = Math.max(0, Math.min(10, Number(score ?? 0)));
    const fiveStarValue = normalized / 2;
    const icons: string[] = [];

    for (let i = 1; i <= 5; i += 1) {
      if (fiveStarValue >= i) icons.push('star');
      else if (fiveStarValue >= i - 0.5) icons.push('star_half');
      else icons.push('star_border');
    }

    return icons;
  }

  get trailerRouteValue(): string | null {
    if (!this.movie?.trailerLink) return null;
    return this.extractYoutubeVideoId(this.movie.trailerLink);
  }

  private loadMovie(): void {
    this.isLoadingMovie = true;
    this.errorMessage = '';

    this.moviesApi.getById(this.movieId).subscribe({
      next: (result) => {
        this.movie = result;
        this.trailerUrl = this.buildTrailerUrl(result.trailerLink);
        this.loadActorDetails(result.actors);
        this.isLoadingMovie = false;
      },
      error: (err) => {
        console.error('Load movie details error:', err);
        this.errorMessage = 'Failed to load movie details.';
        this.isLoadingMovie = false;
      },
    });
  }

  private loadActorDetails(actors: GetMovieByIdActorDto[]): void {
    this.actorDetails.clear();
    if (!actors.length) return;

    forkJoin(actors.map(actor => this.actorsApi.getById(actor.actorId).pipe(catchError(() => of(null))))).subscribe(results => {
      results.forEach(result => {
        if (result) this.actorDetails.set(result.id, result);
      });
    });
  }

  private loadReviews(): void {
    this.isLoadingReviews = true;
    this.reviewsApi.listByMovie(this.reviewsRequest).subscribe({
      next: (result) => {
        this.reviews = result.items ?? [];
        this.totalReviews = result.totalItems ?? 0;
        this.reviewsRequest.paging.page = result.currentPage || this.reviewsRequest.paging.page;
        this.reviewsRequest.paging.pageSize = result.pageSize || this.reviewsRequest.paging.pageSize;
        this.isLoadingReviews = false;
      },
      error: (err) => {
        console.error('Load reviews error:', err);
        this.isLoadingReviews = false;
      },
    });
  }

  private loadFavouriteState(): void {
    if (!this.isAuthenticated) {
      this.isFavorite = false;
      return;
    }

    this.favouritesApi.getMine().subscribe({
      next: (items) => {
        this.isFavorite = items.some((x) => x.movieId === this.movieId);
      },
      error: (err) => {
        console.error('Load favourite state error:', err);
      },
    });
  }

  private buildTrailerUrl(rawTrailer: string | null | undefined): SafeResourceUrl | null {
    if (!rawTrailer) return null;
    const trailerId = this.extractYoutubeVideoId(rawTrailer);
    if (!trailerId) return null;
    return this.sanitizer.bypassSecurityTrustResourceUrl(`https://www.youtube.com/embed/${trailerId}`);
  }

  private extractYoutubeVideoId(rawTrailer: string): string | null {
    const value = rawTrailer.trim();
    if (!value) return null;

    if (!value.includes('http')) {
      return value;
    }

    try {
      const url = new URL(value);
      if (url.hostname.includes('youtu.be')) {
        return url.pathname.replace('/', '') || null;
      }
      if (url.hostname.includes('youtube.com')) {
        return url.searchParams.get('v');
      }
    } catch {
      return null;
    }

    return null;
  }
}
