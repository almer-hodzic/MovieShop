import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AddItemToShoppingCartCommand,
  GetMyShoppingCartQueryDto,
  UpdateShoppingCartItemQuantityCommand,
} from './shopping-cart-api.model';

@Injectable({
  providedIn: 'root',
})
export class ShoppingCartApiService {
  private readonly baseUrl = `${environment.apiUrl}/ShoppingCart`;
  private http = inject(HttpClient);

  getMine(): Observable<GetMyShoppingCartQueryDto> {
    return this.http.get<GetMyShoppingCartQueryDto>(`${this.baseUrl}/my`);
  }

  addItem(payload: AddItemToShoppingCartCommand): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/items`, payload);
  }

  updateItemQuantity(itemId: number, payload: UpdateShoppingCartItemQuantityCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/items/${itemId}/quantity`, payload);
  }

  removeItem(itemId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/items/${itemId}`);
  }

  clear(): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/clear`);
  }
}
