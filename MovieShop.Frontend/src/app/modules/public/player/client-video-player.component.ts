import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ClientPlayerService } from './client-player.service';
import { StreamState } from './stream-state';

@Component({
  selector: 'app-client-video-player',
  standalone: false,
  templateUrl: './client-video-player.component.html',
  styleUrl: './client-video-player.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ClientVideoPlayerComponent implements OnInit, OnDestroy {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private playerService = inject(ClientPlayerService);
  private cdr = inject(ChangeDetectorRef);

  state$: Observable<StreamState> = this.playerService.getState().pipe(
    tap(() => this.cdr.detectChanges()),
  );

  videoId = '';

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const routeValue = String(params['ytId'] ?? '');
      this.videoId = this.extractYoutubeVideoId(routeValue);

      if (this.videoId) {
        this.playerService.init(this.videoId);
      }
    });
  }

  ngOnDestroy(): void {
    this.playerService.destroy();
  }

  pause(): void {
    this.playerService.pause();
  }

  play(): void {
    this.playerService.play();
  }

  stop(): void {
    this.playerService.stop();
    this.router.navigate(['/movies']);
  }

  private extractYoutubeVideoId(rawTrailer: string): string {
    const value = decodeURIComponent(rawTrailer).trim();
    if (!value) {
      return '';
    }

    if (!value.includes('http')) {
      return value;
    }

    try {
      const url = new URL(value);
      if (url.hostname.includes('youtu.be')) {
        return url.pathname.replace('/', '');
      }
      if (url.hostname.includes('youtube.com')) {
        return url.searchParams.get('v') ?? '';
      }
    } catch {
      return '';
    }

    return '';
  }
}
