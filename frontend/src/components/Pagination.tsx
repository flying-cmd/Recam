import { useMemo } from "react";

interface PaginationProps {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  siblingCount: number; // represents the min number of page buttons to be shown on each side of the current page button.
}

export default function Pagination({
  currentPage,
  pageSize,
  totalCount,
  onPageChange,
  siblingCount = 1,
}: PaginationProps) {
  const totalPages = Math.max(1, Math.ceil(totalCount / pageSize));

  const haveNext = currentPage < totalPages;
  const havePrev = currentPage > 1;

  // Build a page list with ellipsis
  const pageList = useMemo(() => {
    if (totalPages <= 1) return [1];

    const start = 1;
    const end = totalPages;

    // Pages around the current page (siblings)
    // avoid overlapping with the start and end pages
    // for example, totalPages = 10, siblingCount = 1, currentPage = 5
    // left = max(2, 4) = 4, right = min(9, 6) = 6, so show [4, 5, 6] in the middle
    const left = Math.max(start + 1, currentPage - siblingCount);
    const right = Math.min(end - 1, currentPage + siblingCount);

    // if the middle segment doesn't start after page 1, add an ellipsis ...
    const items: (number | "dots")[] = [start];

    // construct the page list like: 1 ... 4 5 6 ... 10
    if (left > start + 1) items.push("dots");
    for (let i = left; i <= right; i++) items.push(i);
    if (right < end - 1) items.push("dots");
    items.push(end);

    // remove duplicates (happen in edge cases, like 1 1 -> 1)
    // v: current element value, idx: current element index, arr: whole array
    // If idx === 0 (first element), keep it (there’s no previous element)
    // Otherwise, keep the element only if it’s different from the previous element
    return items.filter((v, idx, arr) => idx === 0 || v !== arr[idx - 1]);
  }, [totalPages, siblingCount, currentPage]);

  const goTo = (page: number) => onPageChange(page);

  return (
    <div className="flex items-center justify-between gap-3">
      <div>
        Page <span className="font-semibold">{currentPage}</span> of{" "}
        <span className="font-semibold">{totalPages}</span>
      </div>

      <div className="flex items-center gap-2">
        <button
          type="button"
          disabled={!havePrev}
          onClick={() => goTo(currentPage - 1)}
          className={`border-none ${
            havePrev ? "bg-white hover:bg-gray-100" : "bg-gray-100"
          }`}
        >
          Prev
        </button>

        <div className="flex items-center gap-1">
          {pageList.map((page, idx) =>
            page === "dots" ? (
              <span key={`dots-${idx}`}>...</span>
            ) : (
              <button
                key={page}
                type="button"
                onClick={() => goTo(page)}
                className={`border-none ${
                  page === currentPage
                    ? "bg-sky-600 text-white border-sky-600"
                    : "bg-white hover:bg-gray-100"
                }`}
              >
                {page}
              </button>
            )
          )}
        </div>

        <button
          type="button"
          disabled={!haveNext}
          onClick={() => goTo(currentPage + 1)}
          className={`round-md ${
            haveNext ? "bg-white hover:bg-gray-100" : "bg-gray-100"
          }`}
        >
          Next
        </button>
      </div>
    </div>
  );
}
