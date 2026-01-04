export type IAgent = {
  id: string;
  agentFirstName: string;
  agentLastName: string;
  avatarUrl: string;
  companyName: string;
  phoneNumber: string;
  email: string;
};

export interface ICreateAgent {
  agentFirstName: string;
  agentLastName: string;
  avatar: File | null;
  companyName: string;
  phoneNumber: string;
  email: string;
}