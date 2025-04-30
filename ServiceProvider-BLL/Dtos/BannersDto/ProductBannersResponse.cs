using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto
{
    public record ProductBannersResponse
    (
      int Id ,
     string NameEn ,
     string NameAr ,
     string? Description ,
     decimal Price ,
     string? MainImageUrl
        
        );
    
}
