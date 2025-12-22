export interface IOrder {
  orderNumber: number;
  clientName: string;
  propertyAddress: string;
  orderTime: string;
  status: OrderStatus;
}

enum OrderStatus {
  Pending,
  Created,
  Delivered
}