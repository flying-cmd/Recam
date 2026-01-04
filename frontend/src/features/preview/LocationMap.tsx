interface LocationMapProps {
  longitude: number;
  latitude: number;
}

export default function LocationMap({ longitude, latitude }: LocationMapProps) {
  const src = `https://www.google.com/maps?q=${latitude},${longitude}&z=15&output=embed`;

  return (
    <div className="flex flex-col items-center">
      <h1 className="text-center font-bold text-2xl mt-10">Location</h1>

      <div className="m-9">
        <iframe
          title="Map"
          src={src}
          className="md:w-150 md:h-120 w-80 h-60"
          loading="lazy"
          referrerPolicy="no-referrer-when-downgrade"
          allowFullScreen
        ></iframe>
      </div>
    </div>
  );
}
