import { useEffect, useMemo, useState } from "react";
import SearchBox from "../../components/SearchBox";
import type { IListingCase } from "../../types/IListingCase";
import { getAllListingCases } from "../../services/listingCaseService";
import ListingCaseTable from "../../features/admin/listingCase/ListingCaseTable";
import Spinner from "../../components/Spinner";
import CreatePropertyModal from "../../features/admin/listingCase/CreatePropertyModal";
import EditPropertyModal from "../../features/admin/listingCase/EditListingCaseModal";
// import Spinner from "../../components/Spinner";

export default function ListingCasePage() {
  const [listingCases, setListingCases] = useState<IListingCase[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [selectedListingCase, setSelectedListingCase] =
    useState<IListingCase | null>(null);
  const [searchTerm, setSearchTerm] = useState("");

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

  const handleEdit = (listingCase: IListingCase) => {
    setSelectedListingCase(listingCase);
    setShowEditModal(true);
  };

  const searchListingCases = useMemo(() => {
    return listingCases.filter(
      (listingCase) =>
        listingCase.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        listingCase.description
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        listingCase.street.toLowerCase().includes(searchTerm.toLowerCase()) ||
        listingCase.city.toLowerCase().includes(searchTerm.toLowerCase()) ||
        listingCase.state.toLowerCase().includes(searchTerm.toLowerCase()) ||
        listingCase.postcode.toString().includes(searchTerm.toLowerCase()) ||
        listingCase.propertyType
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        listingCase.saleCategory
          .toLowerCase()
          .includes(searchTerm.toLowerCase())
    );
  }, [listingCases, searchTerm]);

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
              value={searchTerm}
              onChange={setSearchTerm}
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
              onClick={() => setShowCreateModal(true)}
            >
              + Create Property
            </button>
          </div>
        </div>

        <div>
          {/* handleEdit is used to open the edit modal and get the listing case from the table */}
          <ListingCaseTable
            listingCases={searchListingCases}
            onEdit={handleEdit}
          />
        </div>
      </section>

      {/* Create Property Modal */}
      {showCreateModal && (
        <CreatePropertyModal
          open={showCreateModal}
          onClose={() => setShowCreateModal(false)}
        />
      )}

      {/* Edit Property Modal */}
      {showEditModal && (
        <EditPropertyModal
          open={showEditModal}
          onClose={() => {
            setShowEditModal(false);
            setSelectedListingCase(null);
          }}
          listingCase={selectedListingCase!}
        />
      )}
    </>
  );
}
