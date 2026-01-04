import { LogOut } from "lucide-react";
import { useAuth } from "../hooks/useAuth";
import { IRole } from "../types/IRole";

export default function Navbar() {
  const { user, logout } = useAuth();

  return (
    <nav className="sticky bg-sky-600">
      <div className="flex flex-row justify-between p-4">
        <div className="flex items-center gap-6">
          <div className="text-2xl font-bold text-white font-caveat">Recam</div>

          {user && user.scopes === IRole.PhotographyCompany && (
            <>
              <div className="text-white/70 hover:text-white cursor-pointer">
                Listing Cases
              </div>

              <div className="text-white/70 hover:text-white cursor-pointer">
                Agents
              </div>

              <div className="text-white/70 hover:text-white cursor-pointer">
                Photography Companies
              </div>
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
