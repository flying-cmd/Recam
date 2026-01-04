import { useEffect, useState } from "react";
import SearchBox from "../../components/SearchBox";
import AgentTable from "../../features/admin/AgentTable";
import type { IAgent } from "../../types/IAgent";
import { getAgentsUnderPhotographyCompany } from "../../services/userService";
import Spinner from "../../components/Spinner";

export default function AgentPage() {
  const [agents, setAgents] = useState<IAgent[]>([]);
  const [isLoading, setIsLoading] = useState(true);

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
            />
          </div>

          <div className="flex-1 flex justify-end">
            <button
              type="button"
              className="bg-sky-500 hover:bg-sky-600 text-white rounded-md sm:px-2 sm:ml-6 sm:w-2/3 py-2 "
            >
              + Create New Agent
            </button>
          </div>
        </div>

        <div>
          <AgentTable agents={agents} />
        </div>
      </section>
    </>
  );
}
