using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.PaymentDto
{
    public record TransactionResponse(
      int Id,
      decimal TotalAmount,
      DateTime TransactionDate,
      string Status,
      string PaymentMethod,
      int OrderId,
      string ApplicationUserFullName
    );
}
