import type { IOrder } from "../types/IOrder";
import { service } from "./request";

export async function getAllOrders(): Promise<IOrder[]> {
  const res = await service<IOrder[]>({
    url: "/listings",
    method: "get"
  });

  return res.data;
}