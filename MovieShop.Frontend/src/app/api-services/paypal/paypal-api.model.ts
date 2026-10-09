export interface CreatePayPalOrderCommandDto {
  orderId: string;
  amount: number;
  currencyCode: string;
}

export interface CapturePayPalOrderCommand {
  orderId: string;
}

export interface CapturePayPalOrderCommandDto {
  status: string;
  message: string;
}
