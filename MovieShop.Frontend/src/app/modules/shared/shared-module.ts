import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {materialModules} from './material-modules';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { PosterComponent } from './components/poster/poster.component';
import { StarComponent } from './components/star/star.component';
import { StarRatingComponent } from './components/star-rating/star-rating.component';
import { SearchComponent } from './components/search/search.component';
import { SelectInputComponent } from './components/select-input/select-input.component';
import { CategoryInputComponent } from './components/category-input/category-input.component';
import { SpinnerComponent } from './components/spinner/spinner.component';
import { ScrollTopComponent } from './components/scroll-top/scroll-top.component';
import { ActorDialogComponent } from './components/actor-dialog/actor-dialog.component';
import { MovieShopConfirmDialogComponent } from './components/movieshop-confirm-dialog/movieshop-confirm-dialog.component';
import { MovieshopMenuComponent } from '../public/movieshop-menu/movieshop-menu.component';
import { MovieShopNotificationDialogComponent } from '../public/notification-dialog/movieshop-notification-dialog.component';
import { RouterModule } from '@angular/router';



@NgModule({
  declarations: [
    PosterComponent,
    StarComponent,
    StarRatingComponent,
    SearchComponent,
    SelectInputComponent,
    CategoryInputComponent,
    SpinnerComponent,
    ScrollTopComponent,
    ActorDialogComponent,
    MovieShopConfirmDialogComponent,
    MovieshopMenuComponent,
    MovieShopNotificationDialogComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    ...materialModules
  ],
  exports:[
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    materialModules,
    PosterComponent,
    StarComponent,
    StarRatingComponent,
    SearchComponent,
    SelectInputComponent,
    CategoryInputComponent,
    SpinnerComponent,
    ScrollTopComponent,
    ActorDialogComponent,
    MovieShopConfirmDialogComponent,
    MovieshopMenuComponent,
    MovieShopNotificationDialogComponent
  ]
})
export class SharedModule { }
