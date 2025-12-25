import type { IApiResponse } from "./IApiResponse";

export type ILoginRequest = {
  email: string;
  password: string;
}

export type ILoginResponse = IApiResponse<string>;

export type IRegisterRequest = {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  companyName: string;
  avatarFile: File;
}

export type IRegisterResponse = IApiResponse<string>;