import type { IAgent, ICreateAgent } from "../types/IAgent";
import type { IGetResponse, IPostResponse } from "../types/IApiResponse";
import { service } from "./request";

export const getAgentsUnderPhotographyCompany = async (): Promise<IGetResponse<IAgent[]>> => {
  const res = await service<IGetResponse<IAgent[]>>({
    url: "/user/agentlists",
    method: "get"
  });

  return res.data;
}

export const createAgentByPhotographyCompany = async (form: ICreateAgent): Promise<IPostResponse<ICreateAgent>> => {
  const formData = new FormData();
  formData.append("email", form.email);
  formData.append("agentFirstName", form.agentFirstName);
  formData.append("agentLastName", form.agentLastName);
  formData.append("companyName", form.companyName);
  formData.append("phoneNumber", form.phoneNumber);

  if (form.avatar) {
    formData.append("avatar", form.avatar);
  }
  
  const res = await service<IPostResponse<ICreateAgent>>({
    url: "/user/agent",
    method: "post",
    data: formData,
    headers: {
      "Content-Type": "multipart/form-data"
    }
  });

  return res.data;
}

export const addAgentByAgentId = async (agentId: string): Promise<void> => {
  const res = await service<void>({
    url: `/user/photography-company/agent/${agentId}`,
    method: "put"
  });

  return res.data;
}

export const searchAgentByEmail = async (email: string): Promise<IGetResponse<IAgent>> => {
  const res = await service<IGetResponse<IAgent>>({
    url: `/user/agent/search`,
    method: "get",
    params: {
      email
    }
  });

  return res.data;
}

export const deleteAssignedAgent = async (agentId: string): Promise<void> => {
  await service<void>({
    url: `/user/photography-company/agent/${agentId}`,
    method: "delete"
  });
}