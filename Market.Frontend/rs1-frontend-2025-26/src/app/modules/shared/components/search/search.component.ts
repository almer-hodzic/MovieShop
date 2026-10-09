import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-search',
  standalone: false,
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
})
export class SearchComponent implements OnInit, OnDestroy {
  readonly searchField = new FormControl('', { nonNullable: true });
  @Input() customPlaceholder = '';
  @Input() set search(value: string | null | undefined) {
    this.searchField.setValue(value ?? '', { emitEvent: false });
  }
  @Output() searchChange = new EventEmitter<string>();
  private subscription?: Subscription;

  ngOnInit(): void {
    this.subscription = this.searchField.valueChanges.pipe(distinctUntilChanged()).subscribe(value => {
      this.searchChange.emit(value);
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  clear(): void {
    this.searchField.setValue('');
  }
}
