import PlusMinusButton from "./PlusMinusButton";

interface BasicInfoCardProps {
  label: string;
  value: number;
  min: number;
  max?: number;
  step: number;
  onChange: (value: number) => void;
  icon: React.ReactNode;
}

export default function BasicInfoCard({
  label,
  value,
  min,
  max,
  step,
  onChange,
  icon,
}: BasicInfoCardProps) {
  return (
    <div className="flex flex-row gap-6 items-center border-none rounded-md bg-gray-200 p-4">
      <div>
        {/* Icon */}
        {icon}
        <div>{label}</div>
      </div>

      {/* Button */}
      <PlusMinusButton
        value={value}
        min={min}
        max={max}
        step={step}
        onChange={onChange}
      />
    </div>
  );
}
