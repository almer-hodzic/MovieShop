import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DirectorsApiService } from '../../../../api-services/directors/directors-api.service';
import {
  ListDirectorsQueryDto,
  ListDirectorsRequest,
} from '../../../../api-services/directors/directors-api.model';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';

@Component({
  selector: 'app-directors',
  standalone: false,
  templateUrl: './directors.component.html',
  styleUrl: './directors.component.scss',
})
export class DirectorsComponent
  extends BaseListPagedComponent<ListDirectorsQueryDto, ListDirectorsRequest>
  implements OnInit
{
  private api = inject(DirectorsApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);

  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'birthDate', 'actions'];

  constructor() {
    super();
    this.request = new ListDirectorsRequest();
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
        this.stopLoading('Failed to load directors.');
        console.error('Load directors error:', err);
      },
    });
  }

  onSearch(value: string): void {
    this.request.name = value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.router.navigate(['/admin/directors/new']);
  }

  onEdit(item: ListDirectorsQueryDto): void {
    this.router.navigate(['/admin/directors', item.id, 'edit']);
  }

  onDelete(item: ListDirectorsQueryDto): void {
    const fullName = `${item.firstName} ${item.lastName}`.trim();
    this.confirm.confirm({
      title: 'Delete Director',
      message: `Delete director "${fullName}"?`,
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.delete(item.id).subscribe({
        next: () => {
          this.toaster.success('Director deleted successfully.');
          this.loadPagedData();
        },
        error: (err) => {
          this.stopLoading('Failed to delete director.');
          console.error('Delete director error:', err);
        },
      });
    });
  }
}
