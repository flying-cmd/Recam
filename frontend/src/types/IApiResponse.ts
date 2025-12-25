export type IExceptionResponse = {
  detail: string;
  instance: string;
  status: number;
  timestamp: string;
  title: string;
  traceId: string;
  type: string;
}

export type IApiError = {
  status: number;
  title: string;
}

export type IApiResponse<T> = {
  success: boolean;
  data: T;
  message: string;
}