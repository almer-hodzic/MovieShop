import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { myAuthGuard } from '../../core/guards/my-auth-guard';

import { PublicLayoutComponent } from './public-layout/public-layout.component';
import { WelcomeComponent } from './welcome/welcome.component';
import { MoviesBrowseComponent } from './movies/movies-browse/movies-browse.component';
import { MovieDetailsComponent } from './movies/movie-details/movie-details.component';
import { MovieListFavouritesComponent } from './movies/movie-list-favourites/movie-list-favourites.component';
import { ClientShoppingCartComponent } from './shopping-cart/client-shopping-cart.component';
import { ClientCheckoutComponent } from './shopping-cart/checkout/client-checkout.component';
import { ClientUserProfileComponent } from './user-profile/client-user-profile.component';
import { ClientVideoPlayerComponent } from './player/client-video-player.component';

const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      {
        path: 'welcome',
        component: WelcomeComponent,
      },
      {
        path: 'movies/Favourites',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: MovieListFavouritesComponent,
      },
      {
        path: 'movies/:id',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: MovieDetailsComponent,
      },
      {
        path: 'movies',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: MoviesBrowseComponent,
      },
      {
        path: 'shopping-cart/checkout',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: ClientCheckoutComponent,
      },
      {
        path: 'shopping-cart',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: ClientShoppingCartComponent,
      },
      {
        path: 'user',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: ClientUserProfileComponent,
      },
      {
        path: 'play/:ytId',
        canActivate: [myAuthGuard],
        data: { requireAuth: true },
        component: ClientVideoPlayerComponent,
      },
      {
        path: '',
        redirectTo: 'welcome',
        pathMatch: 'full',
      },
      { path: '**', redirectTo: 'welcome' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class PublicRoutingModule {}
