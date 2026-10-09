import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListReviewsByMovieRequest extends BasePagedQuery {
  movieId!: number;
  userName?: string | null;
}

export interface GetReviewsByMovieIdQueryDto {
  id: number;
  reviewDate: string;
  score: number;
  comment: string;
  userId: number;
  movieId: number;
  userName: string;
}

export interface GetReviewByIdQueryDto {
  id: number;
  reviewDate: string;
  score: number;
  comment: string;
  userId: number;
  movieId: number;
  userName: string;
}

export interface CreateReviewCommand {
  movieId: number;
  comment: string;
  score: number;
}

export interface DeleteReviewCommand {
  id: number;
}

export type ListReviewsByMovieResponse = PageResult<GetReviewsByMovieIdQueryDto>;
