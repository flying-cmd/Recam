interface PlusMinusButtonProps {
  value: number;
  min: number;
  max?: number;
  step: number;
  onChange: (value: number) => void;
}

export default function PlusMinusButton({
  value,
  min,
  max,
  step,
  onChange,
}: PlusMinusButtonProps) {
  const handleIncrement = (value: number) => {
    // If the value is less than the maximum value, increment it by the step value
    if (max) {
      if (value + step <= max) {
        onChange(value + step);
      } else {
        onChange(max);
      }
    } else {
      onChange(value + step);
    }
  };

  const handleDecrement = (value: number) => {
    // If the value is greater than the minimum value, decrement it by the step value
    if (value - step >= min) {
      onChange(value - step);
    } else {
      onChange(min);
    }
  };

  return (
    <div className="flex flex-row h-10 items-center">
      <button
        type="button"
        className="w-full h-full bg-white border border-gray-300 rounded-l-md p-2"
        onClick={() => handleDecrement(value)}
      >
        -
      </button>
      <input
        type="number"
        value={value}
        min={min}
        max={max}
        step={step}
        onChange={(e) => onChange(Number(e.target.value))}
        className="border h-full bg-white border-gray-300 text-center w-10"
      />
      <button
        type="button"
        className="w-full h-full bg-white border border-gray-300 rounded-r-md p-2"
        onClick={() => handleIncrement(value)}
      >
        +
      </button>
    </div>
  );
}
