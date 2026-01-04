interface TextAreaFieldProps {
  label: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
  rows: number;
  error?: string;
}

export default function TextAreaField({
  label,
  placeholder,
  value,
  onChange,
  rows,
  error,
}: TextAreaFieldProps) {
  return (
    <div className="w-full pt-6 pb-2">
      <label className="block mb-2">{label}</label>
      <textarea
        className="w-full border border-gray-300 rounded-md p-2"
        rows={rows}
        value={value}
        placeholder={placeholder}
        onChange={(e) => onChange(e.target.value)}
      ></textarea>
      {error && <p className="text-red-500 text-sm mt-1">{error}</p>}
    </div>
  );
}
