import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { ToasterService } from '../../../../core/services/toaster.service';
import { ActorsApiService } from '../../../../api-services/actors/actors-api.service';
import {
  ListActorsQueryDto,
  ListActorsRequest,
} from '../../../../api-services/actors/actors-api.model';
import { MovieShopConfirmService } from '../../../shared/services/movieshop-confirm.service';
import { MovieShopImageService } from '../../../shared/services/movieshop-image.service';

@Component({
  selector: 'app-actors',
  standalone: false,
  templateUrl: './actors.component.html',
  styleUrl: './actors.component.scss',
})
export class ActorsComponent
  extends BaseListPagedComponent<ListActorsQueryDto, ListActorsRequest>
  implements OnInit
{
  private api = inject(ActorsApiService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private confirm = inject(MovieShopConfirmService);
  readonly imageResolver = inject(MovieShopImageService);

  displayedColumns: string[] = ['id', 'firstName', 'lastName', 'birthDate', 'countryId', 'actions'];

  constructor() {
    super();
    this.request = new ListActorsRequest();
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
        this.stopLoading('Failed to load actors.');
        console.error('Load actors error:', err);
      },
    });
  }

  onSearch(value: string): void {
    this.request.name = value;
    this.request.paging.page = 1;
    this.loadPagedData();
  }

  onCreate(): void {
    this.router.navigate(['/admin/actors/new']);
  }

  onEdit(item: ListActorsQueryDto): void {
    this.router.navigate(['/admin/actors', item.id, 'edit']);
  }

  onDelete(item: ListActorsQueryDto): void {
    const fullName = `${item.firstName} ${item.lastName}`.trim();
    this.confirm.confirm({
      title: 'Delete Actor',
      message: `Delete actor "${fullName}"?`,
      confirmText: 'Delete',
      tone: 'danger',
    }).subscribe((confirmed) => {
      if (!confirmed) return;

      this.startLoading();
      this.api.delete(item.id).subscribe({
        next: () => {
          this.toaster.success('Actor deleted successfully.');
          this.loadPagedData();
        },
        error: (err) => {
          this.stopLoading('Failed to delete actor.');
          console.error('Delete actor error:', err);
        },
      });
    });
  }

  getActorProfileImage(actor: ListActorsQueryDto): string {
    return this.imageResolver.resolveActorImage(actor);
  }
}
