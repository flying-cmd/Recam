import { useEffect, useState } from "react";
import Modal from "../../../../components/modal/Modal";
import type { IAgent } from "../../../../types/IAgent";
import { getAgentsUnderPhotographyCompany } from "../../../../services/userService";
import Spinner from "../../../../components/Spinner";

interface AssignToAgentModalProps {
  open: boolean;
  onClose: () => void;
  onAssign: (agentId: string) => void;
}

export default function AssignToAgentModal({
  open,
  onClose,
  onAssign,
}: AssignToAgentModalProps) {
  const [agents, setAgents] = useState<IAgent[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [selectedAgentId, setSelectedAgentId] = useState<string>("");
  const [isAssigning, setIsAssigning] = useState(false);

  useEffect(() => {
    if (!open) return;

    (async () => {
      try {
        setIsLoading(true);
        const res = await getAgentsUnderPhotographyCompany();
        setAgents(res.data);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [open]);

  const handleAssign = async () => {
    setIsAssigning(true);
    onAssign(selectedAgentId);
    setIsAssigning(false);
    onClose();
  };

  return (
    <Modal open={open} onClose={onClose} title="Add Agent">
      <div className="my-2">
        <label>Select from available agents</label>

        <div className="my-2">
          {isLoading ? (
            <Spinner />
          ) : (
            <select
              className="w-full appearance-none border rounded-md p-2"
              value={selectedAgentId}
              onChange={(e) => setSelectedAgentId(e.target.value)}
            >
              <option value="">-- Select an agent --</option>
              {agents.map((agent) => (
                <option key={agent.id} value={agent.id}>
                  {agent.agentFirstName} {agent.agentLastName}{" "}
                  {`(${agent.companyName})`}
                </option>
              ))}
            </select>
          )}
        </div>

        {/* Button */}
        <div className="flex flex-row justify-end">
          <button
            type="button"
            className="bg-gray-500 hover:bg-gray-600 text-white py-2 px-4 rounded-md"
            onClick={onClose}
            disabled={isAssigning}
          >
            Cancel
          </button>
          <button
            type="button"
            className="bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded-md ml-2"
            onClick={handleAssign}
            disabled={isAssigning}
          >
            Add Agent
          </button>
        </div>
      </div>
    </Modal>
  );
}
