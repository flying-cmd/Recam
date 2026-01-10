import { Plus } from "lucide-react";
import SectionPannelLayout from "./SectionPannelLayout";
import type { IAgent } from "../../../../types/IAgent";
import { useCallback, useEffect, useState } from "react";
import {
  assignAgentToListingCase,
  deleteAgentFromListingCase,
  getAssignedAgentsByListingCaseId,
} from "../../../../services/listingCaseService";
import { useParams } from "react-router-dom";
import AgentCard from "./AgentCard";
import Spinner from "../../../../components/Spinner";
import AssignToAgentModal from "./AssignToAgentModal";

interface AssignToAgentPannelProps {
  onBack: () => void;
}

export default function AssignToAgentPannel({
  onBack,
}: AssignToAgentPannelProps) {
  const { listingCaseId } = useParams<{ listingCaseId: string }>();
  const [agents, setAgents] = useState<IAgent[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);

  const loadAgents = useCallback(async () => {
    if (!listingCaseId) return;

    try {
      const res = await getAssignedAgentsByListingCaseId(
        parseInt(listingCaseId!)
      );
      setAgents(res.data);
    } catch (error) {
      console.error(error);
    }
  }, [listingCaseId]);

  useEffect(() => {
    (async () => {
      try {
        setIsLoading(true);
        await loadAgents();
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [loadAgents]);

  const handleRemoveAgent = async (agentId: string) => {
    await deleteAgentFromListingCase(parseInt(listingCaseId!), agentId);
    await loadAgents();
  };

  const handleAssignAgent = async (agentId: string) => {
    await assignAgentToListingCase(parseInt(listingCaseId!), agentId);
    await loadAgents();
  };

  if (isLoading) return <Spinner />;

  return (
    <>
      <SectionPannelLayout title="Agent" onBack={onBack}>
        <div>
          {/* Button */}
          <div className="flex flex-row justify-center m-4">
            <button
              type="button"
              className="flex flex-row items-center gap-1 border-none rounded-md bg-sky-600 p-2 hover:bg-sky-700 hover:cursor-pointer"
              onClick={() => setShowModal(true)}
            >
              <Plus size={18} color="white" />
              <div className="text-white">Add Agent</div>
            </button>
          </div>

          {/* Content */}
          {agents.length === 0 ? (
            <div className="flex flex-row justify-center items-center bg-gray-100 text-gray-700 w-full h-20">
              <p>No agent assigned to this property</p>
            </div>
          ) : (
            <div className="flex flex-row gap-2 flex-wrap">
              {agents.map((agent) => (
                <AgentCard
                  key={agent.id}
                  agent={agent}
                  onRemove={() => handleRemoveAgent(agent.id)}
                />
              ))}
            </div>
          )}
        </div>
      </SectionPannelLayout>

      {/* Modal */}
      {showModal && (
        <AssignToAgentModal
          open={showModal}
          onClose={() => setShowModal(false)}
          onAssign={handleAssignAgent}
        />
      )}
    </>
  );
}
