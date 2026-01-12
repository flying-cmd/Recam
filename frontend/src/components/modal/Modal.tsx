import { X } from "lucide-react";

interface ModalProps {
  open: boolean;
  onClose: () => void;
  title?: string;
  description?: string;
  children?: React.ReactNode;
}

export default function Modal({
  open,
  onClose,
  title,
  description,
  children,
}: ModalProps) {
  if (!open) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-99">
      {/* Overlay */}
      <div className="absolute inset-0 bg-black/50" onClick={onClose}></div>

      {/* Modal */}
      <div className="absolute inset-0 flex items-center justify-center">
        <div
          role="dialog"
          className="bg-white rounded-lg shadow-lg w-full max-w-3xl max-h-[80vh] flex flex-col overflow-y-auto p-4"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="sticky top-0 z-10 bg-white flex flex-col justify-between">
            <div className="flex justify-between w-full">
              <div></div>
              <button
                type="button"
                className="hover:cursor-pointer"
                onClick={onClose}
              >
                <X color="black" />
              </button>
            </div>

            <div className="w-full">
              {title && <h3 className="text-2xl font-bold">{title}</h3>}
              {description && <p>{description}</p>}
            </div>

            <hr className="mt-4 w-full border-gray-300" />
          </div>

          {/* Body */}
          <div className="w-full">{children}</div>
        </div>
      </div>
    </div>
  );
}
