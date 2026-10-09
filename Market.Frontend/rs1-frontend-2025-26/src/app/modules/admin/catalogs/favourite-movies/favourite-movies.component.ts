import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseListComponent } from '../../../../core/components/base-classes/base-list-component';
import {
  GetMyFavouriteMoviesQueryDto,
} from '../../../../api-services/favourite-movies/favourite-movies-api.model';
import { FavouriteMoviesApiService } from '../../../../api-services/favourite-movies/favourite-movies-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';

@Component({
  selector: 'app-favourite-movies',
  standalone: false,
  templateUrl: './favourite-movies.component.html',
  styleUrl: './favourite-movies.component.scss',
})
export class FavouriteMoviesComponent extends BaseListComponent<GetMyFavouriteMoviesQueryDto> implements OnInit {
  private api = inject(FavouriteMoviesApiService);
  private toaster = inject(ToasterService);
  private router = inject(Router);
  private confirm = inject(MovieShopConfirmService);

  displayedColumns: string[] = ['title', 'director', 'releaseDate', 'score', 'price', 'dateAdded', 'actions'];

  ngOnInit(): void {
    this.initList();
  }

  protected loadData(): void {
    this.startLoading();
    this.api.getMine().subscribe({
      next: (result) => {
        this.items = result;
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load favourite movies.');
        console.error('Load favourite movies error:', err);
      },
    });
  }

  onRemove(item: GetMyFavouriteMoviesQueryDto): void {
    this.confirm.confirm({
      title: 'Remove Favourite',
      message: `Remove "${item.title}" from favourites?`,
      confirmText: 'Remove',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.remove(item.movieId).subscribe({
        next: () => {
          this.toaster.success('Movie removed from favourites.');
          this.loadData();
        },
        error: (err) => {
          this.stopLoading('Failed to remove movie from favourites.');
          console.error('Remove favourite movie error:', err);
        },
      });
    });
  }

  goToMovies(): void {
    this.router.navigate(['/admin/movies']);
  }
}
