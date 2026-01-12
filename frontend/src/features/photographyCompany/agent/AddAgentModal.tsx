import { useState } from "react";
import Modal from "../../../components/modal/Modal";
import TextField from "../../../components/form/TextField";
import {
  MapZodErrorsToFields,
  type FieldErrors,
} from "../../../utils/MapZodErrorsToFields";
import { addAgentSchema } from "./addAgentSchema";
import SearchBox from "../../../components/SearchBox";
import { searchAgentByEmail } from "../../../services/userService";

interface AddAgentModalProps {
  open: boolean;
  onClose: () => void;
  onAddAgent: (agentId: string) => Promise<void>;
}

interface FormState {
  id?: string;
  email: string;
  agentFirstName: string;
  agentLastName: string;
  companyName: string;
  phoneNumber: string;
}

type FormErrors = FieldErrors<keyof FormState>;

export default function AddAgentModal({
  open,
  onClose,
  onAddAgent,
}: AddAgentModalProps) {
  const [form, setForm] = useState<FormState>({
    id: "",
    email: "",
    agentFirstName: "",
    agentLastName: "",
    companyName: "",
    phoneNumber: "",
  });
  const [searchTerm, setSearchTerm] = useState("");
  const [isAdding, setIsAdding] = useState<boolean>(false);
  const [isSearching, setIsSearching] = useState<boolean>(false);
  const [errors, setErrors] = useState<FormErrors>({});
  const [searchError, setSearchError] = useState<string>("");

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
      id: "",
      email: "",
      agentFirstName: "",
      agentLastName: "",
      companyName: "",
      phoneNumber: "",
    });
  };

  const handleSearch = async () => {
    try {
      setIsSearching(true);
      setSearchError("");
      const res = await searchAgentByEmail(searchTerm);

      if (res.data) {
        setForm((prev) => ({
          ...prev,
          id: res.data.id,
          email: res.data.email,
          agentFirstName: res.data.agentFirstName,
          agentLastName: res.data.agentLastName,
          companyName: res.data.companyName,
          phoneNumber: res.data.phoneNumber,
        }));
      } else {
        throw new Error("Agent not found");
      }
    } catch (error) {
      console.error(error);
      setSearchError((error as Error).message);
    } finally {
      setIsSearching(false);
    }
  };

  const handleSave = async () => {
    // validate form
    const formResult = addAgentSchema.safeParse(form);

    // if the form is not valid
    if (!formResult.success) {
      const errors = MapZodErrorsToFields(formResult.error);
      setErrors(errors);
      return;
    }

    // if the form is valid
    setErrors({});

    try {
      setIsAdding(true);
      await onAddAgent(form.id ?? "");

      resetForm();
      onClose();
    } catch (error) {
      console.error(error);
      setSearchError((error as Error).message);
    } finally {
      setIsAdding(false);
    }
  };

  if (!open) {
    return null;
  }

  return (
    <Modal
      open={open}
      onClose={onClose}
      title="Add Agent to Company"
      description="Search for an agent to add to your company."
    >
      {/* Search Agent Box */}
      <div className="w-full flex flex-row flex-start justify-between gap-2 mt-4">
        <div className="w-full">
          <SearchBox
            className="w-full"
            placeholder="Please enter the agent's email to search."
            onChange={setSearchTerm}
            value={searchTerm}
          />
        </div>
        <button
          type="button"
          className="w-20 h-full text-center border-none bg-sky-500 rounded-md p-2 text-white hover:bg-sky-600"
          onClick={handleSearch}
          disabled={isSearching}
        >
          {isSearching ? "Searching..." : "Search"}
        </button>
      </div>

      {searchError && <p className="text-red-500">{searchError}</p>}

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
        label="Email"
        value={form.email}
        placeholder="Please enter the agent's email."
        onChange={(value) => handleFormChange("email", value)}
        error={errors.email}
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

      {/* Save and Cancel buttons */}
      <div className="flex flex-row justify-end gap-4 mt-4 w-full">
        <button
          type="button"
          className="w-20 h-10 bg-white border-black border rounded-2xl p-2 hover:bg-gray-200"
          onClick={onClose}
          disabled={isAdding}
        >
          Cancel
        </button>
        <button
          type="button"
          className="w-30 h-10 bg-blue-400 border border-blue-500 rounded-2xl p-2 text-white hover:bg-blue-500"
          onClick={handleSave}
          disabled={isAdding}
        >
          {isAdding ? "Adding..." : "Add Agent"}
        </button>
      </div>
    </Modal>
  );
}
