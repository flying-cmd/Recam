export const getFilenameFromDisposition = (disposition?: string) => {
  if (!disposition) return null;

  // filename*=UTF-8''...
  const utf8Match = disposition.match(/filename\*\s*=\s*UTF-8''([^;]+)/i);
  if (utf8Match?.[1]) return decodeURIComponent(utf8Match[1]);

  // filename="..."
  const quotedMatch = disposition.match(/filename\s*=\s*"([^"]+)"/i);
  if (quotedMatch?.[1]) return quotedMatch[1];

  // filename=...
  const plainMatch = disposition.match(/filename\s*=\s*([^;]+)/i);
  if (plainMatch?.[1]) return plainMatch[1].trim();

  return null;
}