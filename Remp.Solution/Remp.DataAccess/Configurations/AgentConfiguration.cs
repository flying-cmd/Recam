using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class AgentConfiguration : IEntityTypeConfiguration<Agent>
{
    public void Configure(EntityTypeBuilder<Agent> builder)
    {
        // Primary key
        builder.HasKey(a => a.Id);

        builder.Property(a => a.AgentFirstName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.AgentLastName)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(a => a.AvatarUrl)
            .IsRequired();

        builder.Property(a => a.CompanyName)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(a => a.User)
            .WithOne(u => u.Agent)
            .HasForeignKey<Agent>(a => a.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.AgentListingCases)
            .WithOne(alc => alc.Agent)
            .HasForeignKey(alc => alc.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(a => a.AgentPhotographyCompanies)
            .WithOne(apc => apc.Agent)
            .HasForeignKey(apc => apc.AgentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

