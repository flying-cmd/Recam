import { ChevronLeft, Download, ExternalLink, Pencil } from "lucide-react";
import ActionButton from "./preview/ActionButton";
import { useNavigate, useParams } from "react-router-dom";
import { downloadAllMediaFiles } from "../services/listingCaseService";
import { useState } from "react";

export default function TopBar() {
  const { listingCaseId } = useParams<{ listingCaseId: string }>();
  const [isDownloading, setIsDownloading] = useState(false);
  const navigate = useNavigate();

  const handleDownload = async () => {
    if (!listingCaseId) return;

    try {
      setIsDownloading(true);
      await downloadAllMediaFiles(listingCaseId);
    } catch (error) {
      console.error(error);
    } finally {
      setIsDownloading(false);
    }
  };

  return (
    <div className="flex justify-around items-center">
      <button
        type="button"
        className="inline-flex hover:cursor-pointer hover:text-sky-600"
        onClick={() => navigate(-1)}
      >
        <ChevronLeft />
        BACK
      </button>

      <div>
        <ActionButton icon={<Pencil size={16} className="mx-1" />}>
          Edit
        </ActionButton>
        <ActionButton icon={<ExternalLink size={16} className="mx-1" />}>
          Copy Shareable Link
        </ActionButton>
        <ActionButton
          icon={<Download size={16} className="mx-1" />}
          style={"bg-blue-400 text-white hover:bg-blue-600 cursor-pointer"}
          onClick={handleDownload}
          disabled={isDownloading}
        >
          {isDownloading ? "Downloading..." : "Download All Files"}
        </ActionButton>
      </div>
    </div>
  );
}
