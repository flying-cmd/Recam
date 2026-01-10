import { useCallback, useEffect, useMemo, useState } from "react";
import SearchBox from "../../components/SearchBox";
import type { IAgent } from "../../types/IAgent";
import {
  deleteAssignedAgent,
  getAgentsUnderPhotographyCompany,
} from "../../services/userService";
import Spinner from "../../components/Spinner";
import AgentTable from "../../features/photographyCompany/agent/AgentTable";
import AddAgentModal from "../../features/photographyCompany/agent/AddAgentModal";

export default function AgentPage() {
  const [agents, setAgents] = useState<IAgent[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showAddAgentModal, setShowAddAgentModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");

  const loadAgents = useCallback(async () => {
    try {
      const res = await getAgentsUnderPhotographyCompany();
      setAgents(res.data);
    } catch (error) {
      console.error(error);
    }
  }, []);

  useEffect(() => {
    (async () => {
      try {
        setIsLoading(true);
        await loadAgents();
        setIsLoading(false);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [loadAgents]);

  const searchAgents = useMemo(() => {
    return agents.filter(
      (agent: IAgent) =>
        agent.agentFirstName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        agent.agentLastName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        agent.companyName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        agent.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
        agent.phoneNumber.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [agents, searchTerm]);

  const handleDelete = async (agentId: string) => {
    try {
      await deleteAssignedAgent(agentId);
      await loadAgents();
    } catch (error) {
      console.error(error);
    }
  };

  if (isLoading) return <Spinner />;

  return (
    <>
      <h1 className="text-center font-bold text-2xl mt-20">Hi, Welcome!</h1>

      <section className="sm:mx-25">
        <div className="flex items-center my-6">
          {/* spacer */}
          <div className="flex-1" />

          <div className="flex-1 flex justify-center">
            <SearchBox
              className="sm:w-140 h-full w-full"
              placeholder="Search from agents"
              value={searchTerm}
              onChange={setSearchTerm}
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
              onClick={() => setShowAddAgentModal(true)}
            >
              + Add New Agent
            </button>
          </div>
        </div>

        <div>
          <AgentTable agents={searchAgents} onDelete={handleDelete} />
        </div>
      </section>

      {/* Create Agent Modal */}
      {showAddAgentModal && (
        <AddAgentModal
          open={showAddAgentModal}
          onClose={() => setShowAddAgentModal(false)}
        />
      )}
    </>
  );
}
