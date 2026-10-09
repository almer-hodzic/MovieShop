import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable, map } from 'rxjs';
import {
  MovieShopConfirmDialogComponent,
  MovieShopConfirmDialogData,
} from '../components/movieshop-confirm-dialog/movieshop-confirm-dialog.component';

@Injectable({
  providedIn: 'root',
})
export class MovieShopConfirmService {
  private dialog = inject(MatDialog);

  confirm(data: MovieShopConfirmDialogData): Observable<boolean> {
    return this.dialog
      .open<MovieShopConfirmDialogComponent, MovieShopConfirmDialogData, boolean>(
        MovieShopConfirmDialogComponent,
        {
          data,
          autoFocus: 'dialog',
          restoreFocus: true,
          panelClass: 'movieshop-confirm-panel',
        },
      )
      .afterClosed()
      .pipe(map(Boolean));
  }
}
