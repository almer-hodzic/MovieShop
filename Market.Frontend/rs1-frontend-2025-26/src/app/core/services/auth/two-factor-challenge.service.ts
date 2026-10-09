import { Injectable } from '@angular/core';

export interface TwoFactorChallenge {
  userId: number;
  password: string;
  fingerprint: string | null;
}

@Injectable({ providedIn: 'root' })
export class TwoFactorChallengeService {
  private challenge: TwoFactorChallenge | null = null;

  start(challenge: TwoFactorChallenge): void {
    this.challenge = challenge;
  }

  get snapshot(): TwoFactorChallenge | null {
    return this.challenge;
  }

  clear(): void {
    this.challenge = null;
  }
}
