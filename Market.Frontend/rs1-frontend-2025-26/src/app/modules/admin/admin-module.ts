import { NgModule } from '@angular/core';

import { AdminRoutingModule } from './admin-routing-module';
import { AdminLayoutComponent } from './admin-layout/admin-layout.component';
import { AdminLauncherComponent } from './admin-launcher/admin-launcher.component';
import { SharedModule } from '../shared/shared-module';
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
import { AdminSettingsComponent } from './admin-settings/admin-settings.component';

@NgModule({
  declarations: [
    AdminLayoutComponent,
    AdminLauncherComponent,
    CategoriesComponent,
    CategoryUpsertComponent,
    DirectorsComponent,
    DirectorUpsertComponent,
    ActorsComponent,
    ActorUpsertComponent,
    MoviesComponent,
    MovieUpsertComponent,
    ReviewsComponent,
    FavouriteMoviesComponent,
    ShoppingCartComponent,
    NotificationsComponent,
    AdminSettingsComponent,
  ],
  imports: [
    AdminRoutingModule,
    SharedModule,
  ],
})
export class AdminModule {}
