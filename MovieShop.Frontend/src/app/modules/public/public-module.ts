import { NgModule } from '@angular/core';
import { NgxSliderModule } from '@angular-slider/ngx-slider';

import { PublicRoutingModule } from './public-routing-module';
import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { SharedModule } from '../shared/shared-module';
import { WelcomeComponent } from './welcome/welcome.component';
import { MoviesBrowseComponent } from './movies/movies-browse/movies-browse.component';
import { MoviesListViewComponent } from './movies/movies-list-view/movies-list-view.component';
import { MoviesGridViewComponent } from './movies/movies-grid-view/movies-grid-view.component';
import { MovieListCardComponent } from './movies/movie-list-card/movie-list-card.component';
import { MovieDetailsComponent } from './movies/movie-details/movie-details.component';
import { MovieListFavouritesComponent } from './movies/movie-list-favourites/movie-list-favourites.component';
import { ClientShoppingCartComponent } from './shopping-cart/client-shopping-cart.component';
import { ClientCheckoutComponent } from './shopping-cart/checkout/client-checkout.component';
import { ClientUserProfileComponent } from './user-profile/client-user-profile.component';
import { ClientVideoPlayerComponent } from './player/client-video-player.component';

@NgModule({
  declarations: [
    PublicLayoutComponent,
    WelcomeComponent,
    MoviesBrowseComponent,
    MoviesListViewComponent,
    MoviesGridViewComponent,
    MovieListCardComponent,
    MovieDetailsComponent,
    MovieListFavouritesComponent,
    ClientShoppingCartComponent,
    ClientCheckoutComponent,
    ClientUserProfileComponent,
    ClientVideoPlayerComponent,
  ],
  imports: [
    SharedModule,
    NgxSliderModule,
    PublicRoutingModule,
  ],
})
export class PublicModule {}
