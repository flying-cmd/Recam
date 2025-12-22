import type { IOrder } from "../types/IOrder";

const BACKEND_URL = import.meta.env.VITE_BACKEND_URL;

export async function getAllOrders(): Promise<IOrder[]> {
  const res = await fetch(`${BACKEND_URL}/orders`, {
    headers: {
      "Content-Type": "application/json",
    }
  });

  if (!res.ok) {
    throw new Error("Failed to fetch orders");
  }

  const data = await res.json();
  return data;
}