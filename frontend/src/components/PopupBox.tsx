import { X } from "lucide-react";

interface PopupBoxProps {
  message: string;
  open: boolean;
  onClose: () => void;
}

export default function PopupBox({ message, open, onClose }: PopupBoxProps) {
  if (!open) {
    return;
  }

  return (
    // fill the full screen
    <div
      className="fixed inset-0 z-100 flex justify-center items-center bg-black/40"
      role="dialog"
      aria-modal="true"
      // onClick={onClose} // close on click outside
    >
      {/* Popup box */}
      <div
        className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg"
        onClick={(e) => e.stopPropagation()} // Prevent bubble up
      >
        <div className="flex justify-end mb-2">
          <X className="hover:cursor-pointer" onClick={onClose} />
        </div>

        <p>{message}</p>

        <div className="mt-3 flex justify-end">
          <button
            type="button"
            className="rounded bg-blue-500 px-3 py-2 text-white hover:bg-blue-700"
            onClick={onClose}
          >
            OK
          </button>
        </div>
      </div>
    </div>
  );
}
