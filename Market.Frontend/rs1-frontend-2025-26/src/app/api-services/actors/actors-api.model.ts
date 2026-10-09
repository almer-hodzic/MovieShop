import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListActorsRequest extends BasePagedQuery {
  name?: string | null;
}

export interface ListActorsQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  photo?: string | null;
  birthDate: string;
  countryId: number;
  imdbLink: string;
  biography: string;
}

export interface GetActorByIdQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  photo?: string | null;
  birthDate: string;
  countryId: number;
  imdbLink: string;
  biography: string;
}

export type ListActorsResponse = PageResult<ListActorsQueryDto>;

export interface CreateActorCommand {
  firstName: string;
  lastName: string;
  photoBase64: string;
  birthDate: string;
  countryId: number;
  imdbLink: string;
  biography: string;
}

export interface UpdateActorCommand {
  firstName: string;
  lastName: string;
  photoBase64: string;
  birthDate: string;
  countryId: number;
  imdbLink: string;
  biography: string;
}

