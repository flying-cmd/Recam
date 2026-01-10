import z from "zod";

const acceptedFileTypes = ["image/jpeg", "image/jpg", "image/png"];

export const createAgentSchema = z.object({
  email: z.email("Invalid email address."),
  agentFirstName: z.string().min(1, "First name is required."),
  agentLastName: z.string().min(1, "Last name is required."),
  companyName: z.string().min(1, "Company name is required."),
  phoneNumber: z.string().min(1, "Phone number is required."),
  avatar: z.instanceof(File).nullable().optional().refine((file) => !file || acceptedFileTypes.includes(file.type), "Avatar must be a JPEG, JPG, or PNG file."),
});