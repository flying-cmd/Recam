namespace Remp.Models.Entities;

public class AgentPhotographyCompany
{
    // Primary Key
    public string AgentId { get; set; } = null!;
    public string PhotographyCompanyId { get; set; } = null!;

    // Navigation Properties
    public Agent Agent { get; set; } = null!;
    public PhotographyCompany PhotographyCompany { get; set; } = null!;
}
