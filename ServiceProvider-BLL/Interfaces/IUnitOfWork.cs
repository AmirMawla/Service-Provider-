using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IApplicationUserRepository ApplicationUsers { get; }
        IVendorRepository Vendors { get; }
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        ICartProductRepository CartProducts { get; }
        ICategoryRepository Categories { get; }
        ISubCategoryRepository SubCategories { get; }
        IOrderRepository Orders { get; }
        IOrderProductRepository OrderProducts { get; }
        IPaymentRepository Payments { get; }
        IShippingRepository Shippings { get; }
        IMessageRepository Messages { get; }
        IReviewRepository Reviews { get; }
        IVendorSubCategoryRepository VendorSubCategories { get; }
        IBannersRepository Banners { get; }
        Task<int> Complete();

    }
}
