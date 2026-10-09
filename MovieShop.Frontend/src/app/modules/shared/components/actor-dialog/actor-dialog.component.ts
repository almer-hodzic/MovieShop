import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { GetActorByIdQueryDto } from '../../../../api-services/actors/actors-api.model';
import { MovieShopImageService } from '../../services/movieshop-image.service';

export interface MovieShopActorDialogData extends GetActorByIdQueryDto {
  characterName?: string;
}

@Component({
  selector: 'app-actor-dialog',
  standalone: false,
  templateUrl: './actor-dialog.component.html',
  styleUrl: './actor-dialog.component.scss',
})
export class ActorDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA) public data: MovieShopActorDialogData,
    readonly imageResolver: MovieShopImageService,
  ) {}

  getActorProfileImage(profileImage: string | null | undefined): string {
    return this.imageResolver.resolveActorImage({
      ...this.data,
      photo: profileImage,
    });
  }

  get actorName(): string {
    return `${this.data.firstName} ${this.data.lastName}`.trim();
  }

  get imdbUrl(): string {
    if (!this.data.imdbLink) return '';
    if (this.data.imdbLink.startsWith('http')) return this.data.imdbLink;
    return `https://www.imdb.com/name/${this.data.imdbLink}`;
  }
}
