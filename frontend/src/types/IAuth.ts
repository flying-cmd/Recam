import type { IPostResponse } from "./IApiResponse";

export type ILoginRequest = {
  email: string;
  password: string;
}

export type ILoginResponse = IPostResponse<string>;

export type IRegisterRequest = {
  photographyCompanyName: string;
  email: string;
  phoneNumber: string;
  password: string;
  confirmPassword: string;
}

export type IRegisterResponse = IPostResponse<string>;