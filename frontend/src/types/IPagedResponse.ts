export type IPagedResponse<T> = {
  items: T;
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};