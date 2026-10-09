import { Component, HostListener } from '@angular/core';

@Component({
  selector: 'app-scroll-top',
  standalone: false,
  templateUrl: './scroll-top.component.html',
  styleUrl: './scroll-top.component.scss',
})
export class ScrollTopComponent {
  isShow = false;
  readonly topPosToStartShowing = 100;

  @HostListener('window:scroll')
  checkScroll(): void {
    const position = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;
    this.isShow = position >= this.topPosToStartShowing;
  }

  gotoTop(): void {
    window.scroll({ top: 0, left: 0, behavior: 'smooth' });
  }
}
