using FlapKap.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace FlapKap.Persistence.Data.Configurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(m => m.UserName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(m => m.Balance)
            .IsRequired();
    }
}