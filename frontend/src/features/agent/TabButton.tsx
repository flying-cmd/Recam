interface TabButtonProps {
  title: string;
  active: boolean;
  onClick: () => void;
}

export default function TabButton({ title, active, onClick }: TabButtonProps) {
  return (
    <button
      className={[
        "hover:bg-gray-200 hover:cursor-pointer w-full h-8 flex items-center justify-center",
        active ? "bg-gray-200" : "bg-gray-100",
      ].join(" ")}
      onClick={onClick}
    >
      {title}
    </button>
  );
}
