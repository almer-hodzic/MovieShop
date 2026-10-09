import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoriesApiService } from '../../../../api-services/categories/categories-api.service';
import {
  ListCategoriesQueryDto,
  ListCategoriesRequest,
} from '../../../../api-services/categories/categories-api.model';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';

@Component({
  selector: 'app-categories',
  standalone: false,
  templateUrl: './categories.component.html',
  styleUrl: './categories.component.scss',
})
export class CategoriesComponent
  extends BaseListPagedComponent<ListCategoriesQueryDto, ListCategoriesRequest>
  implements OnInit
{
  private api = inject(CategoriesApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);

  displayedColumns: string[] = ['id', 'categoryName', 'actions'];

  constructor() {
    super();
    this.request = new ListCategoriesRequest();
  }

  ngOnInit(): void {
    this.initList();
  }

  protected loadPagedData(): void {
    this.startLoading();

    this.api.list(this.request).subscribe({
      next: (result) => {
        this.handlePageResult(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load categories.');
        console.error('Load categories error:', err);
      },
    });
  }

  onSearch(value: string): void {
    this.request.categoryName = value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.router.navigate(['/admin/categories/new']);
  }

  onEdit(item: ListCategoriesQueryDto): void {
    this.router.navigate(['/admin/categories', item.id, 'edit']);
  }

  onDelete(item: ListCategoriesQueryDto): void {
    this.confirm.confirm({
      title: 'Delete Category',
      message: `Delete category "${item.categoryName}"?`,
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.delete(item.id).subscribe({
        next: () => {
          this.toaster.success('Category deleted successfully.');
          this.loadPagedData();
        },
        error: (err) => {
          this.stopLoading('Failed to delete category.');
          console.error('Delete category error:', err);
        },
      });
    });
  }
}
