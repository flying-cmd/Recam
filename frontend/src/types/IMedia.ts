export enum MediaType {
  Photo = "Photo",
  Videography = "Videography",
  FloorPlan = "FloorPlan",
  VRTour = "VRTour"
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

export type IUploadMediaRequest = {
  mediaFiles: File[],
  mediaType: string
}
