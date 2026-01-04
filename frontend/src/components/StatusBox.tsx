import { ListingCaseStatus } from "../types/IListingCase";

interface StatusBoxProps {
  status: string;
}

export default function StatusBox({ status }: StatusBoxProps) {
  switch (status) {
    case ListingCaseStatus.Created:
      return (
        <div className="bg-blue-300 text-white text-center py-0.5 rounded">
          Created
        </div>
      );
    case ListingCaseStatus.Pending:
      return (
        <div className="bg-orange-300 text-white text-center py-0.5 rounded">
          Pending
        </div>
      );
    case ListingCaseStatus.Delivered:
      return (
        <div className="bg-green-300 text-white text-center py-0.5 rounded">
          Delivered
        </div>
      );
    default:
      return null;
  }
}
