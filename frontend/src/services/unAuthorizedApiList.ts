const base_url = import.meta.env.VITE_BACKEND_URL

export const unAuthorizedApiList: string[] = [
  `${base_url}/auth/login`,
  `${base_url}/auth/register`,
  `${base_url}/auth/admin/login`,
];