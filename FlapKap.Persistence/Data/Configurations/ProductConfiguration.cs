using FlapKap.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlapKap.Persistence.Data.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(m => m.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(p => p.AmountAvailable)
               .IsRequired();

        builder.Property(p => p.Cost)
               .IsRequired()
               .HasColumnType("decimal(18,2)");

        builder.HasOne(p => p.Seller)
               .WithMany(p => p.Products)
               .HasForeignKey(p => p.SellerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}