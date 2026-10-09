import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { StreamState } from './stream-state';

interface YouTubePlayerEvent {
  data: number;
  target: {
    playVideo: () => void;
  };
}

interface YouTubePlayer {
  playVideo: () => void;
  pauseVideo: () => void;
  stopVideo: () => void;
  destroy?: () => void;
}

interface YouTubeApi {
  Player: new (
    elementId: string,
    options: {
      videoId: string;
      height: string;
      width: string;
      playerVars: Record<string, number>;
      events: {
        onStateChange: (event: YouTubePlayerEvent) => void;
        onError: () => void;
        onReady: (event: YouTubePlayerEvent) => void;
      };
    }
  ) => YouTubePlayer;
  PlayerState: {
    PLAYING: number;
    PAUSED: number;
    ENDED: number;
  };
}

declare global {
  interface Window {
    YT?: YouTubeApi;
    onYouTubeIframeAPIReady?: () => void;
  }
}

@Injectable({
  providedIn: 'root',
})
export class ClientPlayerService {
  private state: StreamState = {
    playing: false,
    paused: false,
    error: false,
  };

  private stateChange = new BehaviorSubject<StreamState>(this.state);
  private player: YouTubePlayer | null = null;
  private pendingVideoId: string | null = null;

  init(id: string): void {
    this.pendingVideoId = id;

    if (window.YT?.Player) {
      this.startVideo(id);
      return;
    }

    const existingScript = document.getElementById('youtube-iframe-api');
    if (!existingScript) {
      const tag = document.createElement('script');
      tag.id = 'youtube-iframe-api';
      tag.src = 'https://www.youtube.com/iframe_api';
      const firstScriptTag = document.getElementsByTagName('script')[0];
      firstScriptTag.parentNode?.insertBefore(tag, firstScriptTag);
    }

    window.onYouTubeIframeAPIReady = () => {
      if (this.pendingVideoId) {
        this.startVideo(this.pendingVideoId);
      }
    };
  }

  getState(): Observable<StreamState> {
    return this.stateChange.asObservable();
  }

  play(): void {
    this.player?.playVideo();
  }

  pause(): void {
    this.player?.pauseVideo();
  }

  stop(): void {
    this.resetState();
    this.stateChange.next(this.state);
    this.player?.stopVideo();
  }

  destroy(): void {
    this.player?.destroy?.();
    this.player = null;
    this.pendingVideoId = null;
    this.resetState();
    this.stateChange.next(this.state);
  }

  private startVideo(id: string): void {
    if (!window.YT?.Player) {
      this.markError();
      return;
    }

    this.player?.destroy?.();
    this.player = new window.YT.Player('player', {
      videoId: id,
      height: '90%',
      width: '100%',
      playerVars: {
        autoplay: 1,
        modestbranding: 1,
        controls: 1,
        rel: 0,
        fs: 1,
        playsinline: 0,
      },
      events: {
        onStateChange: this.onPlayerStateChange.bind(this),
        onError: this.onPlayerError.bind(this),
        onReady: this.onPlayerReady.bind(this),
      },
    });
  }

  private onPlayerReady(event: YouTubePlayerEvent): void {
    event.target.playVideo();
  }

  private onPlayerStateChange(event: YouTubePlayerEvent): void {
    if (!window.YT?.PlayerState) {
      return;
    }

    switch (event.data) {
      case window.YT.PlayerState.PLAYING:
        this.state = { playing: true, paused: false, error: false };
        break;
      case window.YT.PlayerState.PAUSED:
        this.state = { playing: false, paused: true, error: false };
        break;
      case window.YT.PlayerState.ENDED:
        this.resetState();
        break;
    }

    this.stateChange.next(this.state);
  }

  private onPlayerError(): void {
    this.markError();
  }

  private markError(): void {
    this.state = {
      playing: false,
      paused: false,
      error: true,
    };
    this.stateChange.next(this.state);
  }

  private resetState(): void {
    this.state = {
      playing: false,
      paused: false,
      error: false,
    };
  }
}
