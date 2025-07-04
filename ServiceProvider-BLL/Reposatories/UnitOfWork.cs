using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Vendor> _usermanager;
        private readonly IPasswordHasher<Vendor> _passwordHasher;
        private readonly IWebHostEnvironment _env;
        private readonly IPublishEndpoint _publishEndpoint;
        public IApplicationUserRepository ApplicationUsers { get; private set; }
        public IVendorRepository Vendors { get; private set; }
        public IVendorSubCategoryRepository VendorSubCategories { get; private set; }
        public ISubCategoryRepository SubCategories { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IProductRepository Products { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IOrderProductRepository OrderProducts { get; private set; }
        public ICartRepository Carts { get; private set; }
        public ICartProductRepository CartProducts { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IShippingRepository Shippings { get; private set; }
        public IPaymentRepository Payments { get; private set; }
        public IMessageRepository Messages { get; private set; }
        public IBannersRepository Banners { get; private set; }

        public ISearchRepository Search { get; private set; }


        public UnitOfWork(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<Vendor> userManager, IPasswordHasher<Vendor> passwordHasher, IWebHostEnvironment env, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _usermanager = userManager;
            _passwordHasher = passwordHasher;
            _env = env;
            _publishEndpoint = publishEndpoint;
            ApplicationUsers = new ApplicationUserRepository(_context);
            Vendors = new VendorRepository(_context, _usermanager, _passwordHasher, _httpContextAccessor, _env,_publishEndpoint);
            Products = new ProductRepository(_context, _env);
            VendorSubCategories = new VendorSubCategoryRepository(_context);
            SubCategories = new SubCategoryRepository(_context);
            Reviews = new ReviewRepository(_context);
            Orders = new OrderRepository(_context,_publishEndpoint);
            OrderProducts = new OrderProductRepository(_context);
            Carts = new CartRepository(_context, _httpContextAccessor);
            CartProducts = new CartProductRepository(_context);
            Categories = new CategoryRepository(_context);
            Shippings = new ShippingRepository(_context);
            Payments = new PaymentRepository(_context);
            Messages = new MessageRepository(_context);
            Banners = new BannersRepository(_context , _env);
            Search = new SearchRepository(_context);
            _env = env;
        }

        public async Task<int> Complete() =>
            await _context.SaveChangesAsync();

        public void Dispose() =>
            _context.Dispose();
    }
}
