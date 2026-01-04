import ActionsButton from "../../../components/ActionsButton";
import type { IAgent } from "../../../types/IAgent";

interface AgentTableProps {
  agents: IAgent[];
}

export default function AgentTable({ agents }: AgentTableProps) {
  const onEdit = () => {};
  const onDelete = () => {};

  return (
    <div className="border-2 border-gray-300 rounded-lg">
      <table className="w-full text-xs md:text-base">
        <thead>
          <tr className="text-left bg-gray-200">
            <th className="md:px-8 py-4">AGENT NAME</th>
            <th className="md:px-8 py-4">COMPANY</th>
            <th className="md:px-8 py-4">PHONE</th>
            <th className="md:px-8 py-4">EMAIL</th>
            <th className="md:px-8 py-4">ACTIONS</th>
          </tr>
        </thead>

        <tbody className="divide-y-2 divide-gray-300">
          {agents.map((agent) => (
            <tr key={agent.id}>
              <td className="md:px-8 py-4">
                {agent.agentFirstName + " " + agent.agentLastName}
              </td>
              <td className="md:px-8 py-4">{agent.companyName}</td>
              <td className="md:px-8 py-4">{agent.phoneNumber}</td>
              <td className="md:px-8 py-4">{agent.email}</td>
              <td className="md:px-8 py-4">
                <ActionsButton onEdit={onEdit} onDelete={onDelete} />
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
