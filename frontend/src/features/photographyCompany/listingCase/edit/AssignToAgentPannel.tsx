import SectionPannelLayout from "./SectionPannelLayout";

interface AssignToAgentPannelProps {
  onBack: () => void;
}

export default function AssignToAgentPannel({
  onBack,
}: AssignToAgentPannelProps) {
  return (
    <SectionPannelLayout title="Agent" onBack={onBack}>
      <div>
        {/* Button */}
        <div className="flex flex-row justify-center"></div>
      </div>
    </SectionPannelLayout>
  );
}
