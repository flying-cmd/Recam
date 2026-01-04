import ActionsButton from "../../components/ActionsButton";
import StatusBox from "../../components/StatusBox";
import type { IListingCase } from "../../types/IListingCase";

interface listingCaseTableProps {
  listingCases: IListingCase[];
}

export default function ListingCaseTable({
  listingCases,
}: listingCaseTableProps) {
  return (
    <div className="border-2 border-gray-300 rounded-lg">
      <table className="w-full text-xs md:text-base">
        <thead>
          <tr className="text-left bg-gray-200">
            <th className="md:px-8 py-4">PROPERTY#</th>
            <th className="md:px-8 py-4">PROPERTY TYPE</th>
            <th className="md:px-8 py-4">PROPERTY ADDRESS</th>
            <th className="md:px-8 py-4">CREATED AT</th>
            <th className="md:px-8 py-4">STATUS</th>
            <th className="md:px-8 py-4">ACTIONS</th>
          </tr>
        </thead>

        <tbody className="divide-y-2 divide-gray-300">
          {listingCases.map((listingCase) => (
            <tr key={listingCase.id}>
              <td className="md:px-8 py-4">{listingCase.id}</td>
              <td className="md:px-8 py-4">{listingCase.propertyType}</td>
              <td className="md:px-8 py-4">
                {listingCase.street +
                  ", " +
                  listingCase.city +
                  ", " +
                  listingCase.state}
              </td>
              <td className="md:px-8 py-4">
                {new Date(listingCase.createdAt).toLocaleString()}
              </td>
              <td className="md:px-8 py-4">
                <StatusBox status={listingCase.listingCaseStatus} />
              </td>
              <td className="md:px-8 py-4">
                <ActionsButton />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
