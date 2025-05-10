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
                 new SubCategory { NameEn = "Decor", NameAr = "الديكور", ImageUrl = "/images/subcategories/Decorjpg.jpg", CategoryId = context.Categories.First(c => c.NameEn == "Furniture").Id },


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

                if (context.Products.Count() < 40)
            {
                var products = new List<Product> {
                     new Product
                     {

                         NameEn = "Samsung 55\" 4K UHD Smart TV",
                         NameAr = "تلفزيون سامسونج 55 بوصة 4K",
                         Description = "HDR, Smart apps, sleek design",
                         Price = 7999.99m,
                         MainImageUrl = "/images/products/tv1.jpg",
                         CreatedAt = DateTime.UtcNow,
                         UpdatedAt = DateTime.UtcNow,
                         VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
                         SubCategoryId = 20
                     },
    new Product
    {

        NameEn = "LG 65\" NanoCell Smart TV",
        NameAr = "تلفزيون إل جي 65 بوصة",
        Description = "NanoCell technology with AI ThinQ",
        Price = 12999.00m,
        MainImageUrl = "/images/products/tv2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 20
    },

    // Accessories (21)
    new Product
    {

        NameEn = "Anker Power Bank 20000mAh",
        NameAr = "باور بانك أنكر 20000 مللي أمبير",
        Description = "High-speed charging with PowerIQ",
        Price = 499.99m,
        MainImageUrl = "/images/products/accessory1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 21
    },
    new Product
    {

        NameEn = "Wireless Earbuds JBL",
        NameAr = "سماعات JBL لاسلكية",
        Description = "Bluetooth 5.0 with charging case",
        Price = 699.00m,
        MainImageUrl = "/images/products/accessory2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 21
    },

    // Air Conditioners (22)
    new Product
    {

        NameEn = "Carrier 1.5HP AC Cool & Heat",
        NameAr = "مكيف كاريير 1.5 حصان بارد وساخن",
        Description = "Inverter technology with remote control",
        Price = 8999.00m,
        MainImageUrl = "/images/products/ac1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 22
    },
    new Product
    {

        NameEn = "Sharp AC 2.25HP Inverter",
        NameAr = "شارب 2.25 حصان انفرتر",
        Description = "Energy-saving technology with fast cooling",
        Price = 10499.00m,
        MainImageUrl = "/images/products/ac2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 22
    },

    // Refrigerators (23)
    new Product
    {

        NameEn = "Samsung 600L Double Door Fridge",
        NameAr = "ثلاجة سامسونج 600 لتر",
        Description = "Digital inverter, frost-free cooling",
        Price = 11999.00m,
        MainImageUrl = "/images/products/fridge1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 23
    },
    new Product
    {

        NameEn = "LG Smart Fridge 500L",
        NameAr = "ثلاجة إل جي 500 لتر ذكية",
        Description = "Smart inverter compressor with multi-air flow",
        Price = 10999.00m,
        MainImageUrl = "/images/products/fridge2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 23
    },

    // Washing Machines (24)
    new Product
    {

        NameEn = "Bosch 8KG Front Load",
        NameAr = "غسالة بوش 8 كجم تحميل أمامي",
        Description = "EcoSilence Drive with AllergyPlus",
        Price = 8499.00m,
        MainImageUrl = "/images/products/wm1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 24
    },
    new Product
    {

        NameEn = "Toshiba 7KG Top Load",
        NameAr = "غسالة توشيبا 7 كجم تحميل علوي",
        Description = "Hydro Twin Power Wash with air dry",
        Price = 6299.00m,
        MainImageUrl = "/images/products/wm2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 24
    },

    // Microwave (25)
    new Product
    {

        NameEn = "Sharp 25L Grill Microwave",
        NameAr = "ميكروويف شارب 25 لتر",
        Description = "Grill + defrost functions, 900W power",
        Price = 1499.00m,
        MainImageUrl = "/images/products/microwave1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 25
    },
    new Product
    {

        NameEn = "Samsung 30L Smart Microwave",
        NameAr = "ميكروويف سامسونج 30 لتر ذكي",
        Description = "LED display with multiple programs",
        Price = 1799.00m,
        MainImageUrl = "/images/products/microwave2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 25
    },

    // Cameras (26)
    new Product
    {

        NameEn = "Canon EOS 200D II",
        NameAr = "كانون EOS 200D II",
        Description = "DSLR, 24.1MP, Wi-Fi enabled",
        Price = 10299.00m,
        MainImageUrl = "/images/products/camera1.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 26
    },
    new Product
    {

        NameEn = "Sony Alpha a6400 Mirrorless",
        NameAr = "سوني ألفا a6400",
        Description = "24.2MP, 4K video, fast autofocus",
        Price = 15999.00m,
        MainImageUrl = "/images/products/camera2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "80a331f5-2539-4c85-b38d-ced79755b135",
        SubCategoryId = 26
    },
    // Mobile Phones (17)
    new Product
    {

        NameEn = "Galaxy S23 Ultra",
        NameAr = "جالاكسي S23 ألترا",
        Description = "Samsung flagship phone with 200MP camera.",
        Price = 1199.99m,
        MainImageUrl = "/images/products/galaxy_s23_ultra.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 17
    },
    new Product
    {

        NameEn = "iPhone 15 Pro Max",
        NameAr = "آيفون 15 برو ماكس",
        Description = "Apple’s latest iPhone with titanium body and A17 chip.",
        Price = 1299.99m,
        MainImageUrl = "/images/products/iphone_15_pro_max.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 17
    },

    // Laptops (18)
    new Product
    {

        NameEn = "Dell XPS 15",
        NameAr = "ديل XPS 15",
        Description = "High-performance laptop with OLED display.",
        Price = 1599.99m,
        MainImageUrl = "/images/products/dell_xps_15.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 18
    },
    new Product
    {

        NameEn = "MacBook Pro 14",
        NameAr = "ماك بوك برو 14",
        Description = "Apple laptop with M2 Pro chip and Liquid Retina display.",
        Price = 1999.99m,
        MainImageUrl = "/images/products/macbook_pro_14.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 18
    },

    // Gaming Consoles (19)
    new Product
    {

        NameEn = "PlayStation 5",
        NameAr = "بلايستيشن 5",
        Description = "Next-gen Sony gaming console with ultra-fast SSD.",
        Price = 499.99m,
        MainImageUrl = "/images/products/ps5.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 19
    },
    new Product
    {

        NameEn = "Xbox Series X",
        NameAr = "إكس بوكس سيريس إكس",
        Description = "Microsoft’s most powerful console with 4K gaming.",
        Price = 499.99m,
        MainImageUrl = "/images/products/xbox_series_x.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 19
    },


    new Product
    {

        NameEn = "Logitech Wireless Mouse",
        NameAr = "فأرة لوجيتك لاسلكية",
        Description = "Compact and ergonomic mouse with long battery life.",
        Price = 29.99m,
        MainImageUrl = "/images/products/logitech_mouse.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "01930452-bd52-4f57-8c87-677c23d8690b",
        SubCategoryId = 21
    },
     new Product
     {
         NameEn = "Floral Summer Dress",
         NameAr = "فستان صيفي مزهر",
         Description = "Lightweight cotton dress perfect for summer.",
         Price = 59.99m,
         MainImageUrl = "/images/products/floral_summer_dress.jpg",
         CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow,
         VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
         SubCategoryId = 13
     },
    new Product
    {
        NameEn = "Casual Denim Jacket",
        NameAr = "جاكيت جينز كاجوال",
        Description = "Trendy denim jacket for everyday wear.",
        Price = 79.99m,
        MainImageUrl = "/images/products/denim_jacket.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 13
    },
    new Product
    {
        NameEn = "Long Sleeve Maxi Dress",
        NameAr = "فستان ماكسي بأكمام طويلة",
        Description = "Elegant maxi dress with modest styling.",
        Price = 89.99m,
        MainImageUrl = "/images/products/maxi_dress.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 13
    },

    // Sportswear (15)
    new Product
    {
        NameEn = "Running Leggings",
        NameAr = "بنطال رياضي للجري",
        Description = "Stretchable and breathable running leggings.",
        Price = 39.99m,
        MainImageUrl = "/images/products/running_leggings.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 15
    },
    new Product
    {
        NameEn = "Dry-Fit Sports T-shirt",
        NameAr = "تيشيرت رياضي دراي فيت",
        Description = "Moisture-wicking t-shirt ideal for workouts.",
        Price = 29.99m,
        MainImageUrl = "/images/products/dryfit_tshirt.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 15
    },
    new Product
    {
        NameEn = "Women's Training Shoes",
        NameAr = "حذاء تدريب نسائي",
        Description = "Lightweight shoes with responsive cushioning.",
        Price = 99.99m,
        MainImageUrl = "/images/products/womens_training_shoes.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 15
    },

    // Accessories (16)
    new Product
    {
        NameEn = "Leather Handbag",
        NameAr = "حقيبة يد جلدية",
        Description = "Elegant genuine leather handbag with gold accents.",
        Price = 149.99m,
        MainImageUrl = "/images/products/leather_handbag.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Fashion Sunglasses",
        NameAr = "نظارات شمسية موضة",
        Description = "UV-protected sunglasses with trendy frames.",
        Price = 24.99m,
        MainImageUrl = "/images/products/fashion_sunglasses.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Silk Scarf",
        NameAr = "وشاح حرير",
        Description = "Colorful silk scarf with artistic design.",
        Price = 34.99m,
        MainImageUrl = "/images/products/silk_scarf.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "fd69db09-3b72-4ca8-8a50-9d17684a27a1",
        SubCategoryId = 16
    },
     new Product
     {
         NameEn = "LED Light Bulb Pack",
         NameAr = "حزمة لمبات LED",
         Description = "Energy-saving LED bulbs with long lifespan.",
         Price = 15.99m,
         MainImageUrl = "/images/products/led_bulbs.jpg",
         CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow,
         VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
         SubCategoryId = 32
     },
    new Product
    {
        NameEn = "Extension Power Strip",
        NameAr = "مشترك كهرباء",
        Description = "6-outlet power strip with surge protection.",
        Price = 22.50m,
        MainImageUrl = "/images/products/power_strip.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
        SubCategoryId = 32
    },
    new Product
    {
        NameEn = "Wall Socket with USB",
        NameAr = "مقبس حائط مع USB",
        Description = "Wall socket with dual USB ports for charging devices.",
        Price = 18.00m,
        MainImageUrl = "/images/products/usb_socket.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
        SubCategoryId = 32
    },

    // Water Supply (33)
    new Product
    {
        NameEn = "Kitchen Faucet",
        NameAr = "حنفية مطبخ",
        Description = "Stainless steel faucet with adjustable spout.",
        Price = 75.00m,
        MainImageUrl = "/images/products/kitchen_faucet.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
        SubCategoryId = 33
    },
    new Product
    {
        NameEn = "Water Pressure Regulator",
        NameAr = "منظم ضغط الماء",
        Description = "Brass regulator to control home water pressure.",
        Price = 42.00m,
        MainImageUrl = "/images/products/water_regulator.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
        SubCategoryId = 33
    },
    new Product
    {
        NameEn = "Flexible Shower Hose",
        NameAr = "خرطوم دش مرن",
        Description = "Durable, anti-kink shower hose with chrome finish.",
        Price = 19.99m,
        MainImageUrl = "/images/products/shower_hose.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "51127ab7-0c80-40c5-b128-5825199afae4",
        SubCategoryId = 33
    },
    // Hair Salons (27)
    new Product
    {
        NameEn = "Haircut & Styling Package",
        NameAr = "حزمة قص وتصفيف الشعر",
        Description = "Complete haircut and styling service by professionals.",
        Price = 120.00m,
        MainImageUrl = "/images/products/haircut_styling.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 27
    },
    new Product
    {
        NameEn = "Hair Coloring Session",
        NameAr = "جلسة تلوين شعر",
        Description = "Full hair coloring with premium dye and aftercare.",
        Price = 250.00m,
        MainImageUrl = "/images/products/hair_coloring.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 27
    },
    new Product
    {
        NameEn = "Keratin Treatment",
        NameAr = "علاج الكيراتين",
        Description = "Smoothing keratin treatment for frizz-free hair.",
        Price = 300.00m,
        MainImageUrl = "/images/products/keratin.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 27
    },

    // Clinics (29)
    new Product
    {
        NameEn = "Dental Checkup",
        NameAr = "فحص الأسنان",
        Description = "Comprehensive dental checkup including cleaning.",
        Price = 200.00m,
        MainImageUrl = "/images/products/dental_checkup.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 29
    },
    new Product
    {
        NameEn = "Skin Consultation",
        NameAr = "استشارة بشرة",
        Description = "Professional skin analysis and skincare plan.",
        Price = 150.00m,
        MainImageUrl = "/images/products/skin_consult.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 29
    },
    new Product
    {
        NameEn = "Eye Exam",
        NameAr = "فحص العيون",
        Description = "Comprehensive eye examination by a certified ophthalmologist.",
        Price = 180.00m,
        MainImageUrl = "/images/products/eye_exam.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 29
    },

    // Spas (31)
    new Product
    {
        NameEn = "Full Body Massage",
        NameAr = "تدليك الجسم الكامل",
        Description = "Relaxing massage with aromatic oils.",
        Price = 220.00m,
        MainImageUrl = "/images/products/full_body_massage.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 31
    },
    new Product
    {
        NameEn = "Facial Treatment",
        NameAr = "علاج الوجه",
        Description = "Rejuvenating facial with deep cleansing and hydration.",
        Price = 180.00m,
        MainImageUrl = "/images/products/facial.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 31
    },
    new Product
    {
        NameEn = "Moroccan Bath",
        NameAr = "حمام مغربي",
        Description = "Traditional Moroccan bath with scrub and steam.",
        Price = 250.00m,
        MainImageUrl = "/images/products/moroccan_bath.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "4204e4f7-1848-4ad7-bbc5-877c9c6530c0",
        SubCategoryId = 31
    },
    // Accessories (16)
    new Product
    {
        NameEn = "Wireless Earbuds",
        NameAr = "سماعات أذن لاسلكية",
        Description = "High-quality wireless earbuds with noise cancellation.",
        Price = 150.00m,
        MainImageUrl = "/images/products/accessory2.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Smartwatch Strap",
        NameAr = "حزام ساعة ذكية",
        Description = "Comfortable and stylish smartwatch strap.",
        Price = 40.00m,
        MainImageUrl = "/images/products/smartwatch_strap.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Phone Stand Holder",
        NameAr = "حامل هاتف",
        Description = "Adjustable phone stand for desk use.",
        Price = 25.00m,
        MainImageUrl = "/images/products/phone_stand.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 16
    },

    // Mobile Phones (17)
    new Product
    {
        NameEn = "iPhone 14 Pro",
        NameAr = "آيفون 14 برو",
        Description = "Latest Apple iPhone with advanced camera system.",
        Price = 4500.00m,
        MainImageUrl = "/images/products/iphone_15_pro_max.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 17
    },
    new Product
    {
        NameEn = "Samsung Galaxy S23",
        NameAr = "سامسونج جالاكسي S23",
        Description = "Flagship Android phone with AMOLED display.",
        Price = 3800.00m,
        MainImageUrl = "/images/products/galaxy_s23_ultra.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 17
    },
    new Product
    {
        NameEn = "Xiaomi Redmi Note 12",
        NameAr = "شاومي ريدمي نوت 12",
        Description = "Affordable smartphone with long battery life.",
        Price = 1200.00m,
        MainImageUrl = "/images/products/redmi_note_12.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 17
    },

    // Laptops (18)
    new Product
    {
        NameEn = "Dell XPS 15",
        NameAr = "ديل XPS 15",
        Description = "Powerful laptop for professionals and creators.",
        Price = 6800.00m,
        MainImageUrl = "/images/products/dell_xps_15.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 18
    },
    new Product
    {
        NameEn = "MacBook Air M2",
        NameAr = "ماك بوك إير M2",
        Description = "Lightweight and fast laptop from Apple.",
        Price = 7200.00m,
        MainImageUrl = "/images/products/macbook_pro_14.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 18
    },
    new Product
    {
        NameEn = "Lenovo Legion 5",
        NameAr = "لينوفو ليجيون 5",
        Description = "Gaming laptop with NVIDIA graphics.",
        Price = 5900.00m,
        MainImageUrl = "/images/products/lenovo_legion_5.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 18
    },

    // Gaming Consoles (19)
    new Product
    {
        NameEn = "PlayStation 5",
        NameAr = "بلايستيشن 5",
        Description = "Next-gen Sony gaming console.",
        Price = 3500.00m,
        MainImageUrl = "/images/products/ps5.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 19
    },
    new Product
    {
        NameEn = "Xbox Series X",
        NameAr = "إكس بوكس سيريس إكس",
        Description = "Microsoft's most powerful gaming console.",
        Price = 3300.00m,
        MainImageUrl = "/images/products/xbox_series_x.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 19
    },
    new Product
    {
        NameEn = "Nintendo Switch OLED",
        NameAr = "نينتندو سويتش OLED",
        Description = "Hybrid console with upgraded OLED screen.",
        Price = 1800.00m,
        MainImageUrl = "/images/products/switch_oled.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ac11f82b-ae9c-4be8-9854-0fbf98f187b0",
        SubCategoryId = 19
    },
     new Product
     {
         NameEn = "Modern Wall Clock",
         NameAr = "ساعة حائط عصرية",
         Description = "Stylish modern wall clock suitable for any decor.",
         Price = 250.00m,
         MainImageUrl = "/images/products/wall_clock.jpg",
         CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow,
         VendorId = "2da02a02-2884-4c99-bf27-0d9d4f47dbd8",
         SubCategoryId = 52
     },
    new Product
    {
        NameEn = "Wooden Picture Frame Set",
        NameAr = "مجموعة إطارات صور خشبية",
        Description = "Set of 4 wooden picture frames in various sizes.",
        Price = 180.00m,
        MainImageUrl = "/images/products/picture_frames.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "2da02a02-2884-4c99-bf27-0d9d4f47dbd8",
        SubCategoryId = 52
    },
    new Product
    {
        NameEn = "Decorative Vase",
        NameAr = "مزهرية زخرفية",
        Description = "Elegant ceramic vase perfect for flowers or standalone decor.",
        Price = 320.00m,
        MainImageUrl = "/images/products/decorative_vase.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "2da02a02-2884-4c99-bf27-0d9d4f47dbd8",
        SubCategoryId = 52
    },
    new Product
    {
        NameEn = "LED Strip Lights",
        NameAr = "أشرطة إضاءة LED",
        Description = "Multi-color LED strip lights with remote control for ambiance.",
        Price = 95.00m,
        MainImageUrl = "/images/products/led_strip_lights.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "2da02a02-2884-4c99-bf27-0d9d4f47dbd8",
        SubCategoryId = 52
    },
    // Car Wash (53)
    new Product
    {
        NameEn = "Basic Car Wash",
        NameAr = "غسيل سيارة أساسي",
        Description = "Exterior and interior cleaning for your car.",
        Price = 50.00m,
        MainImageUrl = "/images/products/basic_car_wash.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 53
    },
    new Product
    {
        NameEn = "Premium Car Detailing",
        NameAr = "تنظيف شامل للسيارة",
        Description = "Includes waxing, polishing, and deep interior cleaning.",
        Price = 180.00m,
        MainImageUrl = "/images/products/premium_detailing.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 53
    },
    new Product
    {
        NameEn = "Engine Bay Cleaning",
        NameAr = "تنظيف محرك السيارة",
        Description = "Professional cleaning of the car engine bay.",
        Price = 70.00m,
        MainImageUrl = "/images/products/engine_cleaning.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 53
    },

    // Tire Shops (54)
    new Product
    {
        NameEn = "Michelin Tire 17\"",
        NameAr = "إطار ميشلان 17 بوصة",
        Description = "High-performance tire suitable for all seasons.",
        Price = 350.00m,
        MainImageUrl = "/images/products/michelin_17.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 54
    },
    new Product
    {
        NameEn = "Tire Balancing Service",
        NameAr = "خدمة موازنة الإطارات",
        Description = "Ensure smooth and safe driving with tire balancing.",
        Price = 40.00m,
        MainImageUrl = "/images/products/tire_balancing.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 54
    },
    new Product
    {
        NameEn = "Tire Replacement Labor",
        NameAr = "أجرة تركيب الإطارات",
        Description = "Labor cost for replacing all four tires.",
        Price = 100.00m,
        MainImageUrl = "/images/products/tire_replacement.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 54
    },

    // Auto Repair (55) Car Repair
    new Product
    {
        NameEn = "Oil Change Service",
        NameAr = "خدمة تغيير الزيت",
        Description = "Includes oil filter replacement and engine oil.",
        Price = 120.00m,
        MainImageUrl = "/images/products/oil_change.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 55
    },
    new Product
    {
        NameEn = "Brake Pad Replacement",
        NameAr = "استبدال فحمات الفرامل",
        Description = "Front or rear brake pad replacement service.",
        Price = 250.00m,
        MainImageUrl = "/images/products/brake_replacement.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 55
    },
    new Product
    {
        NameEn = "Battery Replacement",
        NameAr = "استبدال البطارية",
        Description = "Installation of a new 12V car battery with warranty.",
        Price = 400.00m,
        MainImageUrl = "/images/products/battery_replacement.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "26b1c29e-f569-4776-b06b-8bc4c919bd0e",
        SubCategoryId = 55
    },
     new Product
     {
         NameEn = "Hourly Gaming Session",
         NameAr = "جلسة ألعاب بالساعة",
         Description = "Access to the latest gaming consoles for one hour.",
         Price = 30.00m,
         MainImageUrl = "/images/products/hourly_gaming.jpg",
         CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow,
         VendorId = "f40e1fd0-89b6-4ad6-8cd2-dca87aa20afb",
         SubCategoryId = 46
     },
    new Product
    {
        NameEn = "VIP Room Package",
        NameAr = "باقة غرفة VIP",
        Description = "Private room with snacks and 3 hours of gaming.",
        Price = 120.00m,
        MainImageUrl = "/images/products/vip_room.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f40e1fd0-89b6-4ad6-8cd2-dca87aa20afb",
        SubCategoryId = 46
    },
    new Product
    {
        NameEn = "Weekend Tournament Entry",
        NameAr = "اشتراك في بطولة نهاية الأسبوع",
        Description = "Join our weekend gaming tournament with prizes.",
        Price = 50.00m,
        MainImageUrl = "/images/products/tournament_entry.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f40e1fd0-89b6-4ad6-8cd2-dca87aa20afb",
        SubCategoryId = 46
    },
    new Product
    {
        NameEn = "Monthly Membership",
        NameAr = "اشتراك شهري",
        Description = "Unlimited access to the gaming center for one month.",
        Price = 350.00m,
        MainImageUrl = "/images/products/monthly_membership.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f40e1fd0-89b6-4ad6-8cd2-dca87aa20afb",
        SubCategoryId = 46
    },
    // Sportswear (15)
    new Product
    {
        NameEn = "Men's Running Shorts",
        NameAr = "شورت جري للرجال",
        Description = "Lightweight and breathable shorts for running.",
        Price = 85.00m,
        MainImageUrl = "/images/products/running_shorts.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 15
    },
    new Product
    {
        NameEn = "Women's Sports Leggings",
        NameAr = "ليجن رياضي للنساء",
        Description = "Flexible and comfortable leggings ideal for workouts.",
        Price = 120.00m,
        MainImageUrl = "/images/products/sports_leggings.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 15
    },
    new Product
    {
        NameEn = "Moisture-Wicking T-Shirt",
        NameAr = "تيشيرت طارد للرطوبة",
        Description = "Keeps you dry and cool during intense workouts.",
        Price = 95.00m,
        MainImageUrl = "/images/products/sports_tshirt.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 15
    },

    // Accessories (16)
    new Product
    {
        NameEn = "Fitness Tracker Watch",
        NameAr = "ساعة تتبع اللياقة",
        Description = "Monitor your heart rate and steps throughout the day.",
        Price = 250.00m,
        MainImageUrl = "/images/products/fitness_tracker.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Sweatband Set",
        NameAr = "طقم رباط عرق",
        Description = "Includes wristbands and a headband for workouts.",
        Price = 45.00m,
        MainImageUrl = "/images/products/sweatband_set.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "Water Bottle",
        NameAr = "زجاجة مياه رياضية",
        Description = "Durable and BPA-free bottle for hydration on the go.",
        Price = 60.00m,
        MainImageUrl = "/images/products/water_bottle.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "1a7d6b32-c634-4cb4-a9a7-e0b1d67b2086",
        SubCategoryId = 16
    },
    new Product
    {
        NameEn = "High-Pressure Water Pump",
        NameAr = "مضخة مياه عالية الضغط",
        Description = "Ideal for domestic and industrial water supply systems.",
        Price = 850.00m,
        MainImageUrl = "/images/products/water_pump.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "6e5668c1-a838-4640-bbe7-d6785fc2b2d8",
        SubCategoryId = 33
    },
    new Product
    {
        NameEn = "Water Filtration System",
        NameAr = "نظام تنقية المياه",
        Description = "Removes impurities and ensures clean water for all uses.",
        Price = 1200.00m,
        MainImageUrl = "/images/products/water_filter.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "6e5668c1-a838-4640-bbe7-d6785fc2b2d8",
        SubCategoryId = 33
    },
    new Product
    {
        NameEn = "Automatic Water Level Controller",
        NameAr = "جهاز تحكم أوتوماتيكي بمستوى المياه",
        Description = "Maintains optimal water levels in storage tanks.",
        Price = 310.00m,
        MainImageUrl = "/images/products/water_regulator.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "6e5668c1-a838-4640-bbe7-d6785fc2b2d8",
        SubCategoryId = 33
    },
    new Product
    {
        NameEn = "Submersible Water Pump",
        NameAr = "مضخة مياه غاطسة",
        Description = "Durable and efficient for deep wells and drainage.",
        Price = 980.00m,
        MainImageUrl = "/images/products/submersible_pump.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "6e5668c1-a838-4640-bbe7-d6785fc2b2d8",
        SubCategoryId = 33
    },
    // Schools (38)
    new Product
    {
        NameEn = "Smart Classroom System",
        NameAr = "نظام الفصول الذكية",
        Description = "Comprehensive system for digital learning in schools.",
        Price = 11000,
        MainImageUrl = "/images/products/classroom-system.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 38
    },
    new Product
    {
        NameEn = "Student Attendance App",
        NameAr = "تطبيق حضور الطلاب",
        Description = "Mobile and web-based student attendance tracker.",
        Price = 3500,
        MainImageUrl = "/images/products/attendance-app.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 38
    },
    new Product
    {
        NameEn = "Digital Library Access",
        NameAr = "الوصول إلى المكتبة الرقمية",
        Description = "Access to a digital library with thousands of educational resources.",
        Price = 4000,
        MainImageUrl = "/images/products/digital-library.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 38
    },

    // Universities (39)
    new Product
    {
        NameEn = "Online Learning Platform",
        NameAr = "منصة التعليم الإلكتروني",
        Description = "Full-featured LMS for universities.",
        Price = 18000,
        MainImageUrl = "/images/products/online-platform.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 39
    },
    new Product
    {
        NameEn = "University ERP System",
        NameAr = "نظام ERP للجامعات",
        Description = "Comprehensive university management software.",
        Price = 25000,
        MainImageUrl = "/images/products/university-erp.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 39
    },
    new Product
    {
        NameEn = "Research Management Tools",
        NameAr = "أدوات إدارة الأبحاث",
        Description = "Tools to streamline research project tracking and reporting.",
        Price = 9500,
        MainImageUrl = "/images/products/research-tools.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "f250538d-0d5a-4763-8271-96299bdefbc3",
        SubCategoryId = 39
    },
    // Hair Salons (27)
    new Product
    {
        NameEn = "Haircut & Styling Package",
        NameAr = "حزمة قص وتصفيف الشعر",
        Description = "Complete haircut and styling service by professionals.",
        Price = 120.00m,
        MainImageUrl = "/images/products/haircut_styling.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 27
    },
    new Product
    {
        NameEn = "Hair Coloring Session",
        NameAr = "جلسة تلوين شعر",
        Description = "Full hair coloring with premium dye and aftercare.",
        Price = 250.00m,
        MainImageUrl = "/images/products/hair_coloring.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 27
    },
    new Product
    {
        NameEn = "Keratin Treatment",
        NameAr = "علاج الكيراتين",
        Description = "Smoothing keratin treatment for frizz-free hair.",
        Price = 300.00m,
        MainImageUrl = "/images/products/keratin.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 27
    },

    // Clinics (29)
    new Product
    {
        NameEn = "Dental Checkup",
        NameAr = "فحص الأسنان",
        Description = "Comprehensive dental checkup including cleaning.",
        Price = 200.00m,
        MainImageUrl = "/images/products/dental_checkup.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 29
    },
    new Product
    {
        NameEn = "Skin Consultation",
        NameAr = "استشارة بشرة",
        Description = "Professional skin analysis and skincare plan.",
        Price = 150.00m,
        MainImageUrl = "/images/products/skin_consult.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 29
    },
    new Product
    {
        NameEn = "Eye Exam",
        NameAr = "فحص العيون",
        Description = "Comprehensive eye examination by a certified ophthalmologist.",
        Price = 180.00m,
        MainImageUrl = "/images/products/eye_exam.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 29
    },

    // Spas (31)
    new Product
    {
        NameEn = "Full Body Massage",
        NameAr = "تدليك الجسم الكامل",
        Description = "Relaxing massage with aromatic oils.",
        Price = 220.00m,
        MainImageUrl = "/images/products/full_body_massage.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 31
    },
    new Product
    {
        NameEn = "Facial Treatment",
        NameAr = "علاج الوجه",
        Description = "Rejuvenating facial with deep cleansing and hydration.",
        Price = 180.00m,
        MainImageUrl = "/images/products/facial.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 31
    },
    new Product
    {
        NameEn = "Moroccan Bath",
        NameAr = "حمام مغربي",
        Description = "Traditional Moroccan bath with scrub and steam.",
        Price = 250.00m,
        MainImageUrl = "/images/products/moroccan_bath.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "18d986cc-4892-4871-8095-21f85df87433",
        SubCategoryId = 31
    },
    new Product
    {
        NameEn = "Residential Waste Pickup",
        NameAr = "جمع النفايات السكنية",
        Description = "Scheduled waste collection service for residential areas.",
        Price = 200,
        MainImageUrl = "/images/products/residential-waste.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "d98c458c-4dec-4b40-9d0b-ca5effd508cd",
        SubCategoryId = 34
    },
    new Product
    {
        NameEn = "Commercial Waste Management",
        NameAr = "إدارة النفايات التجارية",
        Description = "Custom waste solutions for businesses and commercial zones.",
        Price = 750,
        MainImageUrl = "/images/products/commercial-waste.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "d98c458c-4dec-4b40-9d0b-ca5effd508cd",
        SubCategoryId = 34
    },
    new Product
    {
        NameEn = "Recyclable Material Collection",
        NameAr = "جمع المواد القابلة لإعادة التدوير",
        Description = "Pickup service for sorted recyclable materials.",
        Price = 150,
        MainImageUrl = "/images/products/recyclable-pickup.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "d98c458c-4dec-4b40-9d0b-ca5effd508cd",
        SubCategoryId = 34
    },
    new Product
    {
        NameEn = "Industrial Waste Disposal",
        NameAr = "التخلص من النفايات الصناعية",
        Description = "Safe and compliant disposal of hazardous and industrial waste.",
        Price = 1200,
        MainImageUrl = "/images/products/industrial-waste.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "d98c458c-4dec-4b40-9d0b-ca5effd508cd",
        SubCategoryId = 34
    },
     new Product
     {
         NameEn = "Wedding Stage Decoration",
         NameAr = "ديكور منصة الزفاف",
         Description = "Elegant stage decoration with floral and lighting elements for weddings.",
         Price = 3000,
         MainImageUrl = "/images/products/wedding-stage.jpg",
         CreatedAt = DateTime.UtcNow,
         UpdatedAt = DateTime.UtcNow,
         VendorId = "ff4522dc-351a-4667-989f-1df6ae658e1f",
         SubCategoryId = 52
     },
    new Product
    {
        NameEn = "Birthday Party Decor",
        NameAr = "ديكور حفلة عيد الميلاد",
        Description = "Custom themed decorations for birthday parties of all ages.",
        Price = 1000,
        MainImageUrl = "/images/products/birthday-decor.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ff4522dc-351a-4667-989f-1df6ae658e1f",
        SubCategoryId = 52
    },
    new Product
    {
        NameEn = "Corporate Event Setup",
        NameAr = "ترتيب فعاليات الشركات",
        Description = "Professional decor services for conferences, launches, and meetings.",
        Price = 2500,
        MainImageUrl = "/images/products/corporate-decor.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ff4522dc-351a-4667-989f-1df6ae658e1f",
        SubCategoryId = 52
    },
    new Product
    {
        NameEn = "Baby Shower Decoration",
        NameAr = "ديكور استقبال المولود",
        Description = "Delightful decorations tailored for baby showers with balloons and banners.",
        Price = 1200,
        MainImageUrl = "/images/products/baby-shower.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "ff4522dc-351a-4667-989f-1df6ae658e1f",
        SubCategoryId = 52
    },
    // Fast Food (1)
    new Product
    {
        NameEn = "Cheeseburger",
        NameAr = "تشيز برجر",
        Description = "Grilled beef patty with cheese, lettuce, tomato, and sauce.",
        Price = 35,
        MainImageUrl = "/images/products/cheeseburger.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 1
    },
    new Product
    {
        NameEn = "Fried Chicken Bucket",
        NameAr = "دلو دجاج مقلي",
        Description = "Crispy fried chicken pieces served with sauces.",
        Price = 70,
        MainImageUrl = "/images/products/fried-chicken.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 1
    },
    new Product
    {
        NameEn = "Beef Shawarma",
        NameAr = "شاورما لحم",
        Description = "Classic Middle Eastern beef shawarma wrap.",
        Price = 30,
        MainImageUrl = "/images/products/shawarma.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 1
    },

    // Snacks (4)
    new Product
    {
        NameEn = "Nachos with Cheese",
        NameAr = "ناتشوز بالجبن",
        Description = "Crispy nachos topped with melted cheese and jalapenos.",
        Price = 25,
        MainImageUrl = "/images/products/nachos.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 4
    },
    new Product
    {
        NameEn = "French Fries",
        NameAr = "بطاطس مقلية",
        Description = "Golden fries served with ketchup and mayo.",
        Price = 15,
        MainImageUrl = "/images/products/fries.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 4
    },
    new Product
    {
        NameEn = "Onion Rings",
        NameAr = "حلقات بصل",
        Description = "Crispy battered onion rings fried to perfection.",
        Price = 20,
        MainImageUrl = "/images/products/onion-rings.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 4
    },

    // Organic Food (5)
    new Product
    {
        NameEn = "Organic Quinoa Salad",
        NameAr = "سلطة الكينوا العضوية",
        Description = "Healthy salad with quinoa, vegetables, and vinaigrette.",
        Price = 40,
        MainImageUrl = "/images/products/quinoa-salad.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 5
    },
    new Product
    {
        NameEn = "Organic Chicken Breast",
        NameAr = "صدر دجاج عضوي",
        Description = "Grilled organic chicken served with vegetables.",
        Price = 65,
        MainImageUrl = "/images/products/organic-chicken.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 5
    },
    new Product
    {
        NameEn = "Organic Lentil Soup",
        NameAr = "شوربة العدس العضوية",
        Description = "Warm and hearty lentil soup made from organic ingredients.",
        Price = 30,
        MainImageUrl = "/images/products/lentil-soup.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 5
    },

    // Juices (7)
    new Product
    {
        NameEn = "Fresh Orange Juice",
        NameAr = "عصير برتقال طازج",
        Description = "Cold-pressed orange juice with no added sugar.",
        Price = 18,
        MainImageUrl = "/images/products/orange-juice.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 7
    },
    new Product
    {
        NameEn = "Carrot and Apple Juice",
        NameAr = "عصير جزر وتفاح",
        Description = "Refreshing blend of carrot and apple juices.",
        Price = 22,
        MainImageUrl = "/images/products/carrot-apple.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 7
    },
    new Product
    {
        NameEn = "Mixed Berry Juice",
        NameAr = "عصير التوت المشكل",
        Description = "A rich juice mix of strawberries, blueberries, and raspberries.",
        Price = 25,
        MainImageUrl = "/images/products/berry-juice.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 7
    },

    // Soft Drinks (8)
    new Product
    {
        NameEn = "Cola",
        NameAr = "كولا",
        Description = "Chilled classic cola beverage.",
        Price = 10,
        MainImageUrl = "/images/products/cola.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 8
    },
    new Product
    {
        NameEn = "Lemon Soda",
        NameAr = "صودا الليمون",
        Description = "Sparkling lemon soda for a refreshing taste.",
        Price = 12,
        MainImageUrl = "/images/products/lemon-soda.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 8
    },


    // Tea (9)
    new Product
    {
        NameEn = "Green Tea",
        NameAr = "شاي أخضر",
        Description = "Hot brewed green tea with antioxidants.",
        Price = 15,
        MainImageUrl = "/images/products/green-tea.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 9
    },
    new Product
    {
        NameEn = "Chai Latte",
        NameAr = "تشاي لاتيه",
        Description = "Spiced milk tea served hot or iced.",
        Price = 18,
        MainImageUrl = "/images/products/chai-latte.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 9
    },
    new Product
    {
        NameEn = "Mint Tea",
        NameAr = "شاي بالنعناع",
        Description = "Traditional mint tea served hot.",
        Price = 12,
        MainImageUrl = "/images/products/mint-tea.jpg",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        VendorId = "nasserelbrens",
        SubCategoryId = 9
    }
 };
                await context.Products.AddRangeAsync(products);
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



