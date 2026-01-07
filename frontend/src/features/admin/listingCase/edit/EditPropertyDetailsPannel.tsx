import { Bath, BedSingle, CarFront, Grid2x2 } from "lucide-react";
import { useMemo, useState } from "react";
import MapAutocompleteComponent from "../MapAutocompleteComponent";
import type {
  IAddress,
  IListingCaseDetails,
} from "../../../../types/IListingCase";
import { useJsApiLoader } from "@react-google-maps/api";
import { LIBRARIES } from "../../../../utils/googleMapConfig";
import TextField from "../../../../components/form/TextField";
import TextAreaField from "../../../../components/form/TextAreaField";
import RadioGroupField from "../../../../components/form/RadioGroupField";
import BasicInfoCard from "../../../../components/form/BasicInfoCard";
import { updateListingCaseById } from "../../../../services/listingCaseService";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../../../utils/MapZodErrorsToFields";
import { editListingCaseSchema } from "../ListingCaseSchema";
import SectionPannelLayout from "./SectionPannelLayout";

interface EditPropertyDetailsPannelProps {
  listingCase: IListingCaseDetails;
  onBack: () => void;
}

interface FormState {
  title: string;
  description: string;
  street: string;
  city: string;
  state: string;
  postcode: number;
  longitude: number;
  latitude: number;
  price: number;
  bedrooms: number;
  bathrooms: number;
  garages: number;
  floorArea: number;
  propertyType: string;
  saleCategory: string;
}

type FormErrors = FieldErrors<keyof FormState>;

