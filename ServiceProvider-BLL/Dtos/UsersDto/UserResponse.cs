using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.UsersDto
{
    public record UserResponse(
        string Id, 
        string FullName,
        string Email,
         string? Address ,
        string? PhoneNumber,
        DateTime? RegistrationDate
    );
}
