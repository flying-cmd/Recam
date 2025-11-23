using Remp.Common.Helpers;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IPhotographyCompanyRepository
{
    Task<IEnumerable<Agent>> GetAgentsAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
}
