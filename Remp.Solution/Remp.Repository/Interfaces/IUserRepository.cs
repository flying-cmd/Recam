using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
}
