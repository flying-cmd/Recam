import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import Spinner from "./Spinner";

interface RoleBasedRouteProps {
  children: React.ReactNode;
  scopes: string;
}

export default function RoleBasedRoute({
  children,
  scopes,
}: RoleBasedRouteProps) {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <Spinner />;
  }

  if (!user?.scopes) {
    return <Navigate to="/login" replace />;
  }

  // Redirects authenticated users with the wrong role to an /unauthorized page
  if (user.scopes !== scopes) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
}
