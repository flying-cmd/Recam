import type { AxiosResponse } from "axios";
import type { IDeleteResponse, IGetResponse, IPostResponse } from "../types/IApiResponse";
import type { IListingCaseDetails, IListingCase, ICreateListingCase, IUpdateListingCase } from "../types/IListingCase";
import type { IPagedResponse } from "../types/IPagedResponse";
import { service } from "./request";
import { getFilenameFromDisposition } from "./helper/getFilenameFromDisposition";
import type { IAgent } from "../types/IAgent";

export const getAllListingCases = async (pageNumber: number = 1, pageSize: number = 10): Promise<IGetResponse<IPagedResponse<IListingCase[]>>> => {
  const res = await service<IGetResponse<IPagedResponse<IListingCase[]>>>({
    url: "/listings",
    method: "get",
    params: {
      pageNumber,
      pageSize
    }
  });

  return res.data;
}

export const getListingCaseDetailsById = async (id: number): Promise<IGetResponse<IListingCaseDetails>> => {
  const res = await service<IGetResponse<IListingCaseDetails>>({
    url: `/listings/${id}`,
    method: "get"
  });

  return res.data;
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

export const createListingCase = async (params: ICreateListingCase): Promise<IPostResponse<ICreateListingCase>> => {
  const res = await service<IPostResponse<ICreateListingCase>>({
    url: "/listings",
    method: "post",
    data: params
  });

  return res.data;
}

export const deleteListingCaseById = async (id: number): Promise<IDeleteResponse> => {
  const res = await service<IDeleteResponse>({
    url: `/listings/${id}`,
    method: "delete"
  });

  return res.data;
}

export const updateListingCaseById = async (id: number, params: IUpdateListingCase): Promise<void> => {
  await service<void>({
    url: `/listings/${id}`,
    method: "put",
    data: params
  });
}

export const getListingCaseByUserId = async (pageNumber: number = 1, pageSize: number = 10): Promise<IGetResponse<IPagedResponse<IListingCaseDetails[]>>> => {
  const res = await service<IGetResponse<IPagedResponse<IListingCaseDetails[]>>>({
    url: `/listings`,
    method: "get",
    params: {
      pageNumber,
      pageSize
    }
  });

  return res.data;
}

export const getAssignedAgentsByListingCaseId = async (listingCaseId: number): Promise<IGetResponse<IAgent[]>> => {
  const res = await service<IGetResponse<IAgent[]>>({
    url: `/listings/${listingCaseId}/assigned-agent`,
    method: "get"
  });

  return res.data;
}

export const assignAgentToListingCase = async (listingCaseId: number, agentId: string): Promise<void> => {
  await service<void>({
    url: `/listings/${listingCaseId}/assigned-agent`,
    method: "put",
    params: {
      agentId
    }
  });
}

export const deleteAgentFromListingCase = async (listingCaseId: number, agentId: string): Promise<void> => {
  await service<void>({
    url: `/listings/${listingCaseId}/assigned-agent`,
    method: "delete",
    params: {
      agentId
    }
  });
}