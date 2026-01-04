import { useState } from "react";

export default function ActionsButton() {
  const [open, setOpen] = useState(false);

  return (
    <div className="relative text-left">
      <button
        type="button"
        className="cursor-pointer font-bold"
        onClick={() => setOpen(!open)}
      >
        ...
      </button>

      {open && (
        <div
          className="absolute top-full mt-2 w-30 border border-gray-300
         bg-white rounded-md shadow-lg overflow-hidden z-50"
        >
          <div className="divide-y divide-gray-300">
            <button
              type="button"
              className="block w-full p-1 text-left cursor-pointer hover:bg-gray-200"
            >
              Edit
            </button>
            <button
              type="button"
              className="block w-full p-1 text-left cursor-pointer hover:bg-gray-200"
            >
              Delete
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
