import { jwtDecode, type JwtPayload } from "jwt-decode";
import { useEffect, useState } from "react";
import { AuthContext } from "./AuthContext";
import { IRole } from "../../types/IRole";

type AppJwtPayload = JwtPayload & {
  email?: string;
  scopes?: string;
  agentFirstName?: string;
  agentLastName?: string;
};

class JwtUserInfo {
  constructor(
    public id: string,
    public email: string,
    public scopes: string,
    public agentFirstName?: string,
    public agentLastName?: string
  ) {}
}

// Serve as the provider for the AuthContext
// It receives the children prop, which represents that the child components that will have access to the authentication context
const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [token, setToken] = useState<string | null>(null);
  const [user, setUser] = useState<JwtUserInfo | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const login = (token: string) => {
    localStorage.setItem("token", token);
    const decodedToken: AppJwtPayload = jwtDecode(token);
    setToken(token);
    if (decodedToken.scopes === IRole.PhotographyCompany) {
      setUser(
        new JwtUserInfo(
          decodedToken.sub ?? "",
          decodedToken.email ?? "",
          decodedToken.scopes ?? ""
        )
      );
    } else if (decodedToken.scopes === IRole.Agent) {
      setUser(
        new JwtUserInfo(
          decodedToken.sub ?? "",
          decodedToken.email ?? "",
          decodedToken.scopes ?? "",
          decodedToken.agentFirstName ?? "",
          decodedToken.agentLastName ?? ""
        )
      );
    }
  };

  const logout = () => {
    localStorage.removeItem("token");
    setToken(null);
    setUser(null);
  };

  useEffect(() => {
    const savedToken = localStorage.getItem("token");
    if (savedToken) {
      const decodedToken: AppJwtPayload = jwtDecode(savedToken);

      // Check if the token is expired
      try {
        if (!decodedToken.exp) {
          throw new Error("Invalid token");
        }
        const isExpired = Date.now() >= decodedToken.exp * 1000;
        if (isExpired) {
          logout();
        } else {
          setToken(savedToken);
          if (decodedToken.scopes === IRole.PhotographyCompany) {
            setUser(
              new JwtUserInfo(
                decodedToken.sub ?? "",
                decodedToken.email ?? "",
                decodedToken.scopes ?? ""
              )
            );
          } else if (decodedToken.scopes === IRole.Agent) {
            setUser(
              new JwtUserInfo(
                decodedToken.sub ?? "",
                decodedToken.email ?? "",
                decodedToken.scopes ?? "",
                decodedToken.agentFirstName ?? "",
                decodedToken.agentLastName ?? ""
              )
            );
          }
        }
      } catch {
        logout(); // Invalid token
      }
    }

    setIsLoading(false);
  }, []);

  useEffect(() => {
    if (token) {
      const decodedToken: AppJwtPayload = jwtDecode(token);

      if (!decodedToken.exp) {
        logout(); // Invalid token
      } else {
        const expriesIn = decodedToken.exp * 1000 - Date.now();
        const timer = setTimeout(() => {
          logout();
          alert("Token expired. Please login again.");
        }, expriesIn);

        return () => clearTimeout(timer);
      }
    }
  }, [token]);

  return (
    <AuthContext.Provider value={{ token, user, login, logout, isLoading }}>
      {children}
    </AuthContext.Provider>
  );
};

export default AuthProvider;
