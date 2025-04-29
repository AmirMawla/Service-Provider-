using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.CartProductDto
{
    public class CartProductRequestValidator : AbstractValidator<CartProductRequest>
    {
        public CartProductRequestValidator() 
        {
            //RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required");

            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid Product ID");

            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1");

        }
    }
}
