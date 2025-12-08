using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IUserRepository
{
    Task AddAgentAsync(Agent agent);
    Task<Agent?> AddAgentToPhotographyCompanyAsync(AgentPhotographyCompany agentPhotographyCompany);
    Task<Agent?> FindAgentByIdAsync(string agentId);
    Task<User?> FindUserByEmailAsync(string email);
    Task<PhotographyCompany?> FindPhotographyCompanyByIdAsync(string photographyCompanyId);
    Task<Agent?> GetAgentByEmailAsync(string email);
    Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Agent>> GetAgentsUnderPhotographyCompanyAsync(string photographyCompanyId);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<int>> GetAgentListingCaseIdsAsync(string agentId);
    Task<bool> IsAgentAddedToPhotographyCompanyAsync(string agentId, string photographyCompanyId);
    Task<IEnumerable<int>> GetPhotographyCompanyListingCaseIdsAsync(string photographyCompanyId);
}
