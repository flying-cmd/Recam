import StatusBox from "../../components/StatusBox";
import type { IOrder } from "../../types/IOrder";

interface OrdersTableProps {
  orders: IOrder[];
}

export default function OrdersTable({ orders }: OrdersTableProps) {
  return (
    <div className="border-2 border-gray-300 rounded-lg overflow-hidden">
      <table className="w-full text-xs md:text-base">
        <thead>
          <tr className="text-left bg-gray-200">
            <th className="md:px-8 py-4">Order Number</th>
            <th className="md:px-8 py-4">Client Name</th>
            <th className="md:px-8 py-4">Property Address</th>
            <th className="md:px-8 py-4">Order Time</th>
            <th className="md:px-8 py-4">Status</th>
            <th className="md:px-8 py-4" />
          </tr>
        </thead>

        <tbody className="divide-y-2 divide-gray-300">
          {orders.map((order) => (
            <tr key={order.id}>
              <td className="md:px-8 py-4">{order.id}</td>
              <td className="md:px-8 py-4">
                {order.agents
                  .map(
                    (agent) => agent.agentFirstName + " " + agent.agentLastName
                  )
                  .join(", ")}
              </td>
              <td className="md:px-8 py-4">
                {order.street + ", " + order.city + ", " + order.state}
              </td>
              <td className="md:px-8 py-4">
                {new Date(order.createdAt).toLocaleString()}
              </td>
              <td className="md:px-8 py-4">
                <StatusBox status={order.listingCaseStatus} />
              </td>
              <td className="md:px-8 py-4">
                <button type="button" className="cursor-pointer font-bold">
                  ...
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
