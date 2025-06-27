using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto;
public record VendorProductsInOrderDto
(

        int ProductId,
        string ProductImageUrl,
        string NameEn,
        string NameAr,
        decimal Price,
        int Quantity,
        bool IsRated,
        int Rating

    );
