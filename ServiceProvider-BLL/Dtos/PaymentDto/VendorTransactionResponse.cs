using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.PaymentDto
{
    public record VendorTransactionResponse(
      int OrderId,
      string UserFullName,
      string UserEmail,
      decimal Amount,
      string PaymentMethod,
      string Status,
      DateTime TransactionDate,
      List<VendorProductTransactionResponse> Products
    );

    public record VendorProductTransactionResponse(
      int ProductId,
      string NameEn,
      string NameAr,
      decimal Price,
      int Quantity
    );
}
