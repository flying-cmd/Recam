import StatusBox from "../../components/StatusBox";
import type { IListingCaseDetails } from "../../types/IListingCase";

interface PropertyCardProps {
  property: IListingCaseDetails;
  onViewDetails: () => void;
}

export default function PropertyCard({
  property,
  onViewDetails,
}: PropertyCardProps) {
  return (
    <div className="relative border-none rounded-md">
      {/* Status */}
      <div className="absolute top-0 right-0">
        <StatusBox status={property.listingCaseStatus} />
      </div>

      <div className="m-6">
        {/* Header */}
        <div className="text-gray-500">Propperty # {property.id}</div>
        <div className="text-gray-400">
          Created on {new Date(property.createdAt).toLocaleString()}
        </div>

        <hr className="mt-4 w-full border-gray-300" />

        {/* Address */}
        <div className="font-bold text-lg my-4">
          {property.street}, {property.city}, {property.state},{" "}
          {property.postcode}
        </div>

        {/* View Property Details Button */}
        <div className="flex justify-end">
          <button
            type="button"
            onClick={onViewDetails}
            className="hover:cursor-pointer text-gray-400 hover:text-black"
          >
            View property details {"->"}
          </button>
        </div>
      </div>
    </div>
  );
}
