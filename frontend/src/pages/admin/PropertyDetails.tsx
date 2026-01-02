import { useParams } from "react-router-dom";
import TopBar from "../../components/TopBar";
import { useEffect, useState } from "react";
import { getListingCaseDetailsById } from "../../services/orderService";
import { MediaType, type IListingCaseDetails } from "../../types/IOrder";
import Hero from "../../features/order/details/Hero";
import PropertyImages from "../../features/order/details/PropertyImages";
import FloorPlan from "../../features/order/details/FloorPlan";
import Videography from "../../features/order/details/Videography";
import LocationMap from "../../features/order/details/LocationMap";
import CaseContacts from "../../features/order/details/CaseContacts";
import Footer from "../../features/order/details/Footer";

export default function PropertyDetails() {
  const { listingCaseId } = useParams();
  const [propertyDetails, setPropertyDetails] =
    useState<IListingCaseDetails | null>(null);

  useEffect(() => {
    const getDetails = async () => {
      try {
        const res = await getListingCaseDetailsById(listingCaseId!);
        const details: IListingCaseDetails = res.data;

        setPropertyDetails(details);
      } catch (error) {
        console.error(error);
      }
    };

    getDetails();
  }, [listingCaseId]);

  if (propertyDetails === null) return null;

  const mediaAssets = propertyDetails.mediaAssets;

  const heroImageUrl = mediaAssets.find(
    (asset) => asset.isHero === true
  )?.mediaUrl;

  const selectedImagesAsset = mediaAssets.filter(
    (asset) =>
      asset.mediaType === MediaType.Photo &&
      asset.isSelect === true &&
      asset.isDeleted === false
  );

  const floorPlanAsset = mediaAssets.filter(
    (asset) =>
      asset.mediaType === MediaType.FloorPlan &&
      asset.isSelect === true &&
      asset.isDeleted === false
  );

  const videographyAsset = mediaAssets.filter(
    (asset) =>
      asset.mediaType === MediaType.Videography &&
      asset.isSelect === true &&
      asset.isDeleted === false
  );

  return (
    <div>
      <div className="my-3">
        <TopBar />
      </div>

      {/* Hero Image */}
      <Hero
        imageUrl={heroImageUrl!}
        title={propertyDetails.title}
        description={propertyDetails.description}
        street={propertyDetails.street}
        city={propertyDetails.city}
        state={propertyDetails.state}
        postcode={propertyDetails.postcode}
        bedrooms={propertyDetails.bedrooms}
        bathrooms={propertyDetails.bathrooms}
        garages={propertyDetails.garages}
        floorArea={propertyDetails.floorArea}
        propertyType={propertyDetails.propertyType}
        saleCategory={propertyDetails.saleCategory}
      />

      <div className="bg-gray-300 h-1 my-2"></div>

      {/* Property Images */}
      <PropertyImages selectedImagesAsset={selectedImagesAsset} />

      <div className="bg-gray-300 h-1 my-2"></div>

      {/* Property Floor Plan */}
      {floorPlanAsset && <FloorPlan floorPlanAsset={floorPlanAsset} />}

      <div className="bg-gray-300 h-1 my-2"></div>

      {/* Videography */}
      {videographyAsset && <Videography videographyAsset={videographyAsset} />}

      <div className="bg-gray-300 h-1 my-2"></div>

      {/* Location */}
      <LocationMap
        latitude={propertyDetails.latitude}
        longitude={propertyDetails.longitude}
      />

      <div className="bg-gray-300 h-1 my-2"></div>

      {/* Case Contacts */}
      <CaseContacts caseContacts={propertyDetails.caseContacts} />

      {/* Footer */}
      <Footer />
    </div>
  );
}
