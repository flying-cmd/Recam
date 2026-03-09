import type { IApiError } from "../types/IApiResponse";

export const isApiError = (error: unknown): error is IApiError => {
  return (
    typeof error === "object" &&
    error !== null &&
    "status" in error &&
    "title" in error
  );
};

export const getApiErrorMessage = (error: IApiError): string => {
  if (error.isNetworkError || error.status === 0) {
    return "Cannot connect to server. Please try again.";
  }

  if (error.status === 401) {
    return "Your session has expired. Please log in again.";
  }

  if (error.status === 403) {
    return "You do not have permission for this action.";
  }

  return error.title || "Something went wrong.";
};

export const getErrorMessage = (error: unknown): string => {
  if (isApiError(error)) {
    return getApiErrorMessage(error);
  }

  if (error instanceof Error) {
    return error.message;
  }

  return "Something went wrong.";
};
