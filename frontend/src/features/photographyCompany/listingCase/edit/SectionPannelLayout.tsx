interface SectionPannelLayoutProps {
  title: string;
  onBack: () => void;
  children: React.ReactNode;
}

export default function SectionPannelLayout({
  title,
  onBack,
  children,
}: SectionPannelLayoutProps) {
  return (
    <div className="w-full">
      <div className="flex flex-row justify-between">
        <h2 className="font-bold text-xl">{title}</h2>
        <button
          type="button"
          className="text-gray-600 hover:text-black hover:cursor-pointer"
          onClick={onBack}
        >
          Back
        </button>
      </div>

      <hr className="mt-4 w-full border-gray-300" />

      {children}
    </div>
  );
}
