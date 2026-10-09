import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  CapturePayPalOrderCommand,
  CapturePayPalOrderCommandDto,
  CreatePayPalOrderCommandDto,
} from './paypal-api.model';

@Injectable({
  providedIn: 'root',
})
export class PayPalApiService {
  private readonly baseUrl = `${environment.apiUrl}/PayPal`;
  private http = inject(HttpClient);

  createOrder(): Observable<CreatePayPalOrderCommandDto> {
    return this.http.post<CreatePayPalOrderCommandDto>(`${this.baseUrl}/CreateOrder`, null);
  }

  captureOrder(payload: CapturePayPalOrderCommand): Observable<CapturePayPalOrderCommandDto> {
    return this.http.post<CapturePayPalOrderCommandDto>(`${this.baseUrl}/CaptureOrder`, payload);
  }
}
