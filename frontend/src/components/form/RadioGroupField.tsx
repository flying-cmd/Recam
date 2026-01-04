type Option<T extends string> = {
  label: string;
  value: T;
};

interface RadioGroupFieldProps<T extends string> {
  label: string;
  name: string;
  value: T;
  onChange: (value: T) => void;
  options: Option<T>[];
}

export default function RadioGroupField<T extends string>({
  label,
  name,
  value,
  onChange,
  options,
}: RadioGroupFieldProps<T>) {
  return (
    <div className="w-full pt-6 pb-2">
      <p>{label}</p>
      <div className="mt-2 flex flex-wrap gap-8">
        {options.map((option) => (
          <label key={option.value} className="inline-flex items-center gap-2">
            <input
              type="radio"
              name={name}
              value={option.value}
              checked={value === option.value}
              onChange={(e) => onChange(e.target.value as T)}
              className="h-4 w-4 accent-slate-900"
            />
            {option.label}
          </label>
        ))}
      </div>
    </div>
  );
}
