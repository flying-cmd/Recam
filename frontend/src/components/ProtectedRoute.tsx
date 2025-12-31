import { Navigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";
import Spinner from "./Spinner";

export default function ProtectedRoute({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, isLoading } = useAuth();

  if (isLoading) {
    return <Spinner />;
  }

  console.log(user);

  if (!user?.scopes) {
    return <Navigate to="/login" replace />;
  }

  return children;
}
