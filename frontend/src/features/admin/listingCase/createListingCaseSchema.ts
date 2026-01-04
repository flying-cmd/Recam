import { z } from "zod";
import { PropertyType, SaleCategory } from "../../../types/IListingCase";

export const createListingCaseSchema = z.object({
  title: z.string().min(1, "Title is required."),
  description: z.string().min(1, "Description is required."),
  street: z.string().min(1, "Street is required."),
  city: z.string().min(1, "City is required."),
  state: z.string().min(1, "State is required."),
  postcode: z.number().min(1, "Postcode is required.").int("Postcode must be an integer."),
  longitude: z.number().min(1, "Longitude is required."),
  latitude: z.number().min(1, "Latitude is required."),
  price: z.number().min(0, "Price must be greater than or equal to 0."),
  bedrooms: z.number().min(0, "Bedrooms must be greater than or equal to 0.").int("Bedrooms must be an integer."),
  bathrooms: z.number().min(0, "Bathrooms must be greater than or equal to 0.").int("Bathrooms must be an integer."),
  garages: z.number().min(0, "Garages must be greater than or equal to 0.").int("Garages must be an integer."),
  floorArea: z.number().min(0, "Floor area must be greater than or equal to 0."),
  propertyType: z.enum(PropertyType, {message: "Please select a property type."}),
  saleCategory: z.enum(SaleCategory, {message: "Please select a sale category."}),
});