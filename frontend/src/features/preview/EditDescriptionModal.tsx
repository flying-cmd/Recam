import TextAreaField from "../../components/form/TextAreaField";
import Modal from "../../components/modal/Modal";

interface EditDescriptionModalProps {
  open: boolean;
  onClose: () => void;
  value: string;
  onChange: (value: string) => void;
  onSave: () => void;
  isSaving: boolean;
  error?: string;
}

export default function EditDescriptionModal({
  open,
  onClose,
  value,
  onChange,
  onSave,
  isSaving,
  error,
}: EditDescriptionModalProps) {
  return (
    <Modal open={open} onClose={onClose} title="Edit Description">
      <div className="mt-2">
        <TextAreaField
          label="Description"
          placeholder="Description"
          value={value}
          onChange={onChange}
          rows={4}
          error={error}
        />

        <div className="mt-4 flex justify-end gap-2">
          <button
            type="button"
            onClick={onClose}
            disabled={isSaving}
            className="rounded-md border px-4 py-2 text-sm hover:bg-gray-50 disabled:opacity-60"
          >
            Cancel
          </button>

          <button
            type="button"
            onClick={onSave}
            disabled={isSaving}
            className="rounded-md bg-blue-600 px-4 py-2 text-sm text-white hover:bg-blue-700 disabled:opacity-60"
          >
            {isSaving ? "Saving..." : "Save"}
          </button>
        </div>
      </div>
    </Modal>
  );
}
