using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ServiceProvider_DAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServiceProvider_DAL.Data
{
    public class AppDbContext : IdentityDbContext<Vendor,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<ApplicationUser>? ApplicationUsers { get; set; }
        public DbSet<Cart>? Carts { get; set; }
        public DbSet<CartProduct>? CartProducts { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<SubCategory>? SubCategories { get; set; }
        public DbSet<Message>? Messages { get; set; }
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<OrderProduct>? OrderProducts { get; set; }
        public DbSet<Payment>? Payments { get; set; }
        public DbSet<Review>? Reviews { get; set; }
        public DbSet<Shipping>? Shippings { get; set; }
        public DbSet<VendorSubCategory>? VendorSubCategories { get; set; }
        public DbSet<Banners>? Banners { get; set; }
       

    }
}
