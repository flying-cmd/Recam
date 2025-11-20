using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class AgentPhotographyCompanyConfiguration : IEntityTypeConfiguration<AgentPhotographyCompany>
{
    public void Configure(EntityTypeBuilder<AgentPhotographyCompany> builder)
    {
        // Primary key
        builder.HasKey(apc => new { apc.AgentId, apc.PhotographyCompanyId });

        builder.HasOne(apc => apc.Agent)
            .WithMany(a => a.AgentPhotographyCompanies)
            .HasForeignKey(apc => apc.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(apc => apc.PhotographyCompany)
            .WithMany(pc => pc.AgentPhotographyCompanies)
            .HasForeignKey(apc => apc.PhotographyCompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
