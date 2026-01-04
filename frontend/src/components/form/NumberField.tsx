interface NumberFieldProps {
  label: string;
  value: number;
  min?: number;
  step?: number;
  onChange: (value: number) => void;
}

export default function NumberField({
  label,
  value,
  min,
  step,
  onChange,
}: NumberFieldProps) {
  return (
    <div className="w-full pt-4">
      <label className="block mb-2">{label}</label>
      <input
        type="text"
        value={value}
        min={min}
        step={step}
        onChange={(e) => onChange(Number(e.target.value))}
        className="w-full border border-gray-300 rounded-md p-2"
      />
    </div>
  );
}
