import { Component, OnInit, ViewEncapsulation, inject } from '@angular/core';
import { Options } from '@angular-slider/ngx-slider';
import { PageEvent } from '@angular/material/paginator';
import { CategoriesApiService } from '../../../../api-services/categories/categories-api.service';
import { ListCategoriesQueryDto, ListCategoriesRequest } from '../../../../api-services/categories/categories-api.model';
import { ListMoviesQueryDto, ListMoviesRequest } from '../../../../api-services/movies/movies-api.model';
import { MoviesApiService } from '../../../../api-services/movies/movies-api.service';

type MoviesViewMode = 'list' | 'grid';
type MoviesSortMode = 'Last Added' | 'Most Popular';

@Component({
  selector: 'app-movies-browse',
  standalone: false,
  templateUrl: './movies-browse.component.html',
  styleUrl: './movies-browse.component.scss',
  encapsulation: ViewEncapsulation.None,
})
export class MoviesBrowseComponent implements OnInit {
  private moviesApi = inject(MoviesApiService);
  private categoriesApi = inject(CategoriesApiService);

  isLoading = false;
  errorMessage = '';

  movies: ListMoviesQueryDto[] = [];
  categories: ListCategoriesQueryDto[] = [];

  pageTitle = 'Movie List';
  viewMode: MoviesViewMode = 'list';
  sortBy: MoviesSortMode = 'Last Added';

  searchTitle = '';
  selectedCategoryId: number | null = null;
  minValue = 0;
  maxValue = 0;
  options: Options = {
    floor: 0,
    ceil: 0,
  };

  totalMovies = 0;
  currentPage = 1;
  pageSize = 5;

  get categoryOptions(): ListCategoriesQueryDto[] {
    return [{ id: 0, categoryName: 'All' }, ...this.categories];
  }

  ngOnInit(): void {
    this.loadCategories();
    this.loadPriceRange();
  }

  onSearchChange(value: string): void {
    this.searchTitle = value;
    this.applyFilters();
  }

  applyFilters(): void {
    this.currentPage = 1;
    this.loadMovies();
  }

  clearFilters(): void {
    this.searchTitle = '';
    this.selectedCategoryId = null;
    this.minValue = 0;
    this.maxValue = this.options.ceil ?? 0;
    this.sortBy = 'Last Added';
    this.currentPage = 1;
    this.loadMovies();
  }

  onCategoryChange(categoryId: number): void {
    this.selectedCategoryId = categoryId || null;
    this.applyFilters();
  }

  onSortChange(value: MoviesSortMode): void {
    this.sortBy = value;
    this.applyFilters();
  }

  setSliderValue(): void {
    this.applyFilters();
  }

  changeView(view: MoviesViewMode): void {
    this.viewMode = view;
  }

  onPageChange(event: PageEvent): void {
    this.currentPage = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.loadMovies();
  }

  private loadCategories(): void {
    const request = new ListCategoriesRequest();
    request.paging.page = 1;
    request.paging.pageSize = 500;

    this.categoriesApi.list(request).subscribe({
      next: (result) => {
        this.categories = result.items ?? [];
      },
      error: (err) => {
        console.error('Load categories error:', err);
      },
    });
  }

  private loadPriceRange(): void {
    const request = new ListMoviesRequest();
    request.paging.page = 1;
    request.paging.pageSize = 1000;
    request.sortedColumn = 'LastAddedMovies';
    request.isDescending = true;

    this.moviesApi.list(request).subscribe({
      next: (result) => {
        const lastPrice = Math.max(0, ...result.items.map(movie => movie.price));
        this.options = {
          ceil: lastPrice,
          translate: (value: number): string => `€ ${value}`,
        };
        this.minValue = 0;
        this.maxValue = lastPrice;
        this.loadMovies();
      },
      error: (err) => {
        console.error('Load movie price range error:', err);
        this.loadMovies();
      },
    });
  }

  private loadMovies(): void {
    this.isLoading = true;
    this.errorMessage = '';

    const request = this.buildRequest();

    this.moviesApi.list(request).subscribe({
      next: (result) => {
        this.movies = result.items ?? [];
        this.totalMovies = result.totalItems ?? 0;
        this.currentPage = result.currentPage || this.currentPage;
        this.pageSize = result.pageSize || this.pageSize;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Load movies error:', err);
        this.errorMessage = 'Failed to load movies.';
        this.isLoading = false;
      },
    });
  }

  private buildRequest(): ListMoviesRequest {
    const request = new ListMoviesRequest();
    request.paging.page = this.currentPage;
    request.paging.pageSize = this.pageSize;

    request.title = this.searchTitle.trim() || null;
    request.categoryId = this.selectedCategoryId ?? null;

    request.priceFrom = this.options.ceil ? this.minValue : null;
    request.priceTo = this.options.ceil ? this.maxValue : null;

    if (this.sortBy === 'Most Popular') {
      request.sortedColumn = 'AverageScore';
      request.isDescending = true;
    } else {
      request.sortedColumn = 'LastAddedMovies';
      request.isDescending = true;
    }

    return request;
  }
}
