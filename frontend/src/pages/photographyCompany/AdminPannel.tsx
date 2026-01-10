import { useEffect, useState } from "react";
import SearchBox from "../../components/SearchBox";
import type { IListingCase } from "../../types/IListingCase";
import { getAllListingCases } from "../../services/listingCaseService";
import ListingCaseTable from "../../features/photographyCompany/listingCase/ListingCaseTable";

export default function AdminPannel() {
  const [listingCases, setListingCases] = useState<IListingCase[]>([]);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    (async () => {
      try {
        const res = await getAllListingCases();
        if (searchTerm) {
          setListingCases(
            listingCases.filter(
              (listingCase) =>
                listingCase.title
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.description
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.street
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.city
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.state
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.postcode
                  .toString()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.propertyType
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase()) ||
                listingCase.saleCategory
                  .toLowerCase()
                  .includes(searchTerm.toLowerCase())
            )
          );
        } else {
          setListingCases(res.data.items);
        }
      } catch (error) {
        console.error(error);
      }
    })();
  });

  return (
    <>
      <h1 className="text-center font-bold text-2xl mt-20">Hi, Welcome!</h1>

      <section className="sm:mx-25">
        <div className="flex items-center my-6">
          {/* spacer */}
          <div className="flex-1" />

          <div className="flex-1 flex justify-center">
            <SearchBox
              className="sm:w-140 h-full w-full"
              placeholder="Search from listing cases"
              value={searchTerm}
              onChange={setSearchTerm}
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
            >
              + Create Property
            </button>
          </div>
        </div>

        <div>
          <ListingCaseTable listingCases={listingCases} />
        </div>
      </section>
    </>
  );
}
