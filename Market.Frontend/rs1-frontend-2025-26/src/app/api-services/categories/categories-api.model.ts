import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListCategoriesRequest extends BasePagedQuery {
  categoryName?: string | null;
}

export interface ListCategoriesQueryDto {
  id: number;
  categoryName: string;
}

export interface GetCategoryByIdQueryDto {
  id: number;
  categoryName: string;
}

export type ListCategoriesResponse = PageResult<ListCategoriesQueryDto>;

export interface CreateCategoryCommand {
  categoryName: string;
}

export interface UpdateCategoryCommand {
  categoryName: string;
}
