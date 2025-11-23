using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;
using Remp.Models.Enums;

namespace Remp.DataAccess.Configurations;

public class ListingCaseConfiguration : IEntityTypeConfiguration<ListingCase>
{
    public void Configure(EntityTypeBuilder<ListingCase> builder)
    {
        // Primary key
        builder.HasKey(lc => lc.Id);

        builder.Property(lc => lc.Title)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(lc => lc.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(lc => lc.Street)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(lc => lc.City)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(lc => lc.State)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(lc => lc.Postcode)
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(lc => lc.Longitude)
            .HasColumnType("decimal(9, 6)")
            .IsRequired();

        builder.Property(lc => lc.Latitude)
            .HasColumnType("decimal(9, 6)")
            .IsRequired();

        builder.Property(lc => lc.Price)
            .HasColumnType("decimal(16, 2)")
            .IsRequired();

        builder.Property(lc => lc.Bedrooms)
            .IsRequired();

        builder.Property(lc => lc.Bathrooms)
            .IsRequired();

        builder.Property(lc => lc.Garages)
            .IsRequired();

        builder.Property(lc => lc.FloorArea)
            .HasColumnType("decimal(8, 2)")
            .IsRequired();

        builder.Property(lc => lc.CreatedAt)
            .IsRequired();

        builder.Property(lc => lc.IsDeleted)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(lc => lc.PropertyType)
            .IsRequired();

        builder.Property(lc => lc.SaleCategory)
            .IsRequired();

        builder.Property(lc => lc.ListingCaseStatus)
            .HasDefaultValue(ListingCaseStatus.Created)
            .IsRequired();

        builder.HasMany(lc => lc.AgentListingCases)
            .WithOne(alc => alc.ListingCase)
            .HasForeignKey(alc => alc.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lc => lc.User)
            .WithMany(u => u.ListingCases)
            .HasForeignKey(lc => lc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(lc => lc.CaseContacts)
            .WithOne(cc => cc.ListingCase)
            .HasForeignKey(cc => cc.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(lc => lc.MediaAssets)
            .WithOne(ma => ma.ListingCase)
            .HasForeignKey(ma => ma.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
