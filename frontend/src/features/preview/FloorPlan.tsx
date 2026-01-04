import { Download } from "lucide-react";
import type { MediaAsset } from "../../types/IListingCase";
import { useState } from "react";
import { downloadMediaFileById } from "../../services/listingCaseService";

interface FloorPlanProps {
  floorPlanAsset: MediaAsset[];
}

export default function FloorPlan({ floorPlanAsset }: FloorPlanProps) {
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
    <div className="flex flex-col items-center">
      <div>
        <h1 className="text-center font-bold text-2xl mt-20">Floor Plan</h1>
      </div>

      <div className="w-full m-1">
        <h3 className="text-left text-blue-950 m-1">{`Floor Plan (${floorPlanAsset.length})`}</h3>
      </div>

      <div className="flex flex-col items-center justify-center">
        {floorPlanAsset.map((asset) => (
          <div key={asset.id}>
            <div className="h-150 w-full">
              <img
                src={asset.mediaUrl}
                alt="property image"
                className="w-full h-full object-fill"
                loading="lazy"
              />
            </div>

            <div className="flex justify-center">
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
    </div>
  );
}
