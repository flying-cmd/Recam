interface TextFieldProps {
  label: string;
  value: string;
  placeholder: string;
  onChange: (value: string) => void;
}

export default function TextField({
  label,
  value,
  placeholder,
  onChange,
}: TextFieldProps) {
  return (
    <div className="w-full pt-6 pb-2">
      <label className="block mb-2">{label}</label>
      <input
        type="text"
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.target.value)}
        className="w-full border border-gray-300 rounded-md p-2"
      />
    </div>
  );
}
