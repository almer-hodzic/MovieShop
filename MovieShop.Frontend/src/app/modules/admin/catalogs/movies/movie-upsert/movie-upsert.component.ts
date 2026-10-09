import { Component, inject, OnInit } from '@angular/core';
import { FormArray, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ActorsApiService } from '../../../../../api-services/actors/actors-api.service';
import { ListActorsQueryDto, ListActorsRequest } from '../../../../../api-services/actors/actors-api.model';
import {
  ListCategoriesQueryDto,
  ListCategoriesRequest,
} from '../../../../../api-services/categories/categories-api.model';
import { CategoriesApiService } from '../../../../../api-services/categories/categories-api.service';
import {
  ListDirectorsQueryDto,
  ListDirectorsRequest,
} from '../../../../../api-services/directors/directors-api.model';
import { DirectorsApiService } from '../../../../../api-services/directors/directors-api.service';
import {
  CreateMovieActorItem,
  CreateMovieCommand,
  GetMovieByIdQueryDto,
  UpdateMovieCommand,
} from '../../../../../api-services/movies/movies-api.model';
import { MoviesApiService } from '../../../../../api-services/movies/movies-api.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { MovieFormService } from '../services/movie-form.service';

@Component({
  selector: 'app-movie-upsert',
  standalone: false,
  templateUrl: './movie-upsert.component.html',
  styleUrl: './movie-upsert.component.scss',
  providers: [MovieFormService],
})
export class MovieUpsertComponent extends BaseFormComponent<GetMovieByIdQueryDto> implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private moviesApi = inject(MoviesApiService);
  private directorsApi = inject(DirectorsApiService);
  private categoriesApi = inject(CategoriesApiService);
  private actorsApi = inject(ActorsApiService);
  private formService = inject(MovieFormService);
  private toaster = inject(ToasterService);

  override form: FormGroup = this.formService.createForm();
  private movieId?: number;

  directorOptions: ListDirectorsQueryDto[] = [];
  categoryOptions: ListCategoriesQueryDto[] = [];
  actorOptions: ListActorsQueryDto[] = [];
  isLookupsLoading = false;

  get title(): string {
    return this.isEditMode ? 'Edit Movie' : 'Create Movie';
  }

  get actorsArray(): FormArray {
    return this.formService.getActorsArray(this.form);
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    this.movieId = idParam ? Number(idParam) : undefined;

    this.form = this.formService.createForm();
    this.loadLookups();
    this.initForm(Boolean(this.movieId));
  }

  protected loadData(): void {
    if (!this.movieId) {
      return;
    }

    this.startLoading();
    this.moviesApi.getById(this.movieId).subscribe({
      next: (result) => {
        this.model = result;
        this.form = this.formService.createForm(result);
        this.stopLoading();
      },
      error: (err) => {
        this.stopLoading('Failed to load movie.');
        console.error('Get movie by id error:', err);
      },
    });
  }

  protected save(): void {
    this.startLoading();

    const payload = this.buildPayload();

    if (this.isEditMode && this.movieId) {
      const updatePayload: UpdateMovieCommand = { ...payload };
      this.moviesApi.update(this.movieId, updatePayload).subscribe({
        next: () => {
          this.toaster.success('Movie updated successfully.');
          this.stopLoading();
          this.router.navigate(['/admin/movies']);
        },
        error: (err) => {
          this.stopLoading('Failed to update movie.');
          console.error('Update movie error:', err);
        },
      });
      return;
    }

    const createPayload: CreateMovieCommand = { ...payload };
    this.moviesApi.create(createPayload).subscribe({
      next: () => {
        this.toaster.success('Movie created successfully.');
        this.stopLoading();
        this.router.navigate(['/admin/movies']);
      },
      error: (err) => {
        this.stopLoading('Failed to create movie.');
        console.error('Create movie error:', err);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/movies']);
  }

  addActor(): void {
    this.formService.addActor(this.form);
  }

  removeActor(index: number): void {
    this.formService.removeActor(this.form, index);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }

  getActorsArrayError(): string {
    return this.formService.getActorsArrayError(this.form);
  }

  actorDisplayName(actor: ListActorsQueryDto): string {
    return `${actor.firstName} ${actor.lastName}`.trim();
  }

  private loadLookups(): void {
    this.isLookupsLoading = true;

    const directorsRequest = new ListDirectorsRequest();
    directorsRequest.paging.pageSize = 1000;

    const categoriesRequest = new ListCategoriesRequest();
    categoriesRequest.paging.pageSize = 1000;

    const actorsRequest = new ListActorsRequest();
    actorsRequest.paging.pageSize = 1000;

    forkJoin({
      directors: this.directorsApi.list(directorsRequest),
      categories: this.categoriesApi.list(categoriesRequest),
      actors: this.actorsApi.list(actorsRequest),
    }).subscribe({
      next: ({ directors, categories, actors }) => {
        this.directorOptions = directors.items;
        this.categoryOptions = categories.items;
        this.actorOptions = actors.items;
        this.isLookupsLoading = false;
      },
      error: (err) => {
        this.isLookupsLoading = false;
        this.errorMessage = 'Failed to load lookup data.';
        console.error('Load movie lookups error:', err);
      },
    });
  }

  private buildPayload(): CreateMovieCommand {
    const releaseDateValue = this.form.get('releaseDate')?.value;
    const selectedCategories = (this.form.get('categories')?.value ?? []) as number[];

    const actors: CreateMovieActorItem[] = this.actorsArray.controls.map((x) => ({
      actorId: Number(x.get('actorId')?.value),
      characterName: String(x.get('characterName')?.value ?? '').trim(),
    }));

    return {
      title: String(this.form.get('title')?.value ?? '').trim(),
      releaseDate: new Date(releaseDateValue).toISOString(),
      duration: Number(this.form.get('duration')?.value),
      directorId: Number(this.form.get('directorId')?.value),
      categories: selectedCategories.map((x) => Number(x)),
      actors,
      countryId: Number(this.form.get('countryId')?.value),
      trailerLink: String(this.form.get('trailerLink')?.value ?? '').trim(),
      imageBase64: String(this.form.get('imageBase64')?.value ?? '').trim(),
      storyLine: String(this.form.get('storyLine')?.value ?? '').trim(),
      price: Number(this.form.get('price')?.value),
    };
  }
}
