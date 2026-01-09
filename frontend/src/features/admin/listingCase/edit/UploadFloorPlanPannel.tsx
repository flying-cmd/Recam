import { Download, Trash2, Upload } from "lucide-react";
import SectionPannelLayout from "./SectionPannelLayout";
import { useEffect, useRef, useState } from "react";
import {
  deleteMediaFileById,
  getAllMediaByListingCaseId,
  uploadMedia,
} from "../../../../services/media";
import { MediaType, type MediaAsset } from "../../../../types/IMedia";
import Spinner from "../../../../components/Spinner";
import { downloadMediaFileById } from "../../../../services/listingCaseService";

interface UploadFloorPlanPannelProps {
  listingCaseId: number;
  onBack: () => void;
}

export default function UploadFloorPlanPannel({
  listingCaseId,
  onBack,
}: UploadFloorPlanPannelProps) {
  const ref = useRef<HTMLInputElement | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isUploading, setIsUploading] = useState(false);
  const [photos, setPhotos] = useState<MediaAsset[]>([]);
  const [refreshKey, setRefreshKey] = useState(0); // used to refresh to get new photos

  const pickFiles = () => ref.current?.click();

  // Get current photos
  useEffect(() => {
    (async () => {
      try {
        setIsLoading(true);
        const res = await getAllMediaByListingCaseId(listingCaseId);
        if (res.success) {
          setPhotos(
            res.data.filter((media) => media.mediaType === MediaType.FloorPlan)
          );
        } else {
          throw new Error("Failed to get media");
        }
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [listingCaseId, refreshKey]);

  const handleFileChange = async (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    try {
      setIsUploading(true);
      const files = event.target.files;
      if (!files) return;

      // Upload files
      const formData = new FormData();
      for (const file of files) {
        formData.append("mediaFiles", file);
      }
      formData.append("mediaType", MediaType.FloorPlan);
      const uploadRes = await uploadMedia(formData, listingCaseId);

      if (!uploadRes.success) {
        throw new Error("Failed to upload media");
      }

      // Trigger useEffect to refresh to get new photos
      setRefreshKey((prev) => prev + 1);
    } catch (error) {
      console.error(error);
    } finally {
      setIsUploading(false);
    }
  };

  const onDownload = async (mediaAssetId: number) => {
    setIsLoading(true);
    await downloadMediaFileById(mediaAssetId);
    setIsLoading(false);
  };

  const onDelete = async (mediaAssetId: number) => {
    try {
      setIsLoading(true);
      await deleteMediaFileById(mediaAssetId);

      // Trigger useEffect to refresh to get new photos
      setRefreshKey((prev) => prev + 1);
    } catch (error) {
      console.error(error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) return <Spinner />;

  return (
    <SectionPannelLayout title="Floor Plan" onBack={onBack}>
      {/* Button */}
      <div className="flex flex-col items-center justify-center">
        <button
          type="button"
          className="flex flex-row bg-sky-600 text-white h-10 w-40 items-center gap-2 justify-center p-2 text-sm m-3 rounded-md"
          disabled={isUploading}
          onClick={pickFiles}
        >
          <Upload color="white" />
          {isUploading ? "Uploading..." : "Upload Photos"}
        </button>

        {/* Hidden file input */}
        <input
          type="file"
          accept="image/*"
          ref={ref}
          hidden
          onChange={handleFileChange}
        />

        {photos.length === 0 ? (
          <div className="bg-gray-100 text-black/80 h-30 w-full flex flex-col items-center justify-center">
            <p>No photos uploaded yet</p>
          </div>
        ) : (
          <div className="w-full flex flex-row justify-start gap-2 flex-wrap">
            {photos.map((photo) => (
              <div key={photo.id} className="relative">
                {/* Download button */}
                <button
                  type="button"
                  className="absolute top-1 left-1 w-6 h-6 rounded-full bg-blue-600 opacity-80 hover:opacity-100 flex items-center justify-center"
                  onClick={() => onDownload(photo.id)}
                >
                  <Download color="white" size={18} />
                </button>
                {/* Floor plan */}
                <img
                  src={photo.mediaUrl}
                  alt={"Floor plan"}
                  className="w-full h-40 object-cover"
                />
                {/* Delete button */}
                <button
                  type="button"
                  className="absolute top-1 right-1 w-6 h-6 rounded-full bg-red-600 opacity-80 hover:opacity-100 flex items-center justify-center"
                  onClick={() => {
                    onDelete(photo.id);
                  }}
                >
                  <Trash2 color="white" size={18} />
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </SectionPannelLayout>
  );
}
