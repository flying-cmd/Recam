using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class AgentListingCaseConfiguration : IEntityTypeConfiguration<AgentListingCase>
{
    public void Configure(EntityTypeBuilder<AgentListingCase> builder)
    {
        // Primary key
        builder.HasKey(alc => new { alc.AgentId, alc.ListingCaseId });

        builder.HasOne(alc => alc.Agent)
            .WithMany(a => a.AgentListingCases)
            .HasForeignKey(alc => alc.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(alc => alc.ListingCase)
            .WithMany(lc => lc.AgentListingCases)
            .HasForeignKey(alc => alc.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
