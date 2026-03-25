export interface UserFilter {
  search?: string;
  isActive?: boolean | null;
  page: number;
  pageSize: number;
}
