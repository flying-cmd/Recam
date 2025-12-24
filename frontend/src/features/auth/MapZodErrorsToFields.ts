import { z } from "zod";

// TFields must be a string union
export type FieldErrors<TFields extends string> = Partial<Record<TFields, string>>;

export function MapZodErrorsToFields<TFields extends string>(error: z.ZodError): FieldErrors<TFields> {
  const errors: Record<string, string> = {};
  for (const issue of error.issues) {
    const field = issue.path[0]; // get the field name
    if (!field) {
      continue; // no error
    }
    if (!errors[String(field)]) {
      errors[String(field)] = issue.message; // show the first error if there are multiple
    }
  }

  return errors as FieldErrors<TFields>;
}