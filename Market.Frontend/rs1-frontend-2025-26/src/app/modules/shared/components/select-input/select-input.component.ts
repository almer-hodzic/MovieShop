import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Subscription } from 'rxjs';
import { distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-select-input',
  standalone: false,
  templateUrl: './select-input.component.html',
  styleUrl: './select-input.component.scss',
})
export class SelectInputComponent implements OnInit, OnDestroy {
  readonly selectControl = new FormControl('', { nonNullable: true });
  @Input() options: string[] = [];
  @Input() label = '';
  @Input() set option(value: string | null | undefined) {
    this.selectControl.setValue(value ?? '', { emitEvent: false });
  }
  @Output() optionChange = new EventEmitter<string>();
  private subscription?: Subscription;

  ngOnInit(): void {
    this.subscription = this.selectControl.valueChanges.pipe(distinctUntilChanged()).subscribe(value => {
      this.optionChange.emit(value);
    });
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }
}
