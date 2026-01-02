import { OrderStatus } from "../types/IOrder";

interface StatusBoxProps {
  status: string;
}

export default function StatusBox({ status }: StatusBoxProps) {
  const statusToEnum: OrderStatus =
    OrderStatus[status as keyof typeof OrderStatus];

  switch (statusToEnum) {
    case OrderStatus.Created:
      return (
        <div className="bg-blue-300 text-white text-center py-0.5 rounded">
          Created
        </div>
      );
    case OrderStatus.Pending:
      return (
        <div className="bg-orange-300 text-white text-center py-0.5 rounded">
          Pending
        </div>
      );
    case OrderStatus.Delivered:
      return (
        <div className="bg-green-300 text-white text-center py-0.5 rounded">
          Delivered
        </div>
      );
    default:
      return null;
  }
}
