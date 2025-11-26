using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IUserRepository
{
    Task AddAgentToPhotographyCompanyAsync(string agentId, string photographyCompanyId);
    Task<Agent?> FindAgentByIdAsync(string agentId);
    Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId);
    Task<bool> IsAgentAddedToPhotographyCompanyAsync(string agentId, string photographyCompanyId);
}
