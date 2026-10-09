import { Component, Input, inject } from '@angular/core';
import { MovieShopImageService } from '../../services/movieshop-image.service';

@Component({
  selector: 'app-poster',
  standalone: false,
  templateUrl: './poster.component.html',
  styleUrl: './poster.component.scss',
})
export class PosterComponent {
  readonly imageResolver = inject(MovieShopImageService);

  @Input() posterImgUrl: string | null | undefined;
  @Input() fallbackImgUrl: string | null | undefined;
  @Input() alt = '';

  get resolvedPosterImgUrl(): string {
    return this.imageResolver.resolveMovieImage({
      image: this.posterImgUrl,
      posterImgUrl: this.fallbackImgUrl,
      title: this.alt,
    });
  }

  useFallback(event: Event): void {
    const image = event.target as HTMLImageElement;
    const fallback = this.fallbackImgUrl || this.imageResolver.movieFallback;
    if (image.src !== fallback && !image.src.endsWith(fallback)) image.src = fallback;
  }
}
