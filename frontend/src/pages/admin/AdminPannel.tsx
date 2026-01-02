import { useEffect, useState } from "react";
import SearchBox from "../../components/SearchBox";
import OrdersTable from "../../features/order/OrdersTable";
import type { IOrder } from "../../types/IOrder";
import { getAllOrders } from "../../services/orderService";

export default function AdminPannel() {
  const [orders, setOrders] = useState<IOrder[]>([]);

  useEffect(() => {
    (async () => {
      try {
        const orders = await getAllOrders();
        setOrders(orders.data.items);
      } catch (error) {
        console.error(error);
      }
    })();
  });

  return (
    <>
      <h1 className="text-center font-bold text-2xl mt-20">
        Hi, Welcome Username!
      </h1>

      <section className="sm:mx-25">
        <div className="flex items-center my-6">
          {/* spacer */}
          <div className="flex-1" />

          <div className="flex-1 flex justify-center">
            <SearchBox
              className="sm:w-140 h-full w-full"
              placeholder="Search from order list"
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
            >
              + Create Order
            </button>
          </div>
        </div>

        <div>
          <OrdersTable orders={orders} />
        </div>
      </section>
    </>
  );
}
