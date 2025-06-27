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
    internal class BannersConfiguration : IEntityTypeConfiguration<Banners>
    {
        public void Configure(EntityTypeBuilder<Banners> builder)
        {
            builder.HasKey(b => new {b.ProductId,b.VendorId,b.DiscountCode});

            builder.Property(b => b.Description)
               .HasMaxLength(500);

            builder.Property(b => b.DiscountCode)
              .HasMaxLength(50);

            builder.HasOne(b => b.Product)
                   .WithMany(p => p.Banners)
                   .HasForeignKey(b => b.ProductId);

            builder.HasOne(b => b.Vendor)
                   .WithMany(v => v.Banners)
                   .HasForeignKey(b => b.VendorId);

            builder.Property(b => b.DiscountPercentage)
                      .HasColumnType("decimal(18,2)");


        }
    }
}
