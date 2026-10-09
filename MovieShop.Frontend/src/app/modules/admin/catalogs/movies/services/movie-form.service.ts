import { inject, Injectable } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import {
  CreateMovieActorItem,
  GetMovieByIdQueryDto,
} from '../../../../../api-services/movies/movies-api.model';

@Injectable()
export class MovieFormService {
  private fb = inject(FormBuilder);

  createForm(model?: Partial<GetMovieByIdQueryDto>): FormGroup {
    const selectedCategories = model?.categories?.map((x) => x.categoryId) ?? [];
    const actorGroups = model?.actors?.length
      ? model.actors.map((x) => this.createActorGroup({
          actorId: x.actorId,
          characterName: x.characterName,
        }))
      : [this.createActorGroup()];

    return this.fb.group({
      title: [
        model?.title ?? '',
        [Validators.required, Validators.maxLength(200)],
      ],
      releaseDate: [
        model?.releaseDate ? new Date(model.releaseDate) : null,
        [Validators.required],
      ],
      duration: [
        model?.duration ?? null,
        [Validators.required, Validators.min(1)],
      ],
      directorId: [
        model?.directorId ?? null,
        [Validators.required, Validators.min(1)],
      ],
      categories: [
        selectedCategories,
        [Validators.required, this.nonEmptyArrayValidator],
      ],
      countryId: [
        model?.countryId ?? null,
        [Validators.required, Validators.min(1)],
      ],
      trailerLink: [
        model?.trailerLink ?? '',
        [Validators.required, Validators.maxLength(500)],
      ],
      imageBase64: [
        model?.image ?? '',
        [Validators.required],
      ],
      storyLine: [
        model?.storyLine ?? '',
        [Validators.required, Validators.maxLength(4000)],
      ],
      price: [
        model?.price ?? null,
        [Validators.required, Validators.min(0.01)],
      ],
      actors: this.fb.array(actorGroups, [this.minimumOneActorValidator, this.uniqueActorsValidator]),
    });
  }

  getActorsArray(form: FormGroup): FormArray {
    return form.get('actors') as FormArray;
  }

  addActor(form: FormGroup): void {
    const actorsArray = this.getActorsArray(form);
    actorsArray.push(this.createActorGroup());
    actorsArray.markAsTouched();
    actorsArray.updateValueAndValidity();
  }

  removeActor(form: FormGroup, index: number): void {
    const actorsArray = this.getActorsArray(form);
    if (actorsArray.length <= 1) {
      return;
    }

    actorsArray.removeAt(index);
    actorsArray.markAsTouched();
    actorsArray.updateValueAndValidity();
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.touched || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      if (controlName === 'title') return 'Title is required.';
      if (controlName === 'releaseDate') return 'Release date is required.';
      if (controlName === 'duration') return 'Duration is required.';
      if (controlName === 'directorId') return 'Director is required.';
      if (controlName === 'categories') return 'At least one category is required.';
      if (controlName === 'countryId') return 'Country id is required.';
      if (controlName === 'trailerLink') return 'Trailer link is required.';
      if (controlName === 'imageBase64') return 'Image is required.';
      if (controlName === 'storyLine') return 'Story line is required.';
      if (controlName === 'price') return 'Price is required.';
    }

    if (control.errors['maxlength']) {
      if (controlName === 'title') return 'Title can be at most 200 characters.';
      if (controlName === 'trailerLink') return 'Trailer link can be at most 500 characters.';
      if (controlName === 'storyLine') return 'Story line can be at most 4000 characters.';
    }

    if (control.errors['min']) {
      if (controlName === 'duration') return 'Duration must be greater than 0.';
      if (controlName === 'directorId') return 'Director is required.';
      if (controlName === 'countryId') return 'Country id must be greater than 0.';
      if (controlName === 'price') return 'Price must be greater than 0.';
    }

    if (control.errors['minItems'] && controlName === 'categories') {
      return 'At least one category is required.';
    }

    return 'Invalid value.';
  }

  getActorsArrayError(form: FormGroup): string {
    const actorsArray = this.getActorsArray(form);
    if ((!actorsArray.touched && !actorsArray.dirty) || !actorsArray.errors) {
      return '';
    }

    if (actorsArray.errors['minItems']) {
      return 'At least one actor is required.';
    }

    if (actorsArray.errors['duplicateActor']) {
      return 'Each actor can be selected only once.';
    }

    return 'Invalid actors list.';
  }

  private createActorGroup(model?: Partial<CreateMovieActorItem>): FormGroup {
    return this.fb.group({
      actorId: [
        model?.actorId ?? null,
        [Validators.required, Validators.min(1)],
      ],
      characterName: [
        model?.characterName ?? '',
        [Validators.required, Validators.maxLength(150)],
      ],
    });
  }

  private nonEmptyArrayValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    return Array.isArray(value) && value.length > 0 ? null : { minItems: true };
  }

  private minimumOneActorValidator(control: AbstractControl): ValidationErrors | null {
    const formArray = control as FormArray;
    return formArray.length > 0 ? null : { minItems: true };
  }

  private uniqueActorsValidator(control: AbstractControl): ValidationErrors | null {
    const formArray = control as FormArray;
    const actorIds = formArray.controls
      .map((x) => Number(x.get('actorId')?.value))
      .filter((x) => Number.isInteger(x) && x > 0);

    return new Set(actorIds).size === actorIds.length ? null : { duplicateActor: true };
  }
}
