import { useParams } from "react-router-dom";
import TopBar from "../../components/TopBar";
import { useCallback, useEffect, useState } from "react";
import {
  getListingCaseDetailsById,
  updateListingCaseById,
} from "../../services/listingCaseService";
import Hero from "../../features/preview/Hero";
import PropertyImages from "../../features/preview/PropertyImages";
import FloorPlan from "../../features/preview/FloorPlan";
import Videography from "../../features/preview/Videography";
import LocationMap from "../../features/preview/LocationMap";
import CaseContacts from "../../features/preview/CaseContacts";
import Footer from "../../features/preview/Footer";
import type {
  IListingCaseDetails,
  IUpdateListingCase,
} from "../../types/IListingCase";
import { MediaType } from "../../types/IMedia";

export default function PropertyDetailsAgentView() {
  const { listingCaseId } = useParams();
  const [propertyDetails, setPropertyDetails] =
    useState<IListingCaseDetails | null>(null);

  const loadListingCaseDetails = useCallback(async () => {
    const res = await getListingCaseDetailsById(parseInt(listingCaseId!));
    setPropertyDetails(res.data);
  }, [listingCaseId]);

  useEffect(() => {
    (async () => {
      try {
        await loadListingCaseDetails();
      } catch (error) {
        console.error(error);
      }
    })();
  }, [loadListingCaseDetails]);

  if (propertyDetails === null) return null;

  const mediaAssets = propertyDetails.mediaAssets;

  const heroImageUrl = mediaAssets.find(
    (asset) => asset.isHero === true
  )?.mediaUrl;

  const selectedImagesAsset = mediaAssets.filter(
    (asset) => asset.mediaType === MediaType.Photo && asset.isDeleted === false
  );

  const floorPlanAsset = mediaAssets.filter(
    (asset) =>
      asset.mediaType === MediaType.FloorPlan && asset.isDeleted === false
  );

  const videographyAsset = mediaAssets.filter(
    (asset) =>
      asset.mediaType === MediaType.Videography && asset.isDeleted === false
  );

  const handleUpdate = async (updatedParams: IUpdateListingCase) => {
    await updateListingCaseById(parseInt(listingCaseId!), updatedParams);
    await loadListingCaseDetails();
  };

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
        postcode={propertyDetails.postcode.toString()}
        bedrooms={propertyDetails.bedrooms}
        bathrooms={propertyDetails.bathrooms}
        garages={propertyDetails.garages}
        floorArea={propertyDetails.floorArea}
        propertyType={propertyDetails.propertyType}
        saleCategory={propertyDetails.saleCategory}
        onSave={handleUpdate}
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
