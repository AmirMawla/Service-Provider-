using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Data.Configurations
{
    internal class VendorConfiguration : IEntityTypeConfiguration<Vendor>
    {
        public void Configure(EntityTypeBuilder<Vendor> builder)
        {

            builder.Property(v => v.BusinessName).HasMaxLength(200);
            builder.Property(v => v.BusinessType).HasMaxLength(100);
            builder.Property(v => v.TaxNumber).HasMaxLength(50);

            builder.OwnsMany(x => x.RefreshTokens)
                    .ToTable("RefreshTokens")
                    .WithOwner()
                    .HasForeignKey("VendorId");

            builder.HasMany(v => v.Products)
                   .WithOne(p => p.Vendor)
                   .HasForeignKey(p => p.VendorId);

            builder.HasMany(v => v.VendorSubCategories)
                   .WithOne(vs => vs.Vendor)
                   .HasForeignKey(vs => vs.VendorId);

            builder.HasMany(v => v.Messages)
                   .WithOne(m => m.Vendor)
                   .HasForeignKey(m => m.VendorId);
        }
    }
}
