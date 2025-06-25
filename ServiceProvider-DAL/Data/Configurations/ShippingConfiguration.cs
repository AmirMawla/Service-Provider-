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
    public class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
    {
        public void Configure(EntityTypeBuilder<Shipping> builder)
        {
            builder.HasKey(s =>new {s.OrderId,s.VendorId});

            builder.Property(s => s.Status)
                .HasConversion(
                c => c.ToString(),
                c => (ShippingStatus)Enum.Parse(typeof(ShippingStatus), c)
                );


            builder.HasOne(s => s.Order)
                .WithMany(o => o.Shippings)
                .HasForeignKey(s => s.OrderId)
                .IsRequired();

            builder.HasOne(s => s.Vendor)
               .WithMany(v => v.Shippings)
               .HasForeignKey(s => s.VendorId)
               .IsRequired();

        }
    }
}
