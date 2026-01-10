import z from "zod";

export const addAgentSchema = z.object({
  email: z.email("Invalid email address."),
  agentFirstName: z.string().min(1, "First name is required."),
  agentLastName: z.string().min(1, "Last name is required."),
  companyName: z.string().min(1, "Company name is required."),
  phoneNumber: z.string().min(1, "Phone number is required."),
});