import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { CategoriesComponent } from './catalogs/categories/categories.component';
import { CategoryUpsertComponent } from './catalogs/categories/category-upsert/category-upsert.component';
import { DirectorsComponent } from './catalogs/directors/directors.component';
import { DirectorUpsertComponent } from './catalogs/directors/director-upsert/director-upsert.component';
import { ActorsComponent } from './catalogs/actors/actors.component';
import { ActorUpsertComponent } from './catalogs/actors/actor-upsert/actor-upsert.component';
import { MoviesComponent } from './catalogs/movies/movies.component';
import { MovieUpsertComponent } from './catalogs/movies/movie-upsert/movie-upsert.component';
import { ReviewsComponent } from './catalogs/reviews/reviews.component';
import { FavouriteMoviesComponent } from './catalogs/favourite-movies/favourite-movies.component';
import { ShoppingCartComponent } from './catalogs/shopping-cart/shopping-cart.component';
import { NotificationsComponent } from './catalogs/notifications/notifications.component';
import { AdminLauncherComponent } from './admin-launcher/admin-launcher.component';
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';

const routes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        component: AdminLauncherComponent,
        pathMatch: 'full',
      },
      {
        path: 'categories',
        component: CategoriesComponent,
      },
      {
        path: 'categories/new',
        component: CategoryUpsertComponent,
      },
      {
        path: 'categories/:id/edit',
        component: CategoryUpsertComponent,
      },
      {
        path: 'directors',
        component: DirectorsComponent,
      },
      {
        path: 'directors/new',
        component: DirectorUpsertComponent,
      },
      {
        path: 'directors/:id/edit',
        component: DirectorUpsertComponent,
      },
      {
        path: 'actors',
        component: ActorsComponent,
      },
      {
        path: 'actors/new',
        component: ActorUpsertComponent,
      },
      {
        path: 'actors/:id/edit',
        component: ActorUpsertComponent,
      },
      {
        path: 'movies',
        component: MoviesComponent,
      },
      {
        path: 'movies/new',
        component: MovieUpsertComponent,
      },
      {
        path: 'movies/:id/edit',
        component: MovieUpsertComponent,
      },
      {
        path: 'reviews',
        component: ReviewsComponent,
      },
      {
        path: 'favourite-movies',
        component: FavouriteMoviesComponent,
      },
      {
        path: 'shopping-cart',
        component: ShoppingCartComponent,
      },
      {
        path: 'notifications',
        component: NotificationsComponent,
      },
      {
        path: 'settings',
        component: AdminSettingsComponent,
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
