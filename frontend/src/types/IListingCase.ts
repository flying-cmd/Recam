import type { IAgent } from "./IAgent";
import type { MediaAsset } from "./IMedia";

export type IListingCase = {
  id: number;
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
  createdAt: string;
  isDeleted: boolean;
  propertyType: string;
  saleCategory: string;
  listingCaseStatus: string;
  shareUrl: string;
  agents: IAgent[];
}

export type IAddress = {
  street: string;
  city: string;
  state: string;
  postcode: number;
  longitude: number;
  latitude: number;
}

export enum ListingCaseStatus {
  Created = "Created",
  Pending = "Pending",
  Delivered = "Delivered"
}

export enum PropertyType {
  House = "House",
  Unit = "Unit",
  Townhouse = "Townhouse",
  Villa = "Villa",
  Others = "Others"
}

export enum SaleCategory {
  ForSale = "ForSale",
  ForRent = "ForRent",
  Auction = "Auction"
}

export type CaseContact = {
  contactId: string;
  firstName: string;
  lastName: string;
  companyName: string;
  profileImage: string;
  email: string;
  phoneNumber: string;
}

export type IListingCaseDetails = {
  id: number;
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
  createdAt: string;
  isDeleted: boolean;
  propertyType: string;
  saleCategory: string;
  listingCaseStatus: string;
  shareUrl: string;
  userId: string;
  mediaAssets: MediaAsset[];
  caseContacts: CaseContact[];
}

export interface ICreateListingCase {
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
  userId: string;
}

export interface IUpdateListingCase {
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