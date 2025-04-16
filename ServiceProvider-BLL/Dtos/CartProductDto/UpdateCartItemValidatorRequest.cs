using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.CartProductDto
{
    public class UpdateCartItemValidatorRequest : AbstractValidator<UpdateCartItemRequest>
    {
        public UpdateCartItemValidatorRequest()
        {
           // RuleFor(x => x.CartId).GreaterThan(0).WithMessage("Invalid Cart ID");
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid Product ID");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1");
        }
    }
}
