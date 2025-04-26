using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CartProductDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Result<CartResponse>> GetCart(string userId, CancellationToken cancellationToken = default);
        Task<Result<CartProductResponse>> AddToCartAsync(string userId, CartProductRequest request , CancellationToken cancellationToken);
        Task<Result<CartProductResponse>> UpdateCartItemAsync(string userId,UpdateCartItemRequest request , CancellationToken cancellationToken);
    }
}
