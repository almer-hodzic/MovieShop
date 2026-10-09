import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListDirectorsRequest extends BasePagedQuery {
  name?: string | null;
}

export interface ListDirectorsQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  birthDate: string;
}

export interface GetDirectorByIdQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  birthDate: string;
}

export type ListDirectorsResponse = PageResult<ListDirectorsQueryDto>;

export interface CreateDirectorCommand {
  firstName: string;
  lastName: string;
  birthDate: string;
}

export interface UpdateDirectorCommand {
  firstName: string;
  lastName: string;
  birthDate: string;
}

