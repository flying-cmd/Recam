import { Download } from "lucide-react";
import { downloadMediaFileById } from "../../services/listingCaseService";
import { useState } from "react";
import type { MediaAsset } from "../../types/IMedia";

interface PropertyImagesProps {
  selectedImagesAsset: MediaAsset[];
}

export default function PropertyImages({
  selectedImagesAsset,
}: PropertyImagesProps) {
  const [isDownloading, setIsDownloading] = useState(false);

  const handleDownload = async (mediaAssetId: number) => {
    try {
      setIsDownloading(true);
      await downloadMediaFileById(mediaAssetId);
    } catch (error) {
      console.error("Error downloading media file:", error);
    } finally {
      setIsDownloading(false);
    }
  };

  return (
    <>
      <h1 className="text-center font-bold text-2xl mt-20">Image</h1>

      <div className="m-1">
        <h3 className="text-blue-950">
          Images {`(${selectedImagesAsset.length})`}
        </h3>
      </div>

      <div className="flex">
        {selectedImagesAsset.map((asset) => (
          <div key={asset.id} className="m-1 md:w-1/4 md:h-1/4 w-1/3 h-1/3">
            {/* aspect-[16/9] makes every card the same shape */}
            <div className="w-full aspect-16/10">
              <img
                src={asset.mediaUrl}
                alt="property image"
                className="w-full h-full object-fill"
                loading="lazy"
              />
            </div>

            <div className="flex justify-end">
              <button
                type="button"
                className="inline-flex cursor-pointer"
                onClick={() => handleDownload(asset.id)}
                disabled={isDownloading}
              >
                <Download size={13} strokeWidth={1.75} />
                <span className="text-xs text-gray-500 hover:text-black">
                  {isDownloading ? "Downloading..." : "Download"}
                </span>
              </button>
            </div>
          </div>
        ))}
      </div>
    </>
  );
}
