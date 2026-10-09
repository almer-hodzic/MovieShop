import { Injectable, inject } from '@angular/core';
import { IndividualConfig, ToastrService } from 'ngx-toastr';

/**
 * Global toaster service for displaying notifications.
 * Uses the same ngx-toastr presentation as the old MovieShop frontend.
 */
@Injectable({
  providedIn: 'root'
})
export class ToasterService {
  private toastr = inject(ToastrService);

  /**
   * Show success message (green)
   */
  success(message: string, duration?: number): void {
    this.toastr.success(message, undefined, this.durationOptions(duration));
  }

  /**
   * Show error message (red)
   */
  error(message: string, duration?: number): void {
    this.toastr.error(message, undefined, this.durationOptions(duration));
  }

  /**
   * Show warning message (orange)
   */
  warning(message: string, duration?: number): void {
    this.toastr.warning(message, undefined, this.durationOptions(duration));
  }

  /**
   * Show info message (blue)
   */
  info(message: string, duration?: number): void {
    this.toastr.info(message, undefined, this.durationOptions(duration));
  }

  clear(): void {
    this.toastr.clear();
  }

  private durationOptions(duration?: number): Partial<IndividualConfig> | undefined {
    return duration === undefined ? undefined : { timeOut: duration };
  }
}
