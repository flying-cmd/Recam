import type { AxiosResponse } from "axios";
import type { IGetResponse } from "../types/IApiResponse";
import type { IListingCaseDetails, IOrder } from "../types/IOrder";
import type { IPagedResponse } from "../types/IPagedResponse";
import { service } from "./request";

export const getAllOrders = async (pageNumber: number = 1, pageSize: number = 10): Promise<IGetResponse<IPagedResponse<IOrder[]>>> => {
  const res = await service<IGetResponse<IPagedResponse<IOrder[]>>>({
    url: "/listings",
    method: "get",
    params: {
      pageNumber,
      pageSize
    }
  });

  return res.data;
}

export const getListingCaseDetailsById = async (id: string): Promise<IGetResponse<IListingCaseDetails>> => {
  const res = await service<IGetResponse<IListingCaseDetails>>({
    url: `/listings/${id}`,
    method: "get"
  });

  return res.data;
}

const getFilenameFromDisposition = (disposition?: string) => {
  if (!disposition) return null;

  // filename*=UTF-8''...
  const utf8Match = disposition.match(/filename\*\s*=\s*UTF-8''([^;]+)/i);
  if (utf8Match?.[1]) return decodeURIComponent(utf8Match[1]);

  // filename="..."
  const quotedMatch = disposition.match(/filename\s*=\s*"([^"]+)"/i);
  if (quotedMatch?.[1]) return quotedMatch[1];

  // filename=...
  const plainMatch = disposition.match(/filename\s*=\s*([^;]+)/i);
  if (plainMatch?.[1]) return plainMatch[1].trim();

  return null;
}

export const downloadAllMediaFiles = async (listingCaseId: string): Promise<void> => {
  const res: AxiosResponse<Blob> = await service({
    url: `/listings/${listingCaseId}/download`,
    method: "get",
    responseType: "blob"
  });

  const fileName = getFilenameFromDisposition(res.headers["content-disposition"]) ?? `listing-case-${listingCaseId}.zip`;
  
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
}

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
}