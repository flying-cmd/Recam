interface TabButtonProps {
  onClick: () => void;
  icon: React.ReactNode;
  title: string;
}

export default function TabButton({ onClick, icon, title }: TabButtonProps) {
  return (
    <div
      className="w-50 h-20 flex flex-col items-center justify-center bg-gray-200 hover:bg-gray-300 rounded-md p-2 hover:cursor-pointer"
      onClick={onClick}
    >
      {icon}
      <div>{title}</div>
    </div>
  );
}
