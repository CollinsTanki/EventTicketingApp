// src/types/index.ts
export interface TicketType {
  id: number;
  name: string;
  price: number;
  available: number;
}

export interface Event {
  id: number;
  title: string;
  description: string;
  venueName: string;
  city: string;
  startDateTime: string;
  endDateTime: string;
  imageUrl: string;
  ticketTypes: TicketType[];
}

export interface OrderItemInput {
  ticketTypeId: number;
  quantity: number;
}

export interface TicketResponse {
  ticketId: number;
  ticketCode: string;
  ticketTypeName: string;
}

export interface OrderResponse {
  orderId: number;
  status: string;
  totalAmount: number;
  createdAt: string;
  tickets: TicketResponse[];
}

export interface AuthResponse {
  token: string;
  name: string;
  email: string;
}