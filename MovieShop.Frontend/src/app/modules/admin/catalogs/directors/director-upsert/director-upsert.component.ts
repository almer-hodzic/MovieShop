import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CreateDirectorCommand,
  GetDirectorByIdQueryDto,
  UpdateDirectorCommand,
} from '../../../../../api-services/directors/directors-api.model';
import { DirectorsApiService } from '../../../../../api-services/directors/directors-api.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { DirectorFormService } from '../services/director-form.service';

@Component({
  selector: 'app-director-upsert',
  standalone: false,
  templateUrl: './director-upsert.component.html',
  styleUrl: './director-upsert.component.scss',
  providers: [DirectorFormService],
})
export class DirectorUpsertComponent extends BaseFormComponent<GetDirectorByIdQueryDto> implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(DirectorsApiService);
  private formService = inject(DirectorFormService);
  private toaster = inject(ToasterService);

  override form: FormGroup = this.formService.createForm();
  private directorId?: number;

  get title(): string {
    return this.isEditMode ? 'Edit Director' : 'Create Director';
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.directorId = idParam ? Number(idParam) : undefined;

    this.form = this.formService.createForm();
    this.initForm(Boolean(this.directorId));
  }

  protected loadData(): void {
    if (!this.directorId) {
      return;
    }

    this.startLoading();
    this.api.getById(this.directorId).subscribe({
      next: (result) => {
        this.model = result;
        this.form = this.formService.createForm(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load director.');
        console.error('Get director by id error:', err);
      },
    });
  }

  protected save(): void {
    this.startLoading();

    const firstName = String(this.form.value.firstName ?? '').trim();
    const lastName = String(this.form.value.lastName ?? '').trim();
    const birthDate = new Date(this.form.value.birthDate);
    const birthDateIso = birthDate.toISOString();

    if (this.isEditMode && this.directorId) {
      const payload: UpdateDirectorCommand = { firstName, lastName, birthDate: birthDateIso };
      this.api.update(this.directorId, payload).subscribe({
        next: () => {
          this.toaster.success('Director updated successfully.');
          this.stopLoading();
          this.router.navigate(['/admin/directors']);
        },
        error: (err) => {
          this.stopLoading('Failed to update director.');
          console.error('Update director error:', err);
        },
      });
      return;
    }

    const payload: CreateDirectorCommand = { firstName, lastName, birthDate: birthDateIso };
    this.api.create(payload).subscribe({
      next: () => {
        this.toaster.success('Director created successfully.');
        this.stopLoading();
        this.router.navigate(['/admin/directors']);
      },
      error: (err) => {
        this.stopLoading('Failed to create director.');
        console.error('Create director error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/directors']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}

