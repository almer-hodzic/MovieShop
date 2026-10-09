import { Component, Input, OnChanges } from '@angular/core';

@Component({
  selector: 'app-star',
  standalone: false,
  templateUrl: './star.component.html',
  styleUrl: './star.component.scss',
})
export class StarComponent implements OnChanges {
  @Input() rating = 0;
  starWidth = 0;

  ngOnChanges(): void {
    this.starWidth = Math.max(0, Math.min(5, this.rating)) * 90 / 5;
  }
}
