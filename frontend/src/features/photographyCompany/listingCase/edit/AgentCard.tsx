import type { IAgent } from "../../../../types/IAgent";

interface AgentCardProps {
  agent: IAgent;
  onRemove: () => void;
}

export default function AgentCard({ agent, onRemove }: AgentCardProps) {
  return (
    <div className="flex flex-row gap-1 border-none shadow-md p-2 w-70 h-25">
      {/* Avatar */}
      <div className="w-18 h-18 rounded-full">
        <img src={agent.avatarUrl} alt="Avatar" className="object-cover" />
      </div>

      <div className="flex flex-col justify-between">
        {/* Name, company */}
        <div className="flex flex-col">
          <p>
            {agent.agentFirstName} {agent.agentLastName}
          </p>
          <p className="text-gray-400">{agent.companyName}</p>
        </div>

        {/* Remove button */}
        <div>
          <button
            type="button"
            className="text-red-600 hover:cursor-pointer hover:text-red-900"
            onClick={onRemove}
          >
            Remove
          </button>
        </div>
      </div>
    </div>
  );
}
