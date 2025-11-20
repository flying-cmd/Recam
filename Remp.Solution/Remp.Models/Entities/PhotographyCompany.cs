using System.Reflection.Metadata;

namespace Remp.Models.Entities;

public class PhotographyCompany
{
    // Primary key (also foreign key)
    public string Id { get; set; } = null!;

    public string PhotographyCompanyName { get; set; } = null!;

    // Navigation property
    public User User { get; set; } = null!;
    public ICollection<AgentPhotographyCompany> AgentPhotographyCompanies { get; set; } = new List<AgentPhotographyCompany>();
}
