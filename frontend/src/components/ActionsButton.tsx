import { useState } from "react";

interface ActionsButtonProps {
  onEdit: () => void;
  onDelete: () => void;
}

export default function ActionsButton({
  onEdit,
  onDelete,
}: ActionsButtonProps) {
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
              onClick={() => {
                setOpen(false);
                onEdit();
              }}
            >
              Edit
            </button>
            <button
              type="button"
              className="block w-full p-1 text-left cursor-pointer hover:bg-gray-200"
              onClick={() => {
                setOpen(false);
                onDelete();
              }}
            >
              Delete
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
