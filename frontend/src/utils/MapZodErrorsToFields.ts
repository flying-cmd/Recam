import { z } from "zod";

// TFields must be a string union
// type PropertyKey = string | number | symbol
// TFields extends PropertyKey: TFields can be any valid object key type
// Extract<TFields, string>: 
//      Extract<A, B> keeps only the part of A that is assignable to B
//      If TFields = string | number, then Extract<TFields, string> becomes string
//      If TFields = "title" | "price", it stays "title" | "price"
// Record<Keys, string>:
//      Record<K, V> creates an object type whose keys are K and values are V
//      If Keys = "title" | "price", then Record<Keys, string> becomes { title: string, price: string }
// Partial<T> makes every property optional
// FieldErrors<TFields> becomes an object mapping field names → error message, where each field is optional.
export type FieldErrors<TFields extends PropertyKey> = Partial<Record<Extract<TFields, string>, string>>;

export function MapZodErrorsToFields<TFields extends PropertyKey>(error: z.ZodError): FieldErrors<TFields> {
  const errors: Record<string, string> = {}; // store the errors
  // Zod stores detailed error information in error.issues
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