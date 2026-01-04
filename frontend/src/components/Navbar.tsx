import { LogOut } from "lucide-react";
import { useAuth } from "../hooks/useAuth";
import { IRole } from "../types/IRole";
import { NavLink } from "react-router-dom";

const baseLink = "text-white/70 hover:text-white cursor-pointer p-1";
const activeLink = "border-none rounded-md bg-blue-600 text-white/90";

export default function Navbar() {
  const { user, logout } = useAuth();

  const linkClassName = ({ isActive }: { isActive: boolean }) =>
    `${baseLink} ${isActive ? activeLink : null}`;

  return (
    <nav className="sticky bg-sky-600">
      <div className="flex flex-row justify-between p-4">
        <div className="flex items-center gap-6">
          <div className="text-2xl font-bold text-white font-caveat">Recam</div>

          {user && user.scopes === IRole.PhotographyCompany && (
            <>
              <NavLink to="/listings" className={linkClassName}>
                Listing Cases
              </NavLink>

              <NavLink to="/agents" className={linkClassName}>
                Agents
              </NavLink>

              {/* <div className="text-white/70 hover:text-white cursor-pointer">
                Photography Companies
              </div> */}
            </>
          )}

          {/* {user && user.scopes === IRole.Agent && (
            <>
              <div className="text-white/70 hover:text-white cursor-pointer">
                Listing Cases
              </div>

              <div className="text-white/70 hover:text-white cursor-pointer">
                Agents
              </div>
            </>
          )} */}
        </div>

        {user && <LogOut className="cursor-pointer" onClick={logout} />}
      </div>
    </nav>
  );
}
