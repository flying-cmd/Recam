interface ActionButtonProps {
  icon: React.ReactNode;
  children: React.ReactNode;
  style?: string;
  onClick?: () => void;
  disabled?: boolean;
}

export default function ActionButton({
  icon,
  children,
  style,
  onClick,
  disabled,
}: ActionButtonProps) {
  return (
    <button
      className={`inline-flex border rounded-full mx-1 p-2 text-sm ${style}`}
      onClick={onClick}
      disabled={disabled}
    >
      {icon}
      {children}
    </button>
  );
}
