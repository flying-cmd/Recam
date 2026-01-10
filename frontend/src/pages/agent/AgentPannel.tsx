import { useEffect, useMemo, useState } from "react";
import SearchBox from "../../components/SearchBox";
import TabButton from "../../features/agent/TabButton";
import { useAuth } from "../../hooks/useAuth";
import type { IListingCaseDetails } from "../../types/IListingCase";
import { getListingCaseByUserId } from "../../services/listingCaseService";
import PropertyCard from "../../features/agent/PropertyCard";
import Spinner from "../../components/Spinner";
import Pagination from "../../components/Pagination";

export default function AgentPannel() {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState<
    "All" | "Created" | "Pending" | "Delivered"
  >("All");
  const [isLoading, setIsLoading] = useState(true);
  const [propertyList, setPropertyList] = useState<IListingCaseDetails[]>([]);
  const [pageNumber, setPageNumber] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const pageSize = 5;

  // Get property list
  useEffect(() => {
    (async () => {
      try {
        setIsLoading(true);
        const res = await getListingCaseByUserId(pageNumber, pageSize);
        setTotalCount(res.data.totalCount);
        if (activeTab !== "All") {
          setPropertyList(
            res.data.items.filter(
              (item) => item.listingCaseStatus === activeTab
            )
          );
        } else {
          setPropertyList(res.data.items);
        }
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [user?.id, pageNumber, pageSize, activeTab]);

  const handleChangeTab = (
    tab: "All" | "Created" | "Pending" | "Delivered"
  ) => {
    setActiveTab(tab);
    setPageNumber(1);
  };

  const totalPages = useMemo(
    () => Math.max(1, Math.ceil(totalCount / pageSize)),
    [totalCount, pageSize]
  );

  useEffect(() => {
    if (pageNumber > totalPages) setPageNumber(totalPages);
  }, [pageNumber, totalPages]);

  if (isLoading) return <Spinner />;

  return (
    <div className="flex flex-col h-screen bg-gray-100">
      {/* Header */}
      <div className="bg-white">
        <div className="flex flex-col pt-8 pb-4 px-16 gap-4">
          <div>
            Hi, {user?.agentFirstName} {user?.agentLastName}
          </div>
          <div className="flex flex-row justify-between">
            <h2 className="font-bold text-xl">My Property</h2>
            <SearchBox
              className=""
              placeholder="Search my property"
              value=""
              onChange={() => {}}
            />
          </div>
        </div>

        <hr className="mt-4 w-full border-gray-300" />
      </div>

      {/* Content */}
      <div className="flex flex-row gap-2 px-16">
        {/* Left Sidebar */}
        <div className="w-1/5 h-full py-2">
          <div className="flex flex-col gap-2 w-full items-center justify-center">
            {/* Tab Button */}
            {/* All */}
            <TabButton
              title="All"
              active={activeTab === "All"}
              onClick={() => {
                handleChangeTab("All");
              }}
            />
            {/* Created */}
            <TabButton
              title="Created"
              active={activeTab === "Created"}
              onClick={() => {
                handleChangeTab("Created");
              }}
            />
            {/* Pending */}
            <TabButton
              title="Pending"
              active={activeTab === "Pending"}
              onClick={() => {
                handleChangeTab("Pending");
              }}
            />
            {/* Delivered */}
            <TabButton
              title="Delivered"
              active={activeTab === "Delivered"}
              onClick={() => {
                handleChangeTab("Delivered");
              }}
            />
          </div>
        </div>

        {/* Main Content */}
        <div className="w-4/5 mt-2 flex flex-col gap-3">
          {propertyList.length > 0 ? (
            <>
              {propertyList.map((property) => (
                <PropertyCard
                  key={property.id}
                  property={property}
                  onViewDetails={() => {}}
                />
              ))}

              {/* Pagination */}
              <div className="mt-2">
                <Pagination
                  currentPage={pageNumber}
                  pageSize={pageSize}
                  totalCount={totalCount}
                  onPageChange={setPageNumber}
                  siblingCount={1}
                />
              </div>
            </>
          ) : (
            <span className="text-center m-2">No property found</span>
          )}
        </div>
      </div>
    </div>
  );
}
