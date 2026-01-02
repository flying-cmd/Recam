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

type BaseApiResponse = {
  success: boolean;
}

export type IGetResponse<T> = BaseApiResponse & {
  data: T;
}

export type IPostResponse<T> = BaseApiResponse & {
  data: T;
  message: string;
}

export type IPutResponse = BaseApiResponse & {
  message: string;
}

export type IDeleteResponse = BaseApiResponse & {
  message: string;
}