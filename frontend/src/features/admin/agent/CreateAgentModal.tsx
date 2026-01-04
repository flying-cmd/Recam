import { useState } from "react";
import Modal from "../../../components/modal/Modal";
import TextField from "../../../components/form/TextField";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../../utils/MapZodErrorsToFields";
import { createAgentSchema } from "./createAgentSchema";
import { createAgentByPhotographyCompany } from "../../../services/userService";
import AvatarUpload from "../../../components/form/AvatarUpload";

interface CreateAgentModalProps {
  open: boolean;
  onClose: () => void;
}

interface FormState {
  email: string;
  agentFirstName: string;
  agentLastName: string;
  companyName: string;
  phoneNumber: string;
  avatar: File | null;
}

type FormErrors = FieldErrors<keyof FormState>;

export default function CreateAgentModal({
  open,
  onClose,
}: CreateAgentModalProps) {
  const [form, setForm] = useState<FormState>({
    email: "",
    agentFirstName: "",
    agentLastName: "",
    companyName: "",
    phoneNumber: "",
    avatar: null,
  });
  const [isSaving, setIsSaving] = useState<boolean>(false);
  const [errors, setErrors] = useState<FormErrors>({});

  // K extends keyof FormState: K must be a key of FormState
  // FormState[K]: get the type of the key
  const handleFormChange = <K extends keyof FormState>(
    key: K,
    value: FormState[K]
  ) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const resetForm = () => {
    setForm({
      email: "",
      agentFirstName: "",
      agentLastName: "",
      companyName: "",
      phoneNumber: "",
      avatar: null,
    });
  };

  const handleSave = async () => {
    // validate form
    const formResult = createAgentSchema.safeParse(form);

    // if the form is not valid
    if (!formResult.success) {
      const errors = MapZodErrorsToFields(formResult.error);
      setErrors(errors);
      return;
    }

    // if the form is valid
    setErrors({});

    try {
      setIsSaving(true);
      const res = await createAgentByPhotographyCompany(form);

      console.log(res);

      if (res.success) {
        resetForm();
        onClose();
      } else {
        throw new Error("Failed to create agent");
      }
    } catch (error) {
      console.error(error);
    } finally {
      setIsSaving(false);
    }
  };

  if (!open) {
    return null;
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      title="Agent Details"
      description="Please take a moment to complete the agent details."
    >
      {/* Email */}
      <TextField
        label="Email"
        value={form.email}
        placeholder="Please enter the agent's email."
        onChange={(value) => handleFormChange("email", value)}
        error={errors.email}
      />

      <hr className="mt-4 w-full border-gray-300" />

      {/* Agent Name */}
      <div className="w-full flex-col mt-4">
        <p className="text-lg">Agent Name</p>
        <div className="flex gap-4">
          {/* First Name */}
          <TextField
            label="First Name"
            value={form.agentFirstName}
            placeholder="Please enter the agent's first name."
            onChange={(value) => handleFormChange("agentFirstName", value)}
            error={errors.agentFirstName}
          />

          {/* Last Name */}
          <TextField
            label="Last Name"
            value={form.agentLastName}
            placeholder="Please enter the agent's last name."
            onChange={(value) => handleFormChange("agentLastName", value)}
            error={errors.agentLastName}
          />
        </div>
      </div>

      <hr className="mt-4 w-full border-gray-300" />

      {/* Company Name */}
      <TextField
        label="Company Name"
        value={form.companyName}
        placeholder="Please enter the agent's company name."
        onChange={(value) => handleFormChange("companyName", value)}
        error={errors.companyName}
      />

      <hr className="mt-4 w-full border-gray-300" />

      {/* Phone Number */}
      <TextField
        label="Phone Number"
        value={form.phoneNumber}
        placeholder="Please enter the agent's phone number."
        onChange={(value) => handleFormChange("phoneNumber", value)}
        error={errors.phoneNumber}
      />

      <hr className="mt-4 w-full border-gray-300" />

      {/* Avatar Image */}
      <AvatarUpload
        label="Avatar Image"
        onChange={(file) => handleFormChange("avatar", file)}
        error={errors.avatar}
      />

      <hr className="mt-4 w-full border-gray-300" />

      {/* Save and Cancel buttons */}
      <div className="flex flex-row justify-end gap-4 mt-4 w-full">
        <button
          type="button"
          className="w-20 h-10 bg-white border-black border rounded-2xl p-2 hover:bg-gray-200"
          onClick={onClose}
          disabled={isSaving}
        >
          Cancel
        </button>
        <button
          type="button"
          className="w-20 h-10 bg-blue-400 border border-blue-500 rounded-2xl p-2 text-white hover:bg-blue-500"
          onClick={handleSave}
          disabled={isSaving}
        >
          {isSaving ? "Saving..." : "Save"}
        </button>
      </div>
    </Modal>
  );
}
