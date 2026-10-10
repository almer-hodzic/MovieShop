export interface GetMyShoppingCartItemDto {
  itemId: number;
  movieId: number;
  movieTitle: string;
  movieImage?: string | null;
  unitPrice: number;
  quantity: number;
  totalPrice: number;
  addedAt: string;
}

export interface GetMySavedForLaterItemDto {
  itemId: number;
  movieId: number;
  movieTitle: string;
  movieImage?: string | null;
  unitPrice: number;
  addedAt: string;
}

export interface GetMyShoppingCartQueryDto {
  cartId: number;
  userId: number;
  lastModifiedAt: string;
  items: GetMyShoppingCartItemDto[];
  savedForLaterItems: GetMySavedForLaterItemDto[];
  totalQuantity: number;
  totalAmount: number;
}

export interface AddItemToShoppingCartCommand {
  movieId: number;
  quantity: number;
}

export interface UpdateShoppingCartItemQuantityCommand {
  quantity: number;
}
