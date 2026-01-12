import axios, { type AxiosInstance, type AxiosResponse, type InternalAxiosRequestConfig } from "axios";
import { unAuthorizedApiList } from "./unAuthorizedApiList";
import type { IApiError, IExceptionResponse } from "../types/IApiResponse";

export const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_BACKEND_URL,
  timeout: 15000
});

// Add a request interceptor
service.interceptors.request.use(function (config: InternalAxiosRequestConfig) {
    // Do something before request is sent
    // Set the authorization header
    const token = localStorage.getItem("token");
    if (!unAuthorizedApiList.includes(config.url!) && token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  }, function (error) {
    // Do something with request error
    return Promise.reject(error);
  }
);

// Add a response interceptor
service.interceptors.response.use(function onFulfilled(response: AxiosResponse) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response;
  }, function onRejected(error) {
    // Any status codes that falls outside the range of 2xx cause this function to trigger
    // Do something with response error
    if (axios.isAxiosError(error)) {
      const data: IExceptionResponse = error.response?.data;
      const apiError: IApiError = {
        status: data.status,
        title: data.title
      };
      throw new Error(apiError.title);
      // return Promise.reject(apiError);
    }

    return Promise.reject({ title: "Internal Server Error", status: 500 } as IApiError);
  });