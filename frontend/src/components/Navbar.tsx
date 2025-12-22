import { LogOut } from "lucide-react";

export default function Navbar() {
  return (
    <nav className="sticky bg-sky-600">
      <div className="flex flex-row justify-between p-4">
        <div className="flex items-center gap-6">
          <div className="text-2xl font-bold text-white font-caveat">Recam</div>

          <div className="text-white/70 hover:text-white cursor-pointer">
            Orders
          </div>

          <div className="text-white/70 hover:text-white cursor-pointer">
            Clients
          </div>

          <div className="text-white/70 hover:text-white cursor-pointer">
            Staff
          </div>
        </div>

        <LogOut className="cursor-pointer" />
      </div>
    </nav>
  );
}
