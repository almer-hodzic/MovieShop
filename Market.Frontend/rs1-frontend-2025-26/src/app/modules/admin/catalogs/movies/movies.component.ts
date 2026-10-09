import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  ListMoviesQueryDto,
  ListMoviesRequest,
} from '../../../../api-services/movies/movies-api.model';
import { MoviesApiService } from '../../../../api-services/movies/movies-api.service';
import { FavouriteMoviesApiService } from '../../../../api-services/favourite-movies/favourite-movies-api.service';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';

@Component({
  selector: 'app-movies',
  standalone: false,
  templateUrl: './movies.component.html',
  styleUrl: './movies.component.scss',
})
export class MoviesComponent
  extends BaseListPagedComponent<ListMoviesQueryDto, ListMoviesRequest>
  implements OnInit
{
  private api = inject(MoviesApiService);
  private favouritesApi = inject(FavouriteMoviesApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);

  displayedColumns: string[] = ['id', 'title', 'director', 'releaseDate', 'price', 'categories', 'actors', 'favourite', 'actions'];
  private favouriteMovieIds = new Set<number>();

  constructor() {
    super();
    this.request = new ListMoviesRequest();
  }

  ngOnInit(): void {
    this.initList();
    this.loadMyFavourites();
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (result) => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load movies.');
        console.error('Load movies error:', err);
      },
    });
  }

  onSearch(value: string): void {
    this.request.title = value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.router.navigate(['/admin/movies/new']);
  }

  onEdit(item: ListMoviesQueryDto): void {
    this.router.navigate(['/admin/movies', item.id, 'edit']);
  }

  onDelete(item: ListMoviesQueryDto): void {
    this.confirm.confirm({
      title: 'Delete Movie',
      message: `Delete movie "${item.title}"?`,
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.delete(item.id).subscribe({
        next: () => {
          this.toaster.success('Movie deleted successfully.');
          this.loadPagedData();
        },
        error: (err) => {
          this.stopLoading('Failed to delete movie.');
          console.error('Delete movie error:', err);
        },
      });
    });
  }

  onAddToFavourites(item: ListMoviesQueryDto): void {
    this.favouritesApi.add({ movieId: item.id }).subscribe({
      next: () => {
        this.favouriteMovieIds.add(item.id);
        this.toaster.success(`"${item.title}" added to favourites.`);
      },
      error: (err) => {
        console.error('Add to favourites error:', err);
        if (err?.status === 409) {
          this.favouriteMovieIds.add(item.id);
          this.toaster.warning('Movie is already in favourites.');
          return;
        }
        if (err?.status === 401 || err?.status === 403) {
          this.toaster.error('You must be logged in to add favourites.');
          return;
        }
        this.toaster.error('Failed to add movie to favourites.');
      },
    });
  }

  isFavourite(item: ListMoviesQueryDto): boolean {
    return this.favouriteMovieIds.has(item.id);
  }

  getCategoriesLabel(item: ListMoviesQueryDto): string {
    if (!item.categories?.length) {
      return '-';
    }

    return item.categories.map((x) => x.categoryName).join(', ');
  }

  getActorsLabel(item: ListMoviesQueryDto): string {
    if (!item.actors?.length) {
      return '-';
    }

    return item.actors
      .map((x) => `${x.firstName} ${x.lastName}`.trim())
      .join(', ');
  }

  private loadMyFavourites(): void {
    this.favouritesApi.getMine().subscribe({
      next: (result) => {
        this.favouriteMovieIds = new Set(result.map((x) => x.movieId));
      },
      error: (err) => {
        console.error('Load my favourites for movies list error:', err);
      },
    });
  }
}
