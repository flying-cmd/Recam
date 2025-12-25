import { z } from "zod";

const acceptedFileTypes = ["image/jpeg", "image/jpg", "image/png"];

export const registerSchema = z.object({
  email: z.email("Invalid email address."),
  password: z
    .string()
    .min(6, "Password must be at least 6 characters long.")
    .regex(/[A-Z]/, "Password must contain at least one uppercase letter.")
    .regex(/[a-z]/, "Password must contain at least one lowercase letter.")
    .regex(/[0-9]/, "Password must contain at least one number.")
    .regex(/[^a-zA-Z0-9]/, "Password must contain at least one special character."),
  confirmPassword: z.string().min(6, "Password must be at least 6 characters long."),
  firstName: z.string().min(1, "First name is required."),
  lastName: z.string().min(1, "Last name is required."),
  companyName: z.string().min(1, "Company name is required."),
  avatarFile: z.instanceof(File).refine((file) => acceptedFileTypes.includes(file.type), "Avatar must be a JPEG, JPG, or PNG file."),
}).refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match.",
    path: ["confirmPassword"], // attach error to confirmPassword field
});

export const loginSchema = registerSchema.pick({ email: true, password: true });
export const adminSchema = registerSchema.pick({ email: true, password: true });