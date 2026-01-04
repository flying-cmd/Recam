import { Autocomplete } from "@react-google-maps/api";
import { useRef } from "react";
import type { IAddress } from "../../types/IListingCase";

interface MapAutocompleteComponentProps {
  value: string;
  onChange: (val: string) => void;
  onSelect: (address: IAddress) => void;
}

export default function MapAutocompleteComponent({
  value,
  onChange,
  onSelect,
}: MapAutocompleteComponentProps) {
  const inputRef = useRef<google.maps.places.Autocomplete | null>(null);

  const parseAddress = (place: google.maps.places.PlaceResult): IAddress => {
    const comps = place.address_components ?? [];

    const street =
      comps.find((c) => c.types.includes("route"))?.long_name ?? "";
    const city =
      comps.find((c) => c.types.includes("locality"))?.long_name ?? "";
    const state =
      comps.find((c) => c.types.includes("administrative_area_level_1"))
        ?.long_name ?? "";
    const postcode =
      comps.find((c) => c.types.includes("postal_code"))?.long_name ?? "";
    const latitude = place.geometry?.location?.lat?.() ?? 0;
    const longitude = place.geometry?.location?.lng?.() ?? 0;

    return {
      street,
      city,
      state,
      postcode: Number(postcode),
      longitude,
      latitude,
    };
  };

  const onPlaceChanged = () => {
    const place = inputRef.current?.getPlace();
    if (!place) return;

    if (place.formatted_address) {
      onChange(place.formatted_address);
    }

    const address = parseAddress(place);
    onSelect(address);
  };

  return (
    <div className="w-full pt-6 pb-2">
      <label className="block mb-2">Search Address</label>
      <Autocomplete
        onLoad={(ref) => {
          inputRef.current = ref;
          // restrict to AU and address only
          ref.setOptions({
            types: ["address"],
            componentRestrictions: { country: "au" },
          });
        }}
        onPlaceChanged={onPlaceChanged}
      >
        <input
          type="text"
          className="w-full border border-gray-300 rounded-md p-2"
          placeholder="Enter an address"
          value={value}
          onChange={(e) => onChange(e.target.value)}
        />
      </Autocomplete>
    </div>
  );
}
