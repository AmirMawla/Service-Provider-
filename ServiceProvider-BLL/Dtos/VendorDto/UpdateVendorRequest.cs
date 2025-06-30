using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record UpdateVendorRequest
    (
         string? FullName ,
         string? BusinessName ,
         IFormFile? ProfilePictureUrl,
         IFormFile? CoverImageUrl
    );
}
