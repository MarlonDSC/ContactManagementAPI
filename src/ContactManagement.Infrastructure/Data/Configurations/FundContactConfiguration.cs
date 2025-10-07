using ContactManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContactManagement.Infrastructure.Data.Configurations
{
    public class FundContactConfiguration : IEntityTypeConfiguration<FundContact>
    {
        public void Configure(EntityTypeBuilder<FundContact> builder)
        {
            builder.HasKey(fc => fc.Id);

            builder.Property(fc => fc.ContactId)
                .IsRequired();

            builder.Property(fc => fc.FundId)
                .IsRequired();

            builder.Property(fc => fc.CreatedAt)
                .IsRequired();

            builder.Property(fc => fc.UpdatedAt)
                .IsRequired();

            builder.Property(fc => fc.IsDeleted)
                .IsRequired();

            builder.Property(fc => fc.DeletedAt)
                .IsRequired(false);

            // Define relationships
            builder.HasOne(fc => fc.Contact)
                .WithMany()
                .HasForeignKey(fc => fc.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(fc => fc.Fund)
                .WithMany()
                .HasForeignKey(fc => fc.FundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
