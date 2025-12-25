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
  const formData = new FormData();
  formData.append("email", params.email);
  formData.append("password", params.password);
  formData.append("confirmPassword", params.confirmPassword);
  formData.append("firstName", params.firstName);
  formData.append("lastName", params.lastName);
  formData.append("companyName", params.companyName);
  formData.append("avatar", params.avatarFile);
  
  const res = await service<IRegisterResponse>({
    url: "/auth/register",
    method: "post",
    data: formData,
    headers: {
      "Content-Type": "multipart/form-data"
    }
  });

  return res.data;
}