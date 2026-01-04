import { Plus } from "lucide-react";
import { useRef, useState } from "react";

interface AvatarUploadProps {
  label?: string;
  onChange: (file: File | null) => void;
  error?: string;
}

export default function AvatarUpload({
  label,
  onChange,
  error,
}: AvatarUploadProps) {
  const [previewUrl, setPreviewUrl] = useState<string | undefined>(undefined);
  const ref = useRef<HTMLInputElement | null>(null);

  const openPicker = () => ref.current?.click();

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      const file = e.target.files[0];
      setPreviewUrl(URL.createObjectURL(file));
      onChange(file);
    }
  };

  return (
    <div className="w-full">
      <p className="block mb-2">{label}</p>

      <div className="flex items-center gap-4">
        {/* Avatar circle */}
        <button
          type="button"
          className="h-16 w-16 rounded-full bg-gray-200 flex items-center justify-center"
          onClick={openPicker}
          disabled={previewUrl !== undefined}
        >
          {previewUrl ? (
            <img
              src={previewUrl}
              alt="Avatar preview"
              className="w-full h-full object-cover"
            />
          ) : (
            <Plus className="w-6 h-6 text-gray-500" />
          )}
        </button>

        {/* Upload button */}
        <button
          type="button"
          className="px-4 py-2 border border-gray-200 rounded-md bg-white hover:bg-gray-200"
          onClick={openPicker}
        >
          Upload Image
        </button>

        {/* Hidden file input */}
        <input
          ref={ref}
          type="file"
          accept="image/*"
          className="hidden"
          onChange={handleFileChange}
        />
      </div>

      {error && <p className="text-red-500 mt-2">{error}</p>}
    </div>
  );
}
