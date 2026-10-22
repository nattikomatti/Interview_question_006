export interface Product {
  id: number;
  productCode: string;
  createdAt: string;
}

export interface CreateProductRequest {
  productCode: string;
}
