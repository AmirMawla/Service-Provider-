using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_DAL.Data
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context, UserManager<Vendor> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();
            // Add Categories
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { NameEn = "Food", NameAr = "الأكل" },
                    new Category { NameEn = "Beverages", NameAr = "الشرب" },
                    new Category { NameEn = "Clothing", NameAr = "الملابس" },
                    new Category { NameEn = "Electronics", NameAr = "الإلكترونيات" },
                    new Category { NameEn = "Health & Personal Care", NameAr = "الصحة والعناية الشخصية" },
                    new Category { NameEn = "Public Services", NameAr = "الخدمات العامة" },
                    new Category { NameEn = "Education & Training", NameAr = "التعليم والتدريب" },
                    new Category { NameEn = "Entertainment", NameAr = "الترفيه" },
                    new Category { NameEn = "Furniture", NameAr = "الأثاث" },
                    new Category { NameEn = "Automotive Services", NameAr = "خدمات السيارات" });

                await context.SaveChangesAsync();
            }


            if (!context.SubCategories.Any())
            {

                //Add SubCategories
                context.SubCategories.AddRange(
                // Subcategories for Food
                new SubCategory { NameEn = "Fast Food", NameAr = "الوجبات السريعة", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Bakeries", NameAr = "مخابز", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Seafood", NameAr = "مأكولات بحرية", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Snacks", NameAr = "الوجبات الخفيفة", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Organic Food", NameAr = "الأطعمة العضوية", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Dairy Products", NameAr = "منتجات الألبان", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },

                // Subcategories for Beverages
                new SubCategory { NameEn = "Juices", NameAr = "عصائر", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Soft Drinks", NameAr = "مشروبات غازية", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Tea", NameAr = "شاي", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Coffee Beans", NameAr = "حبوب القهوة", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Energy Drinks", NameAr = "مشروبات الطاقة", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },


                // Subcategories for Clothing
                new SubCategory { NameEn = "Men's Clothing", NameAr = "ملابس رجالية", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Women's Clothing", NameAr = "ملابس نسائية", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Children's Clothing", NameAr = "ملابس الأطفال", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Sportswear", NameAr = "الملابس الرياضية", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Accessories", NameAr = "الإكسسوارات", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },



                // Subcategories for Electronics
                new SubCategory { NameEn = "Mobile Phones", NameAr = "الهواتف المحمولة", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Laptops", NameAr = "الحواسيب المحمولة", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Gaming Consoles", NameAr = "أجهزة الألعاب", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Televisions", NameAr = "التلفزيونات", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Accessories", NameAr = "إكسسوارات", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Air Conditioners", NameAr = "أجهزة التكييف", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Refrigerators", NameAr = "الثلاجات", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Washing Machines", NameAr = "غسالات", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Microwave Ovens", NameAr = "الميكروويف", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Cameras", NameAr = "الكاميرات", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },


                // Subcategories for Health & Personal Care
                new SubCategory { NameEn = "Hair Salons", NameAr = "صالونات الشعر", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Fitness Centers", NameAr = "مراكز اللياقة البدنية", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Clinics", NameAr = "العيادات", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Pharmacies", NameAr = "الصيدليات", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Spas", NameAr = "منتجعات صحية", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },


                 // Subcategories for Public Services
                 new SubCategory { NameEn = "Electricity", NameAr = "الكهرباء", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Water Supply", NameAr = "إمدادات المياه", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Waste Collection", NameAr = "جمع النفايات", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Public Transportation", NameAr = "النقل العام", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Emergency Services", NameAr = "خدمات الطوارئ", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Postal Services", NameAr = "الخدمات البريدية", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },

                 // Subcategories for Education & Training
                 new SubCategory { NameEn = "Schools", NameAr = "مدارس", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Universities", NameAr = "جامعات", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Language Centers", NameAr = "مراكز تعليم اللغات", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Online Courses", NameAr = "الدورات عبر الإنترنت", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Skill Development", NameAr = "تطوير المهارات", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },


                 // Subcategories for Entertainment
                 new SubCategory { NameEn = "Cinemas", NameAr = "دور السينما", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Theme Parks", NameAr = "الملاهي", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Concerts", NameAr = "الحفلات الموسيقية", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Gaming Centers", NameAr = "مراكز الألعاب", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Theaters", NameAr = "المسارح", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },


                 // Subcategories for Furniture
                 new SubCategory { NameEn = "Living Room Furniture", NameAr = "أثاث غرفة المعيشة", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Bedroom Furniture", NameAr = "أثاث غرفة النوم", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Office Furniture", NameAr = "أثاث المكتب", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Outdoor Furniture", NameAr = "أثاث الحدائق", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Decor", NameAr = "الديكور", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },


                 // Subcategories for Automotive Services
                 new SubCategory { NameEn = "Car Wash", NameAr = "غسيل السيارات", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Tire Shops", NameAr = "محلات الإطارات", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Auto Repair", NameAr = "إصلاح السيارات", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Car Rentals", NameAr = "تأجير السيارات", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Car Accessories", NameAr = "إكسسوارات السيارات", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id });

                await context.SaveChangesAsync();
            }

            await SeedRoles(userManager, roleManager);

            await SeedAdminUser(userManager, roleManager);

        }





          private  static async Task SeedRoles(UserManager<Vendor> userManager, RoleManager<IdentityRole> roleManager)
          {
                 //if (!await roleManager.RoleExistsAsync("Admin"))
                 //{
                 //    var adminRole = new IdentityRole { Name = "Admin" };
                 //    await roleManager.CreateAsync(adminRole);
                 //}

                 //if (!await roleManager.RoleExistsAsync("Vendor"))
                 //{
                 //    var VendorRole = new IdentityRole { Name = "Vendor" };
                 //    await roleManager.CreateAsync(VendorRole);
                 //}
                 if (!await roleManager.RoleExistsAsync("Admin"))
                 {
                     var adminRole = new IdentityRole { Name = "Admin", ConcurrencyStamp = Guid.NewGuid().ToString() };
                     await roleManager.CreateAsync(adminRole);
                 }
                 else
                 {
                     var existingAdminRole = await roleManager.FindByNameAsync("Admin");
                     if (existingAdminRole?.ConcurrencyStamp == null)
                     {
                         existingAdminRole!.ConcurrencyStamp = Guid.NewGuid().ToString();
                         await roleManager.UpdateAsync(existingAdminRole);
                     }
                 }

                 if (!await roleManager.RoleExistsAsync("Vendor"))
                 {
                     var vendorRole = new IdentityRole { Name = "Vendor", ConcurrencyStamp = Guid.NewGuid().ToString() };
                     await roleManager.CreateAsync(vendorRole);
                 }
                 else
                 {
                     var existingVendorRole = await roleManager.FindByNameAsync("Vendor");
                     if (existingVendorRole?.ConcurrencyStamp == null)
                     {
                         existingVendorRole!.ConcurrencyStamp = Guid.NewGuid().ToString();
                         await roleManager.UpdateAsync(existingVendorRole);
                     }
                 }

          }

           private  static async Task SeedAdminUser(UserManager<Vendor> userManager, RoleManager<IdentityRole> roleManager)
           {
                var adminUserName = "admin";
                var adminUser = await userManager.FindByNameAsync(adminUserName);
                if (adminUser == null)
                {
                    adminUser = new Vendor
                    {
                        UserName = adminUserName,
                        FullName = adminUserName,
                        BusinessType =adminUserName,
                        Email = "admin@Vendor.com",
                        EmailConfirmed = true,
                        IsApproved = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }

                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine(error.Description);
                        }
                    }
                }
           }
    }
}

    

