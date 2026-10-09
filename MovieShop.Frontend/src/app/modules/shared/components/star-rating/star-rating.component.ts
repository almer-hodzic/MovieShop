import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';

@Component({
  selector: 'mat-star-rating',
  standalone: false,
  templateUrl: './star-rating.component.html',
  styleUrl: './star-rating.component.scss',
})
export class StarRatingComponent implements OnChanges {
  @Input() rating = 3;
  @Input() starCount = 5;
  @Input() color: 'primary' | 'accent' | 'warn' = 'accent';
  @Input() readonly = false;
  @Output() ratingUpdated = new EventEmitter<number>();
  ratingArr: number[] = [];

  ngOnChanges(): void {
    this.ratingArr = Array.from({ length: Math.max(0, this.starCount) }, (_, index) => index);
  }

  onClick(rating: number): void {
    if (!this.readonly) this.ratingUpdated.emit(rating);
  }

  showIcon(index: number): string {
    const normalized = Math.max(0, Math.min(this.starCount, Number(this.rating ?? 0)));
    if (normalized >= index + 1) return 'star';
    if (normalized >= index + 0.5) return 'star_half';
    return 'star_border';
  }
}
