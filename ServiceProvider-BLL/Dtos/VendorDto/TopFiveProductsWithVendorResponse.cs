using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record TopFiveProductsWithVendorResponse(
      int Id,
      string NameEn,
      string NameAr,
      int Sold,
      decimal Revenue    
    );
}
