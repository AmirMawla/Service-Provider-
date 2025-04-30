using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                    new Category { NameEn = "Food", NameAr = "الأكل", ImageUrl = "/images/categories/food.jpg" },
                    new Category { NameEn = "Beverages", NameAr = "الشرب", ImageUrl = "/images/categories/Beverages.jpg" },
                    new Category { NameEn = "Clothing", NameAr = "الملابس", ImageUrl = "/images/categories/Clothing.jpg" },
                    new Category { NameEn = "Electronics", NameAr = "الإلكترونيات", ImageUrl = "/images/categories/Electronics.jpg" },
                    new Category { NameEn = "Health & Personal Care", NameAr = "الصحة والعناية الشخصية", ImageUrl = "/images/categories/Health & Personal Care.jpg" },
                    new Category { NameEn = "Public Services", NameAr = "الخدمات العامة", ImageUrl = "/images/categories/Public Services.jpg" },
                    new Category { NameEn = "Education & Training", NameAr = "التعليم والتدريب", ImageUrl = "/images/categories/Education & Training.jpg" },
                    new Category { NameEn = "Entertainment", NameAr = "الترفيه", ImageUrl = "/images/categories/Entertainment.jpg" },
                    new Category { NameEn = "Furniture", NameAr = "الأثاث", ImageUrl = "/images/categories/Furniture.jpg" },
                    new Category { NameEn = "Automotive Services", NameAr = "خدمات السيارات", ImageUrl = "/images/categories/Automotive Services.jpg" });

                await context.SaveChangesAsync();
            }


            if (!context.SubCategories.Any())
            {

                //Add SubCategories
                context.SubCategories.AddRange(
                // Subcategories for Food
                new SubCategory { NameEn = "Fast Food", NameAr = "الوجبات السريعة", ImageUrl = "/images/subcategories/Fast Food.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Bakeries", NameAr = "مخابز", ImageUrl = "/images/subcategories/Bakeries.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Seafood", NameAr = "مأكولات بحرية", ImageUrl = "/images/subcategories/Seafood.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Snacks", NameAr = "الوجبات الخفيفة", ImageUrl = "/images/subcategories/Snacks.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Organic Food", NameAr = "الأطعمة العضوية", ImageUrl = "/images/subcategories/Organic Food.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },
                new SubCategory { NameEn = "Dairy Products", NameAr = "منتجات الألبان", ImageUrl = "/images/subcategories/Dairy Products.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Food").Id },

                // Subcategories for Beverages
                new SubCategory { NameEn = "Juices", NameAr = "عصائر", ImageUrl = "/images/subcategories/Juices.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Soft Drinks", NameAr = "مشروبات غازية", ImageUrl = "/images/subcategories/Soft Drinks.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Tea", NameAr = "شاي", ImageUrl = "/images/subcategories/Tea.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Coffee Beans", NameAr = "حبوب القهوة", ImageUrl = "/images/subcategories/Coffee Beans.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },
                new SubCategory { NameEn = "Energy Drinks", NameAr = "مشروبات الطاقة", ImageUrl = "/images/subcategories/Energy Drinks.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Beverages").Id },


                // Subcategories for Clothing
                new SubCategory { NameEn = "Men's Clothing", NameAr = "ملابس رجالية", ImageUrl = "/images/subcategories/Men's Clothing.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Women's Clothing", NameAr = "ملابس نسائية", ImageUrl = "/images/subcategories/Women's Clothing.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Children's Clothing", NameAr = "ملابس الأطفال", ImageUrl = "/images/subcategories/Children's Clothing.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Sportswear", NameAr = "الملابس الرياضية", ImageUrl = "/images/subcategories/Sportswear.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },
                new SubCategory { NameEn = "Accessories", NameAr = "الإكسسوارات", ImageUrl = "/images/subcategories/Accessories.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Clothing").Id },



                // Subcategories for Electronics
                new SubCategory { NameEn = "Mobile Phones", NameAr = "الهواتف المحمولة", ImageUrl = "/images/subcategories/Mobile Phones.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Laptops", NameAr = "الحواسيب المحمولة", ImageUrl = "/images/subcategories/Laptops.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Gaming Consoles", NameAr = "أجهزة الألعاب", ImageUrl = "/images/subcategories/Gaming Consoles.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Televisions", NameAr = "التلفزيونات", ImageUrl = "/images/subcategories/Televisions.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Accessories", NameAr = "إكسسوارات", ImageUrl = "/images/subcategories/Accessories Electronics.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Air Conditioners", NameAr = "أجهزة التكييف", ImageUrl = "/images/subcategories/Air Conditioners.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Refrigerators", NameAr = "الثلاجات", ImageUrl = "/images/subcategories/Refrigerators.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Washing Machines", NameAr = "غسالات", ImageUrl = "/images/subcategories/Washing Machines.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Microwave Ovens", NameAr = "الميكروويف", ImageUrl = "/images/subcategories/Microwave Ovens.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },
                new SubCategory { NameEn = "Cameras", NameAr = "الكاميرات", ImageUrl = "/images/subcategories/Cameras.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Electronics").Id },


                // Subcategories for Health & Personal Care
                new SubCategory { NameEn = "Hair Salons", NameAr = "صالونات الشعر", ImageUrl = "/images/subcategories/Hair Salons.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Fitness Centers", NameAr = "مراكز اللياقة البدنية", ImageUrl = "/images/subcategories/Fitness Centers.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Clinics", NameAr = "العيادات", ImageUrl = "/images/subcategories/Clinics.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Pharmacies", NameAr = "الصيدليات", ImageUrl = "/images/subcategories/Pharmacies.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },
                new SubCategory { NameEn = "Spas", NameAr = "منتجعات صحية", ImageUrl = "/images/subcategories/Spas.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Health & Personal Care").Id },


                 // Subcategories for Public Services
                 new SubCategory { NameEn = "Electricity", NameAr = "الكهرباء", ImageUrl = "/images/subcategories/Electricity.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Water Supply", NameAr = "إمدادات المياه", ImageUrl = "/images/subcategories/Water Supply.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Waste Collection", NameAr = "جمع النفايات", ImageUrl = "/images/subcategories/Waste Collection.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Public Transportation", NameAr = "النقل العام", ImageUrl = "/images/subcategories/Public Transportation.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Emergency Services", NameAr = "خدمات الطوارئ", ImageUrl = "/images/subcategories/Emergency Services.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },
                 new SubCategory { NameEn = "Postal Services", NameAr = "الخدمات البريدية", ImageUrl = "/images/subcategories/Postal Services.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Public Services").Id },

                 // Subcategories for Education & Training
                 new SubCategory { NameEn = "Schools", NameAr = "مدارس", ImageUrl = "/images/subcategories/Schools.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Universities", NameAr = "جامعات", ImageUrl = "/images/subcategories/Universities.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Language Centers", NameAr = "مراكز تعليم اللغات", ImageUrl = "/images/subcategories/Language Centers.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Online Courses", NameAr = "الدورات عبر الإنترنت", ImageUrl = "/images/subcategories/Online Courses.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },
                 new SubCategory { NameEn = "Skill Development", NameAr = "تطوير المهارات", ImageUrl = "/images/subcategories/Skill Development.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Education & Training").Id },


                 // Subcategories for Entertainment
                 new SubCategory { NameEn = "Cinemas", NameAr = "دور السينما", ImageUrl = "/images/subcategories/Cinemas.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Theme Parks", NameAr = "الملاهي", ImageUrl = "/images/subcategories/Theme Parks.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Concerts", NameAr = "الحفلات الموسيقية", ImageUrl = "/images/subcategories/Concerts.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Gaming Centers", NameAr = "مراكز الألعاب", ImageUrl = "/images/subcategories/Gaming Centers.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },
                 new SubCategory { NameEn = "Theaters", NameAr = "المسارح", ImageUrl = "/images/subcategories/Theaters.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Entertainment").Id },


                 // Subcategories for Furniture
                 new SubCategory { NameEn = "Living Room Furniture", NameAr = "أثاث غرفة المعيشة", ImageUrl = "/images/subcategories/Living Room Furniture.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Bedroom Furniture", NameAr = "أثاث غرفة النوم", ImageUrl = "/images/subcategories/Bedroom Furniture.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Office Furniture", NameAr = "أثاث المكتب", ImageUrl = "/images/subcategories/Office Furniture.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Outdoor Furniture", NameAr = "أثاث الحدائق", ImageUrl = "/images/subcategories/Outdoor Furniture.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },
                 new SubCategory { NameEn = "Decor", NameAr = "الديكور", ImageUrl = "/images/subcategories/Decor.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },


                 // Subcategories for Automotive Services
                 new SubCategory { NameEn = "Car Wash", NameAr = "غسيل السيارات", ImageUrl = "/images/subcategories/Car Wash.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Tire Shops", NameAr = "محلات الإطارات", ImageUrl = "/images/subcategories/Tire Shops.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Auto Repair", NameAr = "إصلاح السيارات", ImageUrl = "/images/subcategories/Auto Repair.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Car Rentals", NameAr = "تأجير السيارات", ImageUrl = "/images/subcategories/Car Rentals.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id },
                 new SubCategory { NameEn = "Car Accessories", NameAr = "إكسسوارات السيارات", ImageUrl = "/images/subcategories/Car Accessories.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Automotive Services").Id });

                await context.SaveChangesAsync();
            }



            //add seeddata for vendors
            var vendorsData = new List<Vendor>
    {
        new Vendor
        {
            UserName = "alyssa.ramirez@gmail.com",
            Email = "alyssa.ramirez@gmail.com",
            FullName = "Alyssa Ramirez",
            BusinessName = "Hawkins Supplies",
            BusinessType = "Electronics",
            TaxNumber = "123456789",
            Rating = 4.3f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile1.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "omar.tarek@citymail.com",
            Email = "omar.tarek@citymail.com",
            FullName = "Omar Tarek",
            BusinessName = "Tarek Tech",
            BusinessType = "Mobile Devices",
            TaxNumber = "987654321",
            Rating = 4.8f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile2.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "sarah.hany@stylehub.com",
            Email = "sarah.hany@stylehub.com",
            FullName = "Sarah Hany",
            BusinessName = "Style Hub",
            BusinessType = "Fashion Retail",
            TaxNumber = "1122334455",
            Rating = 4.5f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile3.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "ahmed.kamal@homefix.com",
            Email = "ahmed.kamal@homefix.com",
            FullName = "Ahmed Kamal",
            BusinessName = "Home Fix",
            BusinessType = "Home Services",
            TaxNumber = "1029384756",
            Rating = 4.2f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile4.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "lina.saeed@beautycare.com",
            Email = "lina.saeed@beautycare.com",
            FullName = "Lina Saeed",
            BusinessName = "Beauty Care",
            BusinessType = "Cosmetics",
            TaxNumber = "1092837465",
            Rating = 4.9f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile5.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "karim.fathy@toolbox.com",
            Email = "karim.fathy@toolbox.com",
            FullName = "Karim Fathy",
            BusinessName = "ToolBox",
            BusinessType = "Hardware Tools",
            TaxNumber = "1192928383",
            Rating = 4.1f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile6.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "nancy.adel@flowerify.com",
            Email = "nancy.adel@flowerify.com",
            FullName = "Nancy Adel",
            BusinessName = "Flowerify",
            BusinessType = "Florist",
            TaxNumber = "1282828282",
            Rating = 4.6f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile7.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "mohamed.ali@carfix.com",
            Email = "mohamed.ali@carfix.com",
            FullName = "Mohamed Ali",
            BusinessName = "CarFix",
            BusinessType = "Auto Service",
            TaxNumber = "1982736456",
            Rating = 4.4f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile8.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "dalia.nour@kidsfun.com",
            Email = "dalia.nour@kidsfun.com",
            FullName = "Dalia Nour",
            BusinessName = "Kids Fun",
            BusinessType = "Toys & Games",
            TaxNumber = "2374829384",
            Rating = 4.7f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile9.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "hassan.mostafa@fitlife.com",
            Email = "hassan.mostafa@fitlife.com",
            FullName = "Hassan Mostafa",
            BusinessName = "Fit Life",
            BusinessType = "Sports Equipment",
            TaxNumber = "2847562837",
            Rating = 4.5f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile10.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "mona.samir@decorworld.com",
            Email = "mona.samir@decorworld.com",
            FullName = "Mona Samir",
            BusinessName = "Decor World",
            BusinessType = "Interior Design",
            TaxNumber = "9483726383",
            Rating = 4.6f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile11.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "tamer.ragab@fixit.com",
            Email = "tamer.ragab@fixit.com",
            FullName = "Tamer Ragab",
            BusinessName = "FixIt",
            BusinessType = "Plumbing Services",
            TaxNumber = "8374628372",
            Rating = 4.3f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile12.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "yasmin.gamal@petslove.com",
            Email = "yasmin.gamal@petslove.com",
            FullName = "Yasmin Gamal",
            BusinessName = "Pets Love",
            BusinessType = "Pet Store",
            TaxNumber = "1627384950",
            Rating = 4.8f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile13.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "khaled.farag@techserve.com",
            Email = "khaled.farag@techserve.com",
            FullName = "Khaled Farag",
            BusinessName = "TechServe",
            BusinessType = "IT Services",
            TaxNumber = "7162738491",
            Rating = 4.7f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile14.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "samar.fouad@bookland.com",
            Email = "samar.fouad@bookland.com",
            FullName = "Samar Fouad",
            BusinessName = "BookLand",
            BusinessType = "Book Store",
            TaxNumber = "6248392012",
            Rating = 4.4f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile15.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "fady.nashed@greenzone.com",
            Email = "fady.nashed@greenzone.com",
            FullName = "Fady Nashed",
            BusinessName = "Green Zone",
            BusinessType = "Gardening",
            TaxNumber = "1039484756",
            Rating = 4.6f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile16.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "dina.maged@salonplus.com",
            Email = "dina.maged@salonplus.com",
            FullName = "Dina Maged",
            BusinessName = "Salon Plus",
            BusinessType = "Beauty Salon",
            TaxNumber = "3847562938",
            Rating = 4.9f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile17.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "nour.youssef@craftify.com",
            Email = "nour.youssef@craftify.com",
            FullName = "Nour Youssef",
            BusinessName = "Craftify",
            BusinessType = "Handmade Crafts",
            TaxNumber = "9384756382",
            Rating = 4.8f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile18.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "mohamed.amin@cleanit.com",
            Email = "mohamed.amin@cleanit.com",
            FullName = "Mohamed Amin",
            BusinessName = "CleanIt",
            BusinessType = "Cleaning Services",
            TaxNumber = "7463829102",
            Rating = 4.2f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile19.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        },
        new Vendor
        {
            UserName = "hanaa.ahmed@eventplus.com",
            Email = "hanaa.ahmed@eventplus.com",
            FullName = "Hanaa Ahmed",
            BusinessName = "Event Plus",
            BusinessType = "Event Planning",
            TaxNumber = "8372918372",
            Rating = 4.9f,
            IsApproved = true,
            ProfilePictureUrl = "/images/vendors/profile20.jpg",
            CoverImageUrl = "/images/vendors/StartCover.jpg"
        }
    };

            foreach (var vendor in vendorsData)
            {
                var existing = await userManager.FindByEmailAsync(vendor.Email);
                if (existing == null)
                {
                    await userManager.CreateAsync(vendor, "P@ssword123");
                    await userManager.AddToRoleAsync(vendor, "Vendor");
                }
            }

            if (!context.Products.Any())
            {
                await context.Products.AddRangeAsync(new Product
                {
                    NameEn = "Samsung Galaxy S23",
                    NameAr = "سامسونج جالاكسي S23",
                    Description = "Latest Samsung flagship smartphone with powerful performance and camera.",
                    Price = 999.99m,
                    MainImageUrl = "/images/products/Samsung Galaxy S23.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Mobile Phones").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Tarek Tech").Id
                },
                new Product
                {
                    NameEn = "Dell XPS 13",
                    NameAr = "ديل اكس بي اس 13",
                    Description = "Premium ultra-portable laptop with excellent display and build quality.",
                    Price = 1499.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Laptops").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Hawkins Supplies").Id
                },

                // Fashion
                new Product
                {
                    NameEn = "Men's Leather Jacket",
                    NameAr = "جاكيت جلدي رجالي",
                    Description = "Stylish leather jacket for men made from genuine leather.",
                    Price = 199.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Men's Clothing").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Style Hub").Id
                },
                new Product
                {
                    NameEn = "Women's Evening Dress",
                    NameAr = "فستان سهرة نسائي",
                    Description = "Elegant evening dress perfect for special occasions.",
                    Price = 249.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Women's Clothing").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Style Hub").Id
                },

                // Home Services
                new Product
                {
                    NameEn = "Home AC Repair",
                    NameAr = "تصليح مكيفات منزلية",
                    Description = "Professional home air conditioning repair service.",
                    Price = 100.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Air Conditioners").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Home Fix").Id
                },

                // Health & Personal Care
                new Product
                {
                    NameEn = "Spa Relaxation Package",
                    NameAr = "باقة استرخاء السبا",
                    Description = "Full body massage and spa services for ultimate relaxation.",
                    Price = 180.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Spas").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Beauty Care").Id
                },

                // Automotive Services
                new Product
                {
                    NameEn = "Car Tire Replacement",
                    NameAr = "تبديل إطارات السيارة",
                    Description = "High-quality tire replacement service for your vehicle.",
                    Price = 300.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Tire Shops").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "CarFix").Id
                },

                // Furniture
                new Product
                {
                    NameEn = "Modern Sofa",
                    NameAr = "كنبة عصرية",
                    Description = "Stylish modern sofa with comfortable seating.",
                    Price = 799.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Living Room Furniture").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Decor World").Id
                },

                // Toys & Games
                new Product
                {
                    NameEn = "Kids' Playhouse",
                    NameAr = "منزل ألعاب للأطفال",
                    Description = "Colorful and safe playhouse for kids' fun activities.",
                    Price = 120.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Children's Clothing").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Kids Fun").Id
                },
                // Electronics
                new Product
                {
                    NameEn = "Apple MacBook Pro 14",
                    NameAr = "آبل ماك بوك برو 14",
                    Description = "Powerful laptop with M2 Pro chip for professionals.",
                    Price = 1999.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Laptops").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Hawkins Supplies").Id
                },
                new Product
                {
                    NameEn = "Sony WH-1000XM5 Headphones",
                    NameAr = "سوني WH-1000XM5 سماعات",
                    Description = "Industry leading noise cancelling wireless headphones.",
                    Price = 399.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameAr == "الإكسسوارات").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Tarek Tech").Id
                },

                // Fashion
                new Product
                {
                    NameEn = "Women's Leather Handbag",
                    NameAr = "حقيبة يد جلدية نسائية",
                    Description = "Premium leather handbag stylish and durable.",
                    Price = 129.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Women's Clothing").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Style Hub").Id
                },

                new Product
                {
                    NameEn = "Men's Running Shoes",
                    NameAr = "أحذية جري رجالية",
                    Description = "Comfortable and lightweight running shoes.",
                    Price = 89.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Men's Clothing").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Style Hub").Id
                },

                new Product
                {
                    NameEn = "Electrical Wiring Repair",
                    NameAr = "تصليح أسلاك كهربائية",
                    Description = "Safe and reliable electrical wiring repair services.",
                    Price = 220.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Auto Repair").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Home Fix").Id
                },

                // Health & Personal Care
                new Product
                {
                    NameEn = "Haircut and Styling",
                    NameAr = "قص وتصفيف الشعر",
                    Description = "Professional haircut and styling services for men and women.",
                    Price = 50.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Hair Salons").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Beauty Care").Id
                },

                new Product
                {
                    NameEn = "Facial Treatment",
                    NameAr = "علاج الوجه",
                    Description = "Rejuvenating facial treatment for glowing skin.",
                    Price = 120.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Spas").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Beauty Care").Id
                },

                // Furniture
                new Product
                {
                    NameEn = "Queen Size Bed",
                    NameAr = "سرير حجم كوين",
                    Description = "Comfortable and spacious queen size bed frame.",
                    Price = 1199.99m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Bedroom Furniture").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Decor World").Id
                },

                // Toys & Games
                new Product
                {
                    NameEn = "Remote Control Car",
                    NameAr = "سيارة بالتحكم عن بعد",
                    Description = "High-speed RC car for kids and adults.",
                    Price = 75.00m,
                    MainImageUrl = "/images/products/test.jpg",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    SubCategoryId = context.SubCategories.First(sc => sc.NameEn == "Gaming Centers").Id,
                    VendorId = userManager.Users.First(v => v.BusinessName == "Kids Fun").Id
                });
                await context.SaveChangesAsync();
            }
           
            if (context.VendorSubCategories.Count() < 5)
            {
                // استرجاع كل الفيندورز من قاعدة البيانات
                var vendors = userManager.Users.ToList();

                // استرجاع كل الـ subcategories من قاعدة البيانات
                var subCategories = context.SubCategories.ToList();

                // قائمة لتخزين علاقات Vendor-SubCategory التي ستتم إضافتها إلى الجدول
                var vendorSubCategories = new List<VendorSubCategory>();

                // حلقات لربط كل فيندور بكل subcategory التي تناسبه بناءً على التوافق المنطقي
                foreach (var subCategory in subCategories)
                {
                    // يمكن إضافة منطق معين هنا لتحديد ما إذا كان هذا الفيندور ينتمي إلى هذه الـ subcategory
                    foreach (var vendor in vendors)
                    {
                        bool shouldLink = false;

                        // منطق تحديد التوافق بين الفيندور والـ subcategory
                        if (subCategory.NameEn == "Fast Food" && vendor.BusinessType == "Food")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Bakeries" && vendor.BusinessType == "Food")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Seafood" && vendor.BusinessType == "Food")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Juices" && vendor.BusinessType == "Beverages")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Soft Drinks" && vendor.BusinessType == "Beverages")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Tea" && vendor.BusinessType == "Beverages")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Mobile Phones" && vendor.BusinessType == "Electronics")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Laptops" && vendor.BusinessType == "Electronics")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Men's Clothing" && vendor.BusinessType == "Clothing")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Women's Clothing" && vendor.BusinessType == "Clothing")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Hair Salons" && vendor.BusinessType == "Health & Personal Care")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Fitness Centers" && vendor.BusinessType == "Health & Personal Care")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Electricity" && vendor.BusinessType == "Public Services")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Water Supply" && vendor.BusinessType == "Public Services")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Cinemas" && vendor.BusinessType == "Entertainment")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Theme Parks" && vendor.BusinessType == "Entertainment")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Living Room Furniture" && vendor.BusinessType == "Furniture")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Bedroom Furniture" && vendor.BusinessType == "Furniture")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Car Wash" && vendor.BusinessType == "Automotive Services")
                            shouldLink = true;
                        else if (subCategory.NameEn == "Tire Shops" && vendor.BusinessType == "Automotive Services")
                            shouldLink = true;

                        // إذا كان هناك تطابق، نربط الفيندور بالـ subCategory
                        if (shouldLink)
                        {
                            vendorSubCategories.Add(new VendorSubCategory
                            {
                                VendorId = vendor.Id,
                                SubCategoryId = subCategory.Id
                            });
                        }
                    }
                }

                // إضافة جميع العلاقات بين الفيندورز والـ subcategories إلى جدول VendorSubCategory
              await  context.VendorSubCategories.AddRangeAsync(vendorSubCategories);

                // حفظ التغييرات في قاعدة البيانات
               await context.SaveChangesAsync();
            }







            await SeedRoles(userManager, roleManager);

            await SeedAdminUser(userManager, roleManager);

            //await SeedApplicationUsers(context);
            await SeedCarts(context);
            await SeedOrders(context);
            await SeedReviews(context);
            await SeedShipping(context);
            await SeedBanners(context);
        }





        private static async Task SeedRoles(UserManager<Vendor> userManager, RoleManager<IdentityRole> roleManager)
        {
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

        private static async Task SeedAdminUser(UserManager<Vendor> userManager, RoleManager<IdentityRole> roleManager)
        {
            var adminUserName = "admin";
            var adminUser = await userManager.FindByNameAsync(adminUserName);
            if (adminUser == null)
            {
                adminUser = new Vendor
                {
                    UserName = adminUserName,
                    FullName = adminUserName,
                    BusinessType = adminUserName,
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

    //    public static async Task SeedApplicationUsers(AppDbContext context)
    //    {
    //        if (context.ApplicationUsers.Count() > 6) return;

    //        var users = new List<ApplicationUser>
    //{
    //    new ApplicationUser
    //    {
    //        FullName = "Ahmed El Sayed",
    //        Email = "ahmed@example.com",
    //        Address = "123 Cairo St.",
    //        PhoneNumber = "01012345678",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1990, 5, 10)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Mona Ali",
    //        Email = "mona@example.com",
    //        Address = "456 Alexandria Rd.",
    //        PhoneNumber = "01123456789",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1985, 7, 20)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Sara Mohamed",
    //        Email = "sara@example.com",
    //        Address = "789 Giza St.",
    //        PhoneNumber = "01234567890",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1992, 3, 15)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Omar Tarek",
    //        Email = "omar@example.com",
    //        Address = "101 Mansoura St.",
    //        PhoneNumber = "01087654321",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1988, 8, 2)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Nadia Hassan",
    //        Email = "nadia@example.com",
    //        Address = "25 Nasr City St.",
    //        PhoneNumber = "01556712345",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1995, 11, 18)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Tamer Ali",
    //        Email = "tamer@example.com",
    //        Address = "36 Zamalek St.",
    //        PhoneNumber = "01065432109",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1990, 4, 20)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Laila Farouk",
    //        Email = "laila@example.com",
    //        Address = "88 Downtown St.",
    //        PhoneNumber = "01187654322",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1987, 2, 28)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Mohamed Ashraf",
    //        Email = "mohamed@example.com",
    //        Address = "77 Maadi St.",
    //        PhoneNumber = "01054321678",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1989, 9, 12)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Fayza Mahmoud",
    //        Email = "fayza@example.com",
    //        Address = "13 Mohandeseen St.",
    //        PhoneNumber = "01233456789",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1993, 1, 5)
    //    },
    //    new ApplicationUser
    //    {
    //        FullName = "Khaled Mohamed",
    //        Email = "khaled@example.com",
    //        Address = "44 Giza St.",
    //        PhoneNumber = "01012345567",
    //        ImageUrl ="/images/vendors/OIP.jpg",
    //        RegistrationDate = DateTime.UtcNow,
    //        BirthDate = new DateOnly(1986, 10, 15)
    //    }
    //};

    //        await context.ApplicationUsers.AddRangeAsync(users);
    //        await context.SaveChangesAsync();
    //    }


        public static async Task SeedCarts(AppDbContext context)
        {
            if (context.Carts.Count() > 3) return;
            var users = context.ApplicationUsers.ToList();
            var products = context.Products.ToList();
    

           await context.Carts.AddRangeAsync(new Cart
           {
               ApplicationUserId = users.First().Id,
               CreatedAt = DateTime.UtcNow,
               UpdatedAt = DateTime.UtcNow,
               CartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = products.First().Id, Quantity = 2 },
                new CartProduct { ProductId = products.Skip(1).First().Id, Quantity = 1 }
            }
           },
        new Cart
        {
            ApplicationUserId = users.Skip(1).First().Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = products.Skip(2).First().Id, Quantity = 3 },
                new CartProduct { ProductId = products.Skip(3).First().Id, Quantity = 1 }
            }
        },
        new Cart
        {
            ApplicationUserId = users.Skip(2).First().Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = products.Skip(4).First().Id, Quantity = 1 },
                new CartProduct { ProductId = products.Skip(3).First().Id, Quantity = 2 }
            }
        },
        new Cart
        {
            ApplicationUserId = users.Skip(3).First().Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CartProducts = new List<CartProduct>
            {
                new CartProduct { ProductId = products.Skip(1).First().Id, Quantity = 3 },
                new CartProduct { ProductId = products.Skip(2).First().Id, Quantity = 1 }
            }
        });
          await  context.SaveChangesAsync();
        }



        public static async Task SeedOrders(AppDbContext context)
        {
            if (context.Orders.Count() > 5) return;

            var users = context.ApplicationUsers.ToList();
            var products = context.Products.ToList();

            await context.Orders.AddRangeAsync(new Order
            {
                TotalAmount = 1099.99m,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                ApplicationUserId = users.First().Id,
                OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.First().Id, Quantity = 2 },
                new OrderProduct { ProductId = products.Skip(1).First().Id, Quantity = 1 }
            },
                Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5), Status = "Pending" },
                Payment = new Payment { TotalAmount = 1099.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Pending, PaymentMethod = "Credit Card" }
            },
        new Order
        {
            TotalAmount = 1300.99m,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Processing,
            ApplicationUserId = users.Skip(1).First().Id,
            OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(2).First().Id, Quantity = 2 },
                new OrderProduct { ProductId = products.Skip(3).First().Id, Quantity = 2 }
            },
            Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
            Payment = new Payment { TotalAmount = 1300.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
        },
             new Order
             {
                 TotalAmount = 1599.99m,
                 OrderDate = DateTime.UtcNow,
                 Status = OrderStatus.Processing,
                 ApplicationUserId = users.Skip(2).First().Id,
                 OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(4).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(5).First().Id, Quantity = 1 }
            },
                 Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                 Payment = new Payment { TotalAmount = 1599.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
             },
                  new Order
                  {
                      TotalAmount = 1800.99m,
                      OrderDate = DateTime.UtcNow,
                      Status = OrderStatus.Processing,
                      ApplicationUserId = users.Skip(3).First().Id,
                      OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(6).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(7).First().Id, Quantity = 1 }
            },
                      Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                      Payment = new Payment { TotalAmount = 1800.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                  },
                       new Order
                       {
                           TotalAmount = 899.99m,
                           OrderDate = DateTime.UtcNow,
                           Status = OrderStatus.Processing,
                           ApplicationUserId = users.Skip(4).First().Id,
                           OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(6).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(1).First().Id, Quantity = 1 }
            },
                           Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                           Payment = new Payment { TotalAmount = 899.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                       },
                        new Order
                        {
                            TotalAmount = 2300.99m,
                            OrderDate = DateTime.UtcNow,
                            Status = OrderStatus.Delivered,
                            ApplicationUserId = users.Skip(5).First().Id,
                            OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(6).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(2).First().Id, Quantity = 5 }
            },
                            Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                            Payment = new Payment { TotalAmount = 2300.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                        },
                         new Order
                         {
                             TotalAmount = 1500.99m,
                             OrderDate = DateTime.UtcNow,
                             Status = OrderStatus.Processing,
                             ApplicationUserId = users.Skip(6).First().Id,
                             OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(6).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(3).First().Id, Quantity = 2 }
            },
                             Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                             Payment = new Payment { TotalAmount = 1500.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                         },
                          new Order
                          {
                              TotalAmount = 3700.99m,
                              OrderDate = DateTime.UtcNow,
                              Status = OrderStatus.Delivered,
                              ApplicationUserId = users.Skip(7).First().Id,
                              OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(5).First().Id, Quantity = 4 },
                new OrderProduct { ProductId = products.Skip(2).First().Id, Quantity = 3 }
            },
                              Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                              Payment = new Payment { TotalAmount = 3700.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                          },
                           new Order
                           {
                               TotalAmount = 899.99m,
                               OrderDate = DateTime.UtcNow,
                               Status = OrderStatus.Processing,
                               ApplicationUserId = users.Skip(8).First().Id,
                               OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(2).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(1).First().Id, Quantity = 1 },
                new OrderProduct { ProductId = products.Skip(3).First().Id, Quantity = 1 }
            },
                               Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                               Payment = new Payment { TotalAmount = 899.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                           },
                                    new Order
                                    {
                                        TotalAmount = 799.99m,
                                        OrderDate = DateTime.UtcNow,
                                        Status = OrderStatus.Processing,
                                        ApplicationUserId = users.Skip(9).First().Id,
                                        OrderProducts = new List<OrderProduct>
            {
                new OrderProduct { ProductId = products.Skip(2).First().Id, Quantity = 3 },
                new OrderProduct { ProductId = products.Skip(1).First().Id, Quantity = 1 },
                new OrderProduct { ProductId = products.Skip(4).First().Id, Quantity = 1 }
            },
                                        Shipping = new Shipping { EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7), Status = "Shipped" },
                                        Payment = new Payment { TotalAmount = 799.99m, TransactionDate = DateTime.UtcNow, Status = PaymentStatus.Completed, PaymentMethod = "PayPal" }
                                    });
            await context.SaveChangesAsync();
        }




        public static async Task SeedReviews(AppDbContext context)
        {
            if (context.Reviews.Count() > 1) return;

            var users = context.ApplicationUsers.ToList();
            var products = context.Products.ToList();

            var reviews = new List<Review>
    {
        new Review
        {
            Rating = 5,
            Comment = "Excellent product, highly recommended!",
            CreatedAt = DateTime.UtcNow,
            ApplicationUserId = users.First().Id,
            ProductId = products.First().Id
        },
        new Review
        {
            Rating = 4,
            Comment = "Good quality but a bit expensive.",
            CreatedAt = DateTime.UtcNow,
            ApplicationUserId = users.Skip(1).First().Id,
            ProductId = products.Skip(1).First().Id
        },

    };

            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();
        }


        public static async Task SeedShipping(AppDbContext context)
        {
            if (context.Shippings.Count() > 1) return;

            var orders = context.Orders.ToList();

            var shippings = new List<Shipping>
    {
        new Shipping
        {
            OrderId = orders.First().Id,
            EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5),
            Status = "Shipped"
        },
        new Shipping
        {
            OrderId = orders.Skip(1).First().Id,
            EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7),
            Status = "Pending"
        },

    };

            await context.Shippings.AddRangeAsync(shippings);
            await context.SaveChangesAsync();
        }


        public static async Task SeedBanners(AppDbContext context)
        {
            if (context.Banners.Any()) return;

            var banners = new List<Banners>
{
    new Banners
    {
        Description = "Limited Time Offer on Samsung Galaxy S23!",
        ImageUrl = "/images/banners/galaxy_offer.jpg",
        DiscountPercentage = 10.00m,
        ProductId = context.Products.First(p => p.NameEn == "Samsung Galaxy S23").Id,
        VendorId = context.Users.First(v => v.BusinessName == "Tarek Tech").Id
    },
    new Banners
    {
        Description = "Big Deal on Dell XPS 13!",
        ImageUrl = "/images/banners/dell_xps_deal.jpg",
        DiscountPercentage = 15.00m,
        ProductId = context.Products.First(p => p.NameEn == "Dell XPS 13").Id,
        VendorId = context.Users.First(v => v.BusinessName == "Hawkins Supplies").Id
    },
    new Banners
    {
        Description = "Leather Jacket Winter Sale!",
        ImageUrl = "/images/banners/leather_jacket.jpg",
        DiscountPercentage = 20.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Men's Leather Jacket")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Style Hub").Id
    },
    new Banners
    {
        Description = "Evening Dress Special Discount",
        ImageUrl = "/images/banners/evening_dress.jpg",
        DiscountPercentage = 25.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Women's Evening Dress")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Style Hub").Id
    },
    new Banners
    {
        Description = "Relax at Spa – Now 30% Off!",
        ImageUrl = "/images/banners/spa_relax.jpg",
        DiscountPercentage = 30.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Spa Relaxation Package")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Beauty Care").Id
    },
    new Banners
    {
        Description = "Tire Replacement Flash Deal!",
        ImageUrl = "/images/banners/18.jpg",
        DiscountPercentage = 18.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Car Tire Replacement")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "CarFix").Id
    },
 
    new Banners
    {
        Description = "Modern Sofa – Comfort & Style Sale!",
        ImageUrl = "/images/banners/22.jpg",
        DiscountPercentage = 22.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Modern Sofa")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Decor World").Id
    },
    new Banners
    {
        Description = "Playhouse Fun Now 15% Off!",
        ImageUrl = "/images/banners/15.jpg",
        DiscountPercentage = 15.00m,
        ProductId = context.Products.First(p => p.NameEn.Contains("Kids' Playhouse")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Kids Fun").Id
    },
    new Banners
    {
        Description = "Exclusive Laptop Discount!",
        ImageUrl = "/images/banners/17.jpg",
        DiscountPercentage = 17.00m,
        ProductId = context.Products.First(p => p.SubCategory.NameEn == "Laptops").Id,
        VendorId = context.Users.First(v => v.BusinessName == "Hawkins Supplies").Id
    },
    new Banners
    {
        Description = "Summer Wardrobe Clearance!",
        ImageUrl = "/images/banners/19.jpg",
            DiscountPercentage = 19.00m,
        ProductId = context.Products.First(p => p.SubCategory.NameEn == "Women's Clothing").Id,
        VendorId = context.Users.First(v => v.BusinessName == "Style Hub").Id
    },
    new Banners
    {
        Description = "Men's Casual Offers!",
        ImageUrl = "/images/banners/16.jpg",
        DiscountPercentage = 16.00m,
        ProductId = context.Products.First(p => p.SubCategory.NameEn == "Men's Clothing").Id,
        VendorId = context.Users.First(v => v.BusinessName == "Style Hub").Id
    },
    new Banners
    {
        Description = "Top Furniture Deals of the Month!",
        ImageUrl = "/images/banners/14.jpg",
        DiscountPercentage = 14.00m,
        ProductId = context.Products.First(p => p.SubCategory.NameEn.Contains("Furniture")).Id,
        VendorId = context.Users.First(v => v.BusinessName == "Decor World").Id
    }
};

            await context.Banners.AddRangeAsync(banners);
            await context.SaveChangesAsync();
        }
    }
}



