import { useEffect, useState } from "react";
import SearchBox from "../../components/SearchBox";
import type { IListingCase } from "../../types/IListingCase";
import { getAllListingCases } from "../../services/listingCaseService";
import ListingCaseTable from "../../features/admin/listingCase/ListingCaseTable";
import Spinner from "../../components/Spinner";
import CreatePropertyModal from "../../features/admin/listingCase/CreatePropertyModal";
// import Spinner from "../../components/Spinner";

export default function ListingCasePage() {
  const [listingCases, setListingCases] = useState<IListingCase[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const res = await getAllListingCases();
        setListingCases(res.data.items);
        setIsLoading(false);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  });

  if (isLoading) return <Spinner />;

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
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
              onClick={() => setShowModal(true)}
            >
              + Create Property
            </button>
          </div>
        </div>

        <div>
          <ListingCaseTable listingCases={listingCases} />
        </div>
      </section>

      {/* Create Property Modal */}
      {showModal && (
        <CreatePropertyModal
          open={showModal}
          onClose={() => setShowModal(false)}
        />
      )}
    </>
  );
}
