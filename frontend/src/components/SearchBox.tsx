import { Search } from "lucide-react";

interface SearchBoxProps {
  className?: string;
  placeholder?: string;
}

export default function SearchBox({ className, placeholder }: SearchBoxProps) {
  return (
    <form>
      <div className={`relative ${className}`}>
        <label htmlFor="search">
          <Search className="absolute top-2 left-2" />
        </label>
        <input
          type="search"
          id="search"
          className="border-none rounded-md w-full pl-10 pr-6 pt-2 pb-2 bg-gray-200"
          placeholder={placeholder}
        />
      </div>
    </form>
  );
}
