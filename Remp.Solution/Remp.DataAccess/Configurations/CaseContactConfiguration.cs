using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class CaseContactConfiguration : IEntityTypeConfiguration<CaseContact>
{
    public void Configure(EntityTypeBuilder<CaseContact> builder)
    {
        // Primary key
        builder.HasKey(cc => cc.ContactId);

        builder.Property(cc => cc.FirstName)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(cc => cc.LastName)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(cc => cc.CompanyName)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(cc => cc.ListingCase)
            .WithMany(lc => lc.CaseContacts)
            .HasForeignKey(cc => cc.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
