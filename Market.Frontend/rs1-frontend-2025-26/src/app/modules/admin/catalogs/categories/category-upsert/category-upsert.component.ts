import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import { CategoriesApiService } from '../../../../../api-services/categories/categories-api.service';
import {
  CreateCategoryCommand,
  GetCategoryByIdQueryDto,
  UpdateCategoryCommand,
} from '../../../../../api-services/categories/categories-api.model';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { CategoryFormService } from '../services/category-form.service';

@Component({
  selector: 'app-category-upsert',
  standalone: false,
  templateUrl: './category-upsert.component.html',
  styleUrl: './category-upsert.component.scss',
  providers: [CategoryFormService],
})
export class CategoryUpsertComponent extends BaseFormComponent<GetCategoryByIdQueryDto> implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(CategoriesApiService);
  private formService = inject(CategoryFormService);
  private toaster = inject(ToasterService);

  override form: FormGroup = this.formService.createForm();
  private categoryId?: number;

  get title(): string {
    return this.isEditMode ? 'Edit Category' : 'Create Category';
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.categoryId = idParam ? Number(idParam) : undefined;

    this.form = this.formService.createForm();
    this.initForm(Boolean(this.categoryId));
  }

  protected loadData(): void {
    if (!this.categoryId) {
      return;
    }

    this.startLoading();
    this.api.getById(this.categoryId).subscribe({
      next: (result) => {
        this.model = result;
        this.form = this.formService.createForm(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load category.');
        console.error('Get category by id error:', err);
      },
    });
  }

  protected save(): void {
    this.startLoading();

    const categoryName = String(this.form.value.categoryName ?? '').trim();

    if (this.isEditMode && this.categoryId) {
      const payload: UpdateCategoryCommand = { categoryName };
      this.api.update(this.categoryId, payload).subscribe({
        next: () => {
          this.toaster.success('Category updated successfully.');
          this.stopLoading();
          this.router.navigate(['/admin/categories']);
        },
        error: (err) => {
          this.stopLoading('Failed to update category.');
          console.error('Update category error:', err);
        },
      });
      return;
    }

    const payload: CreateCategoryCommand = { categoryName };
    this.api.create(payload).subscribe({
      next: () => {
        this.toaster.success('Category created successfully.');
        this.stopLoading();
        this.router.navigate(['/admin/categories']);
      },
      error: (err) => {
        this.stopLoading('Failed to create category.');
        console.error('Create category error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/categories']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
