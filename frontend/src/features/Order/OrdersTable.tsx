import type { IOrder } from "../../types/IOrder";

interface OrdersTableProps {
  orders: IOrder[];
}

export default function OrdersTable({ orders }: OrdersTableProps) {
  return (
    <div className="border-2 border-gray-300 rounded-lg overflow-hidden">
      <table className="w-full">
        <tr className="text-left bg-gray-200">
          <th className="px-8 py-4">Order Number</th>
          <th className="px-8 py-4">Client Name</th>
          <th className="px-8 py-4">Property Address</th>
          <th className="px-8 py-4">Order Time</th>
          <th className="px-8 py-4">Status</th>
          <th className="px-8 py-4" />
        </tr>

        {orders.map((order) => (
          <tr className="divide-y-2 divide-gray-300" key={order.orderNumber}>
            <td className="px-8 py-4">{order.orderNumber}</td>
            <td className="px-8 py-4">{order.clientName}</td>
            <td className="px-8 py-4">{order.propertyAddress}</td>
            <td className="px-8 py-4">{order.orderTime}</td>
            <td className="px-8 py-4">{order.status}</td>
            <td className="px-8 py-4">
              <button>...</button>
            </td>
          </tr>
        ))}
        <tr>
          <td>1</td>
          <td>John Doe</td>
          <td>123 Main St, Anytown, USA</td>
          <td>2022-01-01 10:00:00</td>
          <td>Delivered</td>
          <td>
            <button>...</button>
          </td>
        </tr>
      </table>
    </div>
  );
}
