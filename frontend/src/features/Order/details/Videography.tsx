import { Download } from "lucide-react";
import type { MediaAsset } from "../../../types/IOrder";
import { useState } from "react";
import { downloadMediaFileById } from "../../../services/orderService";

interface VideographyProps {
  videographyAsset: MediaAsset[];
}

export default function Videography({ videographyAsset }: VideographyProps) {
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
      <h1 className="text-center font-bold text-2xl m-10">Videography</h1>

      <div className="w-full">
        <h3 className="text-left text-blue-950 m-1">{`Videography (${videographyAsset.length})`}</h3>
      </div>

      {videographyAsset.map((asset) => (
        <div>
          <div key={asset.id}>
            <video
              controls
              className="md:w-150 md:h-full w-80 h-60 object-cover"
            >
              <source
                src={asset.mediaUrl}
                type={`video/${asset.mediaUrl.split(".").pop()}`}
              />
            </video>
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

      {/* <div className="flex justify-end">
        <button type="button" className="inline-flex cursor-pointer">
          <Download size={13} strokeWidth={1.75} />
          <span className="text-xs text-gray-500 hover:text-black">
            Download
          </span>
        </button>
      </div> */}
    </div>
  );
}
