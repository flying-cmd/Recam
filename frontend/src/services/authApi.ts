import type { ILoginRequest, ILoginResponse, IRegisterRequest, IRegisterResponse } from "../types/IAuth";
import { service } from "./request";

export const login = async (params: ILoginRequest): Promise<ILoginResponse> => {
  const res = await service<ILoginResponse>({
    url: "/auth/login",
    method: "post",
    data: params
  });

  return res.data;
}

export const register = async (params: IRegisterRequest): Promise<IRegisterResponse> => {
  const res = await service<IRegisterResponse>({
    url: "/auth/register",
    method: "post",
    data: params,
  });

  return res.data;
}