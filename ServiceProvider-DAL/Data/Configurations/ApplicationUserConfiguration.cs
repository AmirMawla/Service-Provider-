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
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).ValueGeneratedNever();

            builder.Property(u => u.FullName).IsRequired().HasMaxLength(100);

            builder.Property(u => u.Address).HasMaxLength(250);

            builder.Property(u => u.BirthDate)
                .HasColumnType("date");

            builder.Property(u => u.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.HasMany(u => u.Messages)
                   .WithOne(m => m.User)
                   .HasForeignKey(m => m.ApplicationUserId);

            builder.HasMany(u => u.Reviews)
                   .WithOne(r => r.User)
                   .HasForeignKey(r => r.ApplicationUserId);

            builder.HasMany(u => u.Orders)
                   .WithOne(o => o.User)
                   .HasForeignKey(r => r.ApplicationUserId);

            builder.HasData(new List<ApplicationUser>
            {
                new ApplicationUser
                { Id = "abcdefg",
                  FullName = "ahmed tahoon",
                  Email = "ahmed@email.com",
                  Address = "sharkia",
                  PhoneNumber = "01002694473"
                },
                 new ApplicationUser
                { Id = "abcdedvdvfdsfgh",
                  FullName = "amir elsayed",
                  Email = "amir@email.com",
                  Address = "sharkia",
                  PhoneNumber = "01002694473"
                },
                  new ApplicationUser
                { Id = "fsjnvjkdsfjsfnvf",
                  FullName = "Hossam mostafa",
                  Email = "hossam@email.com",
                  Address = "sharkia",
                  PhoneNumber = "01002694473"
                },
                new ApplicationUser
                { Id = "jndsjknfjmfifimkf",
                  FullName = "ahmed fathi",
                  Email = "fathi@email.com",
                  Address = "sharkia",
                  PhoneNumber = "01002694473"
                }
            });
        }
    }
}
