import type { AxiosResponse } from "axios";
import type { IGetResponse, IPostResponse } from "../types/IApiResponse";
import type { IUploadMediaRequest, MediaAsset } from "../types/IMedia";
import { service } from "./request";
import { getFilenameFromDisposition } from "./helper/getFilenameFromDisposition";

export const uploadMedia = async (params: FormData, listingCaseId: number): Promise<IPostResponse<IUploadMediaRequest>> => {
  const res = await service<IPostResponse<IUploadMediaRequest>>({
    url: `/listings/${listingCaseId}/media`,
    method: "post",
    data: params,
    headers: {
      "Content-Type": "multipart/form-data"
    }
  });

  return res.data;
};

export const getAllMediaByListingCaseId = async (listingCaseId: number): Promise<IGetResponse<MediaAsset[]>> => {
  const res = await service<IGetResponse<MediaAsset[]>>({
    url: `/listings/${listingCaseId}/media`,
    method: "get"
  });

  return res.data;
};

export const downloadMediaFileById = async (mediaAssetId: number): Promise<void> => {
  const res: AxiosResponse<Blob> = await service({
    url: `/media/download/${mediaAssetId}`,
    method: "get",
    responseType: "blob"
  });

  const fileName = getFilenameFromDisposition(res.headers["content-disposition"]) ?? `media-${mediaAssetId}.${res.headers["content-type"]?.split("/").pop()}`;

  // Create a temporary URL for the blob
  const url = window.URL.createObjectURL(res.data);

  // Create a temporary link to trigger the download
  const templink = document.createElement("a");
  templink.href = url;
  templink.download = fileName;

  // Append the link to the body and click it to trigger the download
  document.body.appendChild(templink);
  templink.click();

  // Clean up the temporary link and URL
  templink.remove();
  window.URL.revokeObjectURL(url);
};

export const deleteMediaFileById = async (mediaAssetId: number): Promise<void> => {
  await service<void>({
    url: `/media/${mediaAssetId}`,
    method: "delete"
  });
};