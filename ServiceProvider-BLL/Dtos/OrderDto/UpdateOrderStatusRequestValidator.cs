using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto
{
    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.NewStatus)
                .NotEmpty()
                .Must(BeValidStatus).WithMessage("Invalid order status");

             bool BeValidStatus(string status) =>
                new[] { "Preparing", "OutForDelivery", "Delivered",}.Contains(status);
        }
    }
   
}
