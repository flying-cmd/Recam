import type { IAgent } from "./IAgent";

export type IOrder = {
  id: number;
  title: string;
  description: string;
  street: string;
  city: string;
  state: string;
  postcode: string;
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

export enum OrderStatus {
  Created,
  Pending,
  Delivered
}

export enum PropertyType {
  House,
  Unit,
  Townhouse,
  Villa,
  Others
}

export enum SaleCategory {
  ForSale,
  ForRent,
  Auction
}

export type MediaAsset = {
  id: number;
  mediaType: string;
  mediaUrl: string;
  uploadedAt: string;
  isSelect: boolean;
  isHero: boolean;
  isDeleted: boolean;
}

export enum MediaType {
  Photo = "Photo",
  Videography = "Videography",
  FloorPlan = "FloorPlan",
  VRTour = "VRTour"
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
  postcode: string;
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