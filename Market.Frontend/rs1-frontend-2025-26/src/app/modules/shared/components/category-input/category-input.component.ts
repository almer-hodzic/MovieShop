import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { distinctUntilChanged } from 'rxjs/operators';

export interface MovieShopCategoryOption {
  id: number;
  categoryName: string;
}

@Component({
  selector: 'app-category-input',
  standalone: false,
  templateUrl: './category-input.component.html',
  styleUrl: './category-input.component.scss',
})
export class CategoryInputComponent implements OnInit, OnDestroy {
  readonly selectControl = new FormControl<number | null>(null);
  @Input() options: MovieShopCategoryOption[] = [];
  @Input() label = '';
  @Input() set option(value: number | null | undefined) {
    this.selectControl.setValue(value ?? null, { emitEvent: false });
  }
  @Output() optionChange = new EventEmitter<number>();
  private subscription?: Subscription;

  ngOnInit(): void {
    this.subscription = this.selectControl.valueChanges.pipe(distinctUntilChanged()).subscribe(value => {
      this.optionChange.emit(Number(value));
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
