import { Bath, BedSingle, CarFront, Grid2x2 } from "lucide-react";

interface HeroProps {
  imageUrl: string;
  title: string;
  description: string;
  street: string;
  city: string;
  state: string;
  postcode: string;
  bedrooms: number;
  bathrooms: number;
  garages: number;
  floorArea: number;
  propertyType: string;
  saleCategory: string;
}

export default function Hero({
  imageUrl,
  title,
  description,
  street,
  city,
  state,
  postcode,
  bedrooms,
  bathrooms,
  garages,
  floorArea,
  propertyType,
  saleCategory,
}: HeroProps) {
  return (
    <>
      <div className="flex md::w-full md:h-full h-70">
        <div className="w-3/5">
          <img
            src={imageUrl}
            alt="hero image"
            className="w-full h-full object-fill"
          />
        </div>

        <div className="bg-orange-950 w-2/5 text-white flex flex-col items-center">
          <div className="border rounded-md md:mt-10 mt-8 p-1 md:text-base text-sm">
            {propertyType.toUpperCase()}{" "}
            {saleCategory === "ForSale"
              ? "For Sale".toUpperCase()
              : saleCategory === "ForRent"
              ? "For Rent".toUpperCase()
              : "Auction".toUpperCase()}
          </div>

          <div className="mt-5">
            <h1 className="text-3xl font-bold text-center">{street}</h1>
            <h2 className="text-center">
              {city}, {state} {postcode}
            </h2>
          </div>

          <div className="md:mt-10 mt-5 font-bold">___</div>

          <div className="flex md:mt-5 mt-2">
            <div className="md:m-2">
              <BedSingle size={28} strokeWidth={1.75} />
              <p className="text-center md:text-sm text-xs">{bedrooms} Beds</p>
            </div>

            <div className="md:m-2">
              <Bath size={28} strokeWidth={1.75} />
              <p className="text-center md:text-sm text-xs">
                {bathrooms} Baths
              </p>
            </div>

            <div className="md:m-2">
              <CarFront size={28} strokeWidth={1.75} />
              <p className="text-center md:text-sm text-xs">
                {garages} Garages
              </p>
            </div>

            <div className="md:m-2">
              <Grid2x2 size={28} strokeWidth={1.75} />
              <p className="text-center md:text-sm text-xs">
                {floorArea} m<sup>2</sup>
              </p>
            </div>
          </div>
        </div>
      </div>

      <div>
        <h1 className="text-3xl font-semibold mt-5 text-center">{title}</h1>
        <p className="mt-2 text-center">{description}</p>
      </div>

      <div className="flex justify-center mt-3">
        <button
          type="button"
          className="text-sm font-medium underline underline-offset-2 hover:text-blue-600 cursor-pointer"
        >
          Click to add
        </button>
      </div>
    </>
  );
}
