using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Remp.Models.Entities;

namespace Remp.DataAccess.Configurations;

public class MediaAssetConfiguration : IEntityTypeConfiguration<MediaAsset>
{
    public void Configure(EntityTypeBuilder<MediaAsset> builder)
    {
        // Primary key
        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.MediaType)
            .IsRequired();

        builder.Property(ma => ma.MediaUrl)
            .IsRequired();

        builder.Property(ma => ma.UploadedAt)
            .IsRequired();

        builder.Property(ma => ma.IsSelect)
            .IsRequired();

        builder.Property(ma => ma.IsHero)
            .IsRequired();

        builder.HasOne(ma => ma.ListingCase)
            .WithMany(lc => lc.MediaAssets)
            .HasForeignKey(ma => ma.ListingCaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ma => ma.User)
            .WithMany(a => a.MediaAssets)
            .HasForeignKey(ma => ma.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
