import { useState } from "react";

interface AuthorizedProps {
  authorized: React.ReactNode;
  unauthorized: React.ReactNode;
}

export default function Authorized(props: AuthorizedProps) {
  const [authorized, setAuthorized] = useState(false);

  return <>{authorized ? props.authorized : props.unauthorized}</>;
}
