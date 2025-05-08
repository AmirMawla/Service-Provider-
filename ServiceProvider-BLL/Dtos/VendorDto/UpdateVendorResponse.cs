using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record UpdateVendorResponse
    (
         string UserName ,
         string BusinessName ,
         IFormFile? ProfilePictureUrl,
         IFormFile? CoverImageUrl


    //public string BusinessType { get; set; }


    //[DataType(DataType.Password)]
    //public string OldPassword { get; set; }

    //[DataType(DataType.Password)]
    //public string NewPassword { get; set; }

    //[Required]
    //[DataType(DataType.Password)]
    //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    //public string ConfirmPassword { get; set; }
    );
}
