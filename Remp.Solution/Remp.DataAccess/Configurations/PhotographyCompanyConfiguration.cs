using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class PhotographyCompanyConfiguration : IEntityTypeConfiguration<PhotographyCompany>
{
    public void Configure(EntityTypeBuilder<PhotographyCompany> builder)
    {
        // Primary key
        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.PhotographyCompanyName)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(pc => pc.User)
            .WithOne(u => u.PhotographyCompany)
            .HasForeignKey<PhotographyCompany>(pc => pc.Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(pc => pc.AgentPhotographyCompanies)
            .WithOne(apc => apc.PhotographyCompany)
            .HasForeignKey(apc => apc.PhotographyCompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

