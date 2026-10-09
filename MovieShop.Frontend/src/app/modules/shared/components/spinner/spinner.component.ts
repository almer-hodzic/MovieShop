import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-spinner',
  standalone: false,
  templateUrl: './spinner.component.html',
  styleUrl: './spinner.component.scss',
})
export class SpinnerComponent implements OnInit {
  @Input() size = 60;
  @Input() color = '#fff';
  @Input() animationDuration = 2000;
  currentAnimationName = '';

  ngOnInit(): void {
    this.currentAnimationName = 'spring-spinner-animation';
  }

  get spinnerStyle(): object {
    return { height: `${this.size}px`, width: `${this.size}px` };
  }

  get spinnerPartStyle(): object {
    return { height: `${this.size / 2}px`, width: `${this.size}px` };
  }

  get rotatorStyle(): object {
    return {
      height: `${this.size}px`,
      width: `${this.size}px`,
      borderRightColor: this.color,
      borderTopColor: this.color,
      borderWidth: `${this.size / 7}px`,
      animationDuration: `${this.animationDuration}ms`,
      animationName: this.currentAnimationName,
    };
  }
}
