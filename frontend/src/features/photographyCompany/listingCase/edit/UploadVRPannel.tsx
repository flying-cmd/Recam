// import { Download, Trash2, Upload } from "lucide-react";
// import SectionPannelLayout from "./SectionPannelLayout";
// import { useEffect, useRef, useState } from "react";
// import {
//   deleteMediaFileById,
//   getAllMediaByListingCaseId,
//   uploadMedia,
// } from "../../../../services/media";
// import { MediaType, type MediaAsset } from "../../../../types/IMedia";
// import Spinner from "../../../../components/Spinner";
// import { downloadMediaFileById } from "../../../../services/listingCaseService";

// interface UploadVRPannelProps {
//   listingCaseId: number;
//   onBack: () => void;
// }

// export default function UploadVRPannel({
//   listingCaseId,
//   onBack,
// }: UploadVRPannelProps) {
//   const ref = useRef<HTMLInputElement | null>(null);
//   const [isLoading, setIsLoading] = useState(true);
//   const [isUploading, setIsUploading] = useState(false);
//   const [vrfiles, setVrfiles] = useState<MediaAsset[]>([]);
//   const [refreshKey, setRefreshKey] = useState(0); // used to refresh to get new photos

//   const pickFiles = () => ref.current?.click();

//   // Get current photos
//   useEffect(() => {
//     (async () => {
//       try {
//         setIsLoading(true);
//         const res = await getAllMediaByListingCaseId(listingCaseId);
//         if (res.success) {
//           setVrfiles(
//             res.data.filter((media) => media.mediaType === MediaType.VRTour)
//           );
//         } else {
//           throw new Error("Failed to get media");
//         }
//       } catch (error) {
//         console.error(error);
//       } finally {
//         setIsLoading(false);
//       }
//     })();
//   }, [listingCaseId, refreshKey]);

//   const handleFileChange = async (
//     event: React.ChangeEvent<HTMLInputElement>
//   ) => {
//     try {
//       setIsUploading(true);
//       const files = event.target.files ? Array.from(event.target.files) : [];
//       if (files.length === 0) return;

//       const invalidFile = files.find(
//         (file) => !file.name.toLowerCase().endsWith(".gltf")
//       );
//       if (invalidFile) {
//         alert("Invalid file format. Only .gltf files are allowed.");
//         return;
//       }

//       // Upload files
//       const formData = new FormData();
//       for (const file of files) {
//         formData.append("mediaFiles", file);
//       }
//       formData.append("mediaType", MediaType.VRTour);
//       const uploadRes = await uploadMedia(formData, listingCaseId);

//       if (!uploadRes.success) {
//         throw new Error("Failed to upload media");
//       }

//       // Trigger useEffect to refresh to get new photos
//       setRefreshKey((prev) => prev + 1);
//     } catch (error) {
//       console.error(error);
//     } finally {
//       setIsUploading(false);
//     }
//   };

//   const onDownload = async (mediaAssetId: number) => {
//     setIsLoading(true);
//     await downloadMediaFileById(mediaAssetId);
//     setIsLoading(false);
//   };

//   const onDelete = async (mediaAssetId: number) => {
//     try {
//       setIsLoading(true);
//       await deleteMediaFileById(mediaAssetId);

//       // Trigger useEffect to refresh to get new photos
//       setRefreshKey((prev) => prev + 1);
//     } catch (error) {
//       console.error(error);
//     } finally {
//       setIsLoading(false);
//     }
//   };

//   if (isLoading) return <Spinner />;

//   return (
//     <SectionPannelLayout title="Photography" onBack={onBack}>
//       {/* Button */}
//       <div className="flex flex-col items-center justify-center">
//         <button
//           type="button"
//           className="flex flex-row bg-sky-600 text-white h-10 w-40 items-center gap-2 justify-center p-2 text-sm m-3 rounded-md"
//           disabled={isUploading}
//           onClick={pickFiles}
//         >
//           <Upload color="white" />
//           {isUploading ? "Uploading..." : "Upload Photos"}
//         </button>

//         {/* Hidden file input */}
//         <input
//           type="file"
//           accept=".gltf"
//           ref={ref}
//           hidden
//           onChange={handleFileChange}
//         />

//         {vrfiles.length === 0 ? (
//           <div className="bg-gray-100 text-black/80 h-30 w-full flex flex-col items-center justify-center">
//             <p>No VR files uploaded yet</p>
//           </div>
//         ) : (
//           <div className="w-full flex flex-row justify-center gap-2 flex-wrap">
//             {vrfiles.map((vr) => (
//               <div key={vr.id} className="flex flex-col flex-start">
//                 {/* Video */}
//                 <div>
//                   <video
//                     controls
//                     className="md:w-150 md:h-full w-90 h-40 object-cover"
//                   >
//                     <source
//                       src={vr.mediaUrl}
//                       type={`video/${vr.mediaUrl.split(".").pop()}`}
//                     />
//                   </video>
//                 </div>
//                 <div className="flex flex-row justify-between">
//                   {/* Download button */}
//                   <button
//                     type="button"
//                     className="w-6 h-6 rounded-full bg-blue-600 opacity-80 hover:opacity-100 flex items-center justify-center"
//                     onClick={() => onDownload(vr.id)}
//                   >
//                     <Download color="white" size={18} />
//                   </button>
//                   {/* Delete button */}
//                   <button
//                     type="button"
//                     className="w-6 h-6 rounded-full bg-red-600 opacity-80 hover:opacity-100 flex items-center justify-center"
//                     onClick={() => {
//                       onDelete(video.id);
//                     }}
//                   >
//                     <Trash2 color="white" size={18} />
//                   </button>
//                 </div>
//               </div>
//             ))}
//           </div>
//         )}
//       </div>
//     </SectionPannelLayout>
//   );
// }