export default function EditPropertyDetailsPannel({
  listingCase,
  onBack,
}: EditPropertyDetailsPannelProps) {
  const { isLoaded } = useJsApiLoader({
    id: "google-map-script",
    googleMapsApiKey: import.meta.env.VITE_GOOGLEMAPS_API_KEY,
    libraries: LIBRARIES,
  });

  const saleCategoryOptions = useMemo(
    () => [
      { label: "For Sale", value: "ForSale" },
      { label: "For Rent", value: "ForRent" },
      { label: "Auction", value: "Auction" },
    ],
    []
  );

  const propertyTypeOptions = useMemo(
    () => [
      { label: "House", value: "House" },
      { label: "Unit", value: "Unit" },
      { label: "Townhouse", value: "Townhouse" },
      { label: "Villa", value: "Villa" },
      { label: "Others", value: "Others" },
    ],
    []
  );

  const [form, setForm] = useState<FormState>({
    title: listingCase.title,
    description: listingCase.description,
    street: listingCase.street,
    city: listingCase.city,
    state: listingCase.state,
    postcode: listingCase.postcode,
    longitude: listingCase.longitude,
    latitude: listingCase.latitude,
    price: listingCase.price,
    bedrooms: listingCase.bedrooms,
    bathrooms: listingCase.bathrooms,
    garages: listingCase.garages,
    floorArea: listingCase.floorArea,
    propertyType: listingCase.propertyType,
    saleCategory: listingCase.saleCategory,
  });
  const [addressSearch, setAddressSearch] = useState<string>("");
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [errors, setErrors] = useState<FormErrors>({});

  // K extends keyof FormState: K must be a key of FormState
  // FormState[K]: get the type of the key
  const handleFormChange = <K extends keyof FormState>(
    key: K,
    value: FormState[K]
  ) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const handleSelectAddress = (address: IAddress) => {
    setForm((prev) => ({
      ...prev,
      street: address.street,
      city: address.city,
      state: address.state,
      postcode: address.postcode,
      longitude: address.longitude,
      latitude: address.latitude,
    }));
  };

  const handleSave = async () => {
    // validate form
    const formResult = editListingCaseSchema.safeParse(form);

    // if the form is not valid
    if (!formResult.success) {
      const errors = MapZodErrorsToFields(formResult.error);
      setErrors(errors);
      return;
    }

    // if the form is valid
    setErrors({});

    try {
      setIsSaving(true);
      await updateListingCaseById(listingCase.id, form);
    } catch (error) {
      console.error(error);
    } finally {
      setIsSaving(false);
    }
  };

  if (!open) {
    return null;
  }

  return (
    <SectionPannelLayout title="Property Details" onBack={onBack}>
      {/* Property Title */}
      <TextField
        label="Property Title"
        value={form.title}
        placeholder="Please provide a title for the property."
        onChange={(value) => handleFormChange("title", value)}
        error={errors.title}
      />
      <hr className="mt-4 w-full border-gray-300" />

      {/* Property Description */}
      <TextAreaField
        label="Property Description"
        value={form.description}
        placeholder="Please provide a description of the property."
        onChange={(value) => handleFormChange("description", value)}
        rows={4}
        error={errors.description}
      />
      <hr className="mt-4 w-full border-gray-300" />

      {/* Sale Category */}
      <RadioGroupField
        label="Sale Category"
        name="saleCategory"
        value={form.saleCategory}
        onChange={(value) => handleFormChange("saleCategory", value)}
        options={saleCategoryOptions}
        error={errors.saleCategory}
      />
      <hr className="mt-4 w-full border-gray-300" />

      {/* Property Type */}
      <RadioGroupField
        label="Property Type"
        name="propertyType"
        value={form.propertyType}
        onChange={(value) => handleFormChange("propertyType", value)}
        options={propertyTypeOptions}
        error={errors.propertyType}
      />
      <hr className="mt-4 w-full border-gray-300" />

      {/* Search Address */}
      {isLoaded && (
        <MapAutocompleteComponent
          value={addressSearch}
          onChange={setAddressSearch}
          onSelect={(address) => handleSelectAddress(address)}
        />
      )}

      {/* Address Input */}
      {/* Street */}
      <TextField
        label="Street"
        value={form.street}
        placeholder=""
        onChange={(value) => handleFormChange("street", value)}
        error={errors.street}
      />

      <div className="flex gap-4">
        {/* City */}
        <TextField
          label="City"
          value={form.city}
          placeholder=""
          onChange={(value) => handleFormChange("city", value)}
          error={errors.city}
        />

        {/* State */}
        <TextField
          label="State"
          value={form.state}
          placeholder=""
          onChange={(value) => handleFormChange("state", value)}
          error={errors.state}
        />
      </div>

      <div className="flex gap-4">
        {/* Postcode */}
        <TextField
          label="Postcode"
          value={form.postcode.toString()}
          placeholder=""
          onChange={(value) => handleFormChange("postcode", Number(value))}
          error={errors.postcode}
        />

        {/* Longitude */}
        <TextField
          label="Longitude"
          value={form.longitude.toString()}
          placeholder=""
          onChange={(value) => handleFormChange("longitude", Number(value))}
          error={errors.longitude}
        />
      </div>

      <div className="flex gap-4">
        {/* Latitude */}
        <TextField
          label="Latitude"
          value={form.latitude.toString()}
          placeholder=""
          onChange={(value) => handleFormChange("latitude", Number(value))}
          error={errors.latitude}
        />

        {/* Price */}
        <TextField
          label="Price"
          value={form.price.toString()}
          placeholder=""
          onChange={(value) => handleFormChange("price", Number(value))}
          error={errors.price}
        />
      </div>

      <hr className="mt-4 w-full border-gray-300" />
      {/* Basic information */}
      <div className="my-5 w-full">
        <p>Basic Information</p>
        <div className="flex flex-row items-start flex-wrap my-2">
          <div className="flex flex-row gap-4 w-full flex-wrap">
            {/* Bedrooms */}
            <BasicInfoCard
              label="Beds"
              value={form.bedrooms}
              min={0}
              step={1}
              onChange={(value) => handleFormChange("bedrooms", value)}
              icon={<BedSingle color="black" />}
            />

            {/* Baths */}
            <BasicInfoCard
              label="Baths"
              value={form.bathrooms}
              min={0}
              step={1}
              onChange={(value) => handleFormChange("bathrooms", value)}
              icon={<Bath color="black" />}
            />

            {/* Garage */}
            <BasicInfoCard
              label="Garage"
              value={form.garages}
              min={0}
              step={1}
              onChange={(value) => handleFormChange("garages", value)}
              icon={<CarFront color="black" />}
            />

            {/* Area */}
            <BasicInfoCard
              label="Area"
              value={form.floorArea}
              min={0}
              step={1}
              onChange={(value) => handleFormChange("floorArea", value)}
              icon={<Grid2x2 color="black" />}
            />
          </div>
        </div>
      </div>
      <hr className="mt-4 w-full border-gray-300" />

      {/* Save and Cancel buttons */}
      <div className="flex flex-row justify-end gap-4 mt-4 w-full">
        <button
          type="button"
          className="w-20 h-10 bg-blue-400 border border-blue-500 rounded-2xl p-2 text-white hover:bg-blue-500"
          onClick={handleSave}
          disabled={isSaving}
        >
          {isSaving ? "Saving..." : "Save"}
        </button>
      </div>
    </SectionPannelLayout>
  );
}
