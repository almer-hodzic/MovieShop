import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListMoviesRequest extends BasePagedQuery {
  title?: string | null;
  categoryId?: number | null;
  priceFrom?: number | null;
  priceTo?: number | null;
  sortedColumn?: string | null;
  isDescending?: boolean | null;
}

export interface ListMoviesCategoryDto {
  categoryId: number;
  categoryName: string;
}

export interface ListMoviesActorDto {
  actorId: number;
  firstName: string;
  lastName: string;
  characterName: string;
}

export interface ListMoviesQueryDto {
  id: number;
  title: string;
  releaseDate: string;
  creationDate: string;
  duration: number;
  directorId: number;
  directorName: string;
  countryId: number;
  trailerLink?: string | null;
  image?: string | null;
  storyLine: string;
  price: number;
  averageScore?: number | null;
  categories: ListMoviesCategoryDto[];
  actors: ListMoviesActorDto[];
}

export interface GetMovieByIdCategoryDto {
  categoryId: number;
  categoryName: string;
}

export interface GetMovieByIdActorDto {
  actorId: number;
  firstName: string;
  lastName: string;
  characterName: string;
}

export interface GetMovieByIdQueryDto {
  id: number;
  title: string;
  releaseDate: string;
  creationDate: string;
  duration: number;
  directorId: number;
  directorName: string;
  countryId: number;
  trailerLink?: string | null;
  image?: string | null;
  storyLine: string;
  price: number;
  averageScore?: number | null;
  categories: GetMovieByIdCategoryDto[];
  actors: GetMovieByIdActorDto[];
}

export interface CreateMovieActorItem {
  actorId: number;
  characterName: string;
}

export interface CreateMovieCommand {
  title: string;
  releaseDate: string;
  duration: number;
  directorId: number;
  categories: number[];
  actors: CreateMovieActorItem[];
  countryId: number;
  trailerLink: string;
  imageBase64: string;
  storyLine: string;
  price: number;
}

export interface UpdateMovieActorItem {
  actorId: number;
  characterName: string;
}

export interface UpdateMovieCommand {
  title: string;
  releaseDate: string;
  duration: number;
  directorId: number;
  categories: number[];
  actors: UpdateMovieActorItem[];
  countryId: number;
  trailerLink: string;
  imageBase64: string;
  storyLine: string;
  price: number;
}

export type ListMoviesResponse = PageResult<ListMoviesQueryDto>;
