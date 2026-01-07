import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { IListingCaseDetails } from "../../types/IListingCase";
import { getListingCaseDetailsById } from "../../services/listingCaseService";
import Spinner from "../../components/Spinner";
import TabButton from "../../features/admin/listingCase/TabButton";
import { Grid2x2, House, Image, ScanEye, Users, Video } from "lucide-react";
import UploadPhotographyPannel from "../../features/admin/listingCase/edit/UploadPhotographyPannel";
import EditPropertyDetailsPannel from "../../features/admin/listingCase/edit/EditPropertyDetailsPannel";

type NavSection =
  | ""
  | "Photography"
  | "Floor Plan"
  | "Videography"
  | "VR Tour"
  | "Agents"
  | "Property details";

export default function EditListingCasePage() {
  const { listingCaseId } = useParams<{ listingCaseId: string }>();
  const [isLoading, setIsLoading] = useState(true);
  const [listingCase, setListingCase] = useState<IListingCaseDetails | null>(
    null
  );
  const [navSection, setNavSection] = useState<NavSection>("");

  useEffect(() => {
    (async () => {
      try {
        const res = await getListingCaseDetailsById(parseInt(listingCaseId!));
        setListingCase(res.data);
        setIsLoading(false);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [listingCaseId]);

  const renderPannel = () => {
    switch (navSection) {
      case "Photography":
        return (
          <UploadPhotographyPannel
            listingCaseId={parseInt(listingCaseId!)}
            onBack={() => setNavSection("")}
          />
        );
      case "Property details":
        return (
          <EditPropertyDetailsPannel
            listingCase={listingCase!}
            onBack={() => setNavSection("")}
          />
        );
      default:
        return null;
    }
  };

  return (
    <div className="flex flex-col items-center">
      <div>
        <h1 className="text-center font-bold text-2xl mt-20">Hi, Welcome!</h1>
      </div>

      <div className="mt-12 w-full px-40">
        {/* Nav */}
        <div className="flex flex-row gap-2 items-start text-gray-500 mb-3">
          <span>Property</span>
          <span>{">"}</span>
          <span>
            {`${listingCase?.street}, ${listingCase?.city}, ${listingCase?.state}, ${listingCase?.postcode}`}
          </span>
          {navSection && <span>{`> ${navSection}`}</span>}
        </div>

        {isLoading ? (
          <Spinner />
        ) : navSection === "" ? (
          // Button
          <div className="flex flex-row gap-4 flex-wrap items-center w-full">
            <TabButton
              onClick={() => setNavSection("Photography")}
              icon={<Image color="#2d87e6" />}
              title="Photography"
            />
            <TabButton
              onClick={() => setNavSection("Floor Plan")}
              icon={<Grid2x2 color="#e6802d" />}
              title="Floor Plan"
            />
            <TabButton
              onClick={() => setNavSection("Videography")}
              icon={<Video color="#209760" />}
              title="Videography"
            />
            <TabButton
              onClick={() => setNavSection("VR Tour")}
              icon={<ScanEye color="#e06c1f" />}
              title="VR Tour"
            />
            <TabButton
              onClick={() => setNavSection("Agents")}
              icon={<Users color="#2e89c2" />}
              title="Agents"
            />
            <TabButton
              onClick={() => setNavSection("Property details")}
              icon={<House color="#68037c" />}
              title="Property details"
            />
          </div>
        ) : (
          <div className="border-none rounded-md p-6 shadow-lg w-full">
            {renderPannel()}
          </div>
        )}
      </div>

      {navSection === "" && (
        <div className="mt-12">
          <button
            type="button"
            className="bg-sky-600 rounded-md p-2 hover:bg-sky-700 text-white"
          >
            Deliver to agent
          </button>
        </div>
      )}
    </div>
  );
}
