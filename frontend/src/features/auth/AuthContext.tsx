import { createContext } from "react";

class JwtUserInfo {
  constructor(public id: string, public email: string, public scopes: string) {}
}

type AuthContextType = {
  token: string | null;
  user: JwtUserInfo | null;
  login: (token: string) => void;
  logout: () => void;
  isLoading: boolean;
};

// Create an empty context object that will be used to store the authentication state and functions between components
export const AuthContext = createContext<AuthContextType | undefined>(
  undefined
);
