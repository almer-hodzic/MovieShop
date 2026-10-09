import { Component, Input } from '@angular/core';
import { ListMoviesQueryDto } from '../../../../api-services/movies/movies-api.model';

@Component({
  selector: 'app-movies-list-view',
  standalone: false,
  templateUrl: './movies-list-view.component.html',
  styleUrl: './movies-list-view.component.scss',
})
export class MoviesListViewComponent {
  @Input() movies: ListMoviesQueryDto[] = [];
}
