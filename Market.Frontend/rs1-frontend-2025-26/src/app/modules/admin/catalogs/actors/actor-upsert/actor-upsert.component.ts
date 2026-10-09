import { Component, inject, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  CreateActorCommand,
  GetActorByIdQueryDto,
  UpdateActorCommand,
} from '../../../../../api-services/actors/actors-api.model';
import { ActorsApiService } from '../../../../../api-services/actors/actors-api.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { ActorFormService } from '../services/actor-form.service';

@Component({
  selector: 'app-actor-upsert',
  standalone: false,
  templateUrl: './actor-upsert.component.html',
  styleUrl: './actor-upsert.component.scss',
  providers: [ActorFormService],
})
export class ActorUpsertComponent extends BaseFormComponent<GetActorByIdQueryDto> implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private api = inject(ActorsApiService);
  private formService = inject(ActorFormService);
  private toaster = inject(ToasterService);

  override form: FormGroup = this.formService.createForm();
  private actorId?: number;

  get title(): string {
    return this.isEditMode ? 'Edit Actor' : 'Create Actor';
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.actorId = idParam ? Number(idParam) : undefined;

    this.form = this.formService.createForm();
    this.initForm(Boolean(this.actorId));
  }

  protected loadData(): void {
    if (!this.actorId) {
      return;
    }

    this.startLoading();
    this.api.getById(this.actorId).subscribe({
      next: (result) => {
        this.model = result;
        this.form = this.formService.createForm(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load actor.');
        console.error('Get actor by id error:', err);
      },
    });
  }

  protected save(): void {
    this.startLoading();

    const firstName = String(this.form.value.firstName ?? '').trim();
    const lastName = String(this.form.value.lastName ?? '').trim();
    const photoBase64 = String(this.form.value.photoBase64 ?? '').trim();
    const birthDate = new Date(this.form.value.birthDate).toISOString();
    const countryId = Number(this.form.value.countryId);
    const imdbLink = String(this.form.value.imdbLink ?? '').trim();
    const biography = String(this.form.value.biography ?? '').trim();

    if (this.isEditMode && this.actorId) {
      const payload: UpdateActorCommand = {
        firstName,
        lastName,
        photoBase64,
        birthDate,
        countryId,
        imdbLink,
        biography,
      };

      this.api.update(this.actorId, payload).subscribe({
        next: () => {
          this.toaster.success('Actor updated successfully.');
          this.stopLoading();
          this.router.navigate(['/admin/actors']);
        },
        error: (err) => {
          this.stopLoading('Failed to update actor.');
          console.error('Update actor error:', err);
        },
      });
      return;
    }

    const payload: CreateActorCommand = {
      firstName,
      lastName,
      photoBase64,
      birthDate,
      countryId,
      imdbLink,
      biography,
    };

    this.api.create(payload).subscribe({
      next: () => {
        this.toaster.success('Actor created successfully.');
        this.stopLoading();
        this.router.navigate(['/admin/actors']);
      },
      error: (err) => {
        this.stopLoading('Failed to create actor.');
        console.error('Create actor error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/actors']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}

