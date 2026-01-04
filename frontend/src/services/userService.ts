import type { IAgent } from "../types/IAgent";
import type { IGetResponse } from "../types/IApiResponse";
import { service } from "./request";

export const getAgentsUnderPhotographyCompany = async (): Promise<IGetResponse<IAgent[]>> => {
  const res = await service<IGetResponse<IAgent[]>>({
    url: "/user/agentlists",
    method: "get"
  });

  return res.data;
}