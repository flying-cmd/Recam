import { useEffect, useMemo, useState } from "react";
import SearchBox from "../../components/SearchBox";
import AgentTable from "../../features/admin/agent/AgentTable";
import type { IAgent } from "../../types/IAgent";
import { getAgentsUnderPhotographyCompany } from "../../services/userService";
import Spinner from "../../components/Spinner";
import CreateAgentModal from "../../features/admin/agent/CreateAgentModal";

export default function AgentPage() {
  const [agents, setAgents] = useState<IAgent[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [showModal, setShowModal] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    (async () => {
      try {
        const res = await getAgentsUnderPhotographyCompany();
        setAgents(res.data);
        setIsLoading(false);
      } catch (error) {
        console.error(error);
      } finally {
        setIsLoading(false);
      }
    })();
  });

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
              onClick={() => setShowModal(true)}
            >
              + Create New Agent
            </button>
          </div>
        </div>

        <div>
          <AgentTable agents={searchAgents} />
        </div>
      </section>

      {/* Create Agent Modal */}
      {showModal && (
        <CreateAgentModal
          open={showModal}
          onClose={() => setShowModal(false)}
        />
      )}
    </>
  );
}
