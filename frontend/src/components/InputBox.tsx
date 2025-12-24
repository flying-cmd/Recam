interface InputBoxProps {
  id: string;
  label: string;
  type: string;
  value: string;
  setValue: (val: string) => void;
  placeholder: string;
  error?: string;
}

export default function InputBox({
  id,
  label,
  type,
  value,
  setValue,
  placeholder,
  error,
}: InputBoxProps) {
  return (
    <>
      <label
        className="block text-gray-700 text-sm font-bold mb-2"
        htmlFor={id}
      >
        {label}
      </label>
      <input
        className="border rounded w-full py-2 px-3 text-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-300"
        id={id}
        type={type}
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder={placeholder}
      />

      {error && <p className="text-red-500 text-xs mt-1">{error}</p>}
    </>
  );
}
