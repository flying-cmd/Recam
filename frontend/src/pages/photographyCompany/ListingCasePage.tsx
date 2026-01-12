import { useCallback, useEffect, useMemo, useState } from "react";
import SearchBox from "../../components/SearchBox";
import type {
  ICreateListingCase,
  IListingCase,
} from "../../types/IListingCase";
import {
  createListingCase,
  deleteListingCaseById,
  getAllListingCases,
} from "../../services/listingCaseService";
import Spinner from "../../components/Spinner";
import ListingCaseTable from "../../features/photographyCompany/listingCase/ListingCaseTable";
import CreatePropertyModal from "../../features/photographyCompany/listingCase/CreatePropertyModal";
import Pagination from "../../components/Pagination";

export default function ListingCasePage() {
  const [listingCases, setListingCases] = useState<IListingCase[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [pageNumber, setPageNumber] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 5;

  const loadListingCases = useCallback(async () => {
    try {
      const res = await getAllListingCases(pageNumber, pageSize);
      setListingCases(res.data.items);
      setTotalCount(res.data.totalCount);
    } catch (error) {
      console.error(error);
      throw new Error("Failed to load listing cases");
    }
  }, [pageNumber, pageSize]);

  useEffect(() => {
    (async () => {
      try {
        setIsLoading(true);
        await loadListingCases();
        setIsLoading(false);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [loadListingCases]);

  const handlePageChange = (page: number) => setPageNumber(page);

  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  useEffect(() => {
    if (pageNumber > totalPages) {
      setPageNumber(totalPages);
    }
  }, [pageNumber, totalPages]);

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

  const handleCreateListingCase = useCallback(
    async (formData: ICreateListingCase) => {
      try {
        await createListingCase(formData);
        await loadListingCases();
      } catch (error) {
        console.error(error);
        throw new Error("Failed to create listing case");
      }
    },
    [loadListingCases]
  );

  const handleDelete = useCallback(
    async (listingCaseId: number) => {
      try {
        await deleteListingCaseById(listingCaseId);
        await loadListingCases();
      } catch (error) {
        console.error(error);
        throw new Error("Failed to delete listing case");
      }
    },
    [loadListingCases]
  );

  if (isLoading) return <Spinner />;

  return (
    <>
      <h1 className="text-center font-bold text-2xl mt-20">Hi, Welcome!</h1>

      <section className="sm:mx-25 mb-10">
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
            onDelete={handleDelete}
          />
        </div>

        {/* Pagination */}
        <div className="mt-2">
          <Pagination
            currentPage={pageNumber}
            pageSize={pageSize}
            totalCount={totalCount}
            onPageChange={handlePageChange}
            siblingCount={1}
          />
        </div>
      </section>

      {/* Create Property Modal */}
      {showCreateModal && (
        <CreatePropertyModal
          open={showCreateModal}
          onClose={() => setShowCreateModal(false)}
          onCreate={handleCreateListingCase}
        />
      )}
    </>
  );
}
