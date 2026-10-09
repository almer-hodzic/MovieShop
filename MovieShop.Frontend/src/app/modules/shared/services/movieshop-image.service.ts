import { Injectable } from '@angular/core';

type ImageValue = string | null | undefined;

export interface MovieImageSource {
  id?: number | null;
  movieId?: number | null;
  title?: string | null;
  movieTitle?: string | null;
  image?: ImageValue;
  movieImage?: ImageValue;
  posterImgUrl?: ImageValue;
}

export interface ActorImageSource {
  id?: number | null;
  actorId?: number | null;
  firstName?: string | null;
  lastName?: string | null;
  photo?: ImageValue;
  actorPhoto?: ImageValue;
  image?: ImageValue;
}

export interface UserImageSource {
  profileImage?: ImageValue;
  image?: ImageValue;
  photo?: ImageValue;
  avatar?: ImageValue;
}

@Injectable({ providedIn: 'root' })
export class MovieShopImageService {
  readonly movieFallback = 'https://cdn.browshot.com/static/images/not-found.png';
  readonly actorFallback = '/assets/img/register-avatar.png';
  readonly userFallback = '/assets/img/register-avatar.png';

  private readonly legacyMovieImagesByTitle: Record<string, string> = {
    [this.normalizeKey('The Lord of the Rings: The Fellowship of the Ring')]:
      '/assets/img/movieshop/details/movie-the-lord-of-the-rings-the-fellowship-of-the-ring.jpg',
    [this.normalizeKey('The Departed')]: '/assets/img/movieshop/details/movie-the-departed.jpg',
    [this.normalizeKey('Poor Things')]: '/assets/img/movieshop/details/movie-poor-things.jpg',
  };

  private readonly legacyActorImagesByName: Record<string, string> = {
    [this.normalizeKey('Emma Stone')]: '/assets/img/movieshop/details/actor-emma-stone.jpg',
    [this.normalizeKey('Willem Dafoe')]: '/assets/img/movieshop/details/actor-willem-dafoe.jpg',
    [this.normalizeKey('Mark Ruffalo')]: '/assets/img/movieshop/details/actor-mark-ruffalo.jpg',
    [this.normalizeKey('Ramy Youssef')]: '/assets/img/movieshop/details/actor-ramy-youssef.jpg',
    [this.normalizeKey('Christopher Abbott')]: '/assets/img/movieshop/details/actor-christopher-abbott.jpg',
    [this.normalizeKey('Jerrod Carmichael')]: '/assets/img/movieshop/details/actor-jerrod-carmichael.jpg',
    [this.normalizeKey('Margaret Qualley')]: '/assets/img/movieshop/details/actor-margaret-qualley.jpg',
  };

  resolveMovieImage(movie: MovieImageSource | ImageValue, title?: string | null): string {
    const source = typeof movie === 'string' || movie == null ? { image: movie, title } : movie;
    const image = this.normalizeBackendImage(source.image ?? source.movieImage ?? source.posterImgUrl, 'movie');
    if (image) return image;

    return this.resolveLegacyMovieImage(source) ?? this.movieFallback;
  }

  resolveActorImage(actor: ActorImageSource | ImageValue, firstName?: string | null, lastName?: string | null): string {
    const source = typeof actor === 'string' || actor == null ? { photo: actor, firstName, lastName } : actor;
    const image = this.normalizeBackendImage(source.photo ?? source.actorPhoto ?? source.image, 'actor');
    if (image) return image;

    return this.resolveLegacyActorImage(source) ?? this.actorFallback;
  }

  resolveUserImage(user: UserImageSource | ImageValue): string {
    const source = typeof user === 'string' || user == null ? { profileImage: user } : user;
    const image = this.normalizeBackendImage(source.profileImage ?? source.image ?? source.photo ?? source.avatar, 'user');
    return image ?? this.userFallback;
  }

  resolveLegacyMovieImage(movie: MovieImageSource): string | null {
    const title = this.normalizeKey(movie.title ?? movie.movieTitle);
    return title ? this.legacyMovieImagesByTitle[title] ?? null : null;
  }

  useMovieFallback(event: Event): void {
    this.swapImageSource(event, this.movieFallback);
  }

  useActorFallback(event: Event): void {
    this.swapImageSource(event, this.actorFallback);
  }

  useUserFallback(event: Event): void {
    this.swapImageSource(event, this.userFallback);
  }

  private resolveLegacyActorImage(actor: ActorImageSource): string | null {
    const name = this.normalizeKey(`${actor.firstName ?? ''} ${actor.lastName ?? ''}`);
    return name ? this.legacyActorImagesByName[name] ?? null : null;
  }

  private normalizeBackendImage(value: ImageValue, entity: 'movie' | 'actor' | 'user'): string | null {
    const image = value?.trim();
    if (!image || this.isWrongSemanticFallback(image, entity)) return null;
    if (image.startsWith('http') || image.startsWith('data:image') || image.startsWith('/assets/')) return image;
    if (image.startsWith('./assets/')) return image.slice(1);
    if (image.startsWith('assets/')) return `/${image}`;
    return `data:image/jpeg;base64,${image}`;
  }

  private isWrongSemanticFallback(image: string, entity: 'movie' | 'actor' | 'user'): boolean {
    const normalized = image.toLowerCase();
    if (entity === 'movie' && normalized.includes('register-avatar')) return true;
    return normalized.includes('not-found');
  }

  private normalizeKey(value: string | null | undefined): string {
    return (value ?? '').trim().toLowerCase();
  }

  private swapImageSource(event: Event, fallback: string): void {
    const image = event.target as HTMLImageElement;
    if (!image || image.src === fallback || image.src.endsWith(fallback)) return;
    image.src = fallback;
  }
}
