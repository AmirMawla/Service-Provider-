using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto;
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.NameEn)
            .NotEmpty()
            .WithMessage("Please ensure you have entered {PropertyName}")
            .Length(1, 100);

        RuleFor(x => x.NameAr)
            .NotEmpty()
            .WithMessage("Please ensure you have entered {PropertyName}")
            .Length(1, 100);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("invalid {PropertyName}");

        RuleFor(x => x.SubCategoryId)
           .GreaterThan(0)
           .WithMessage("invalid {PropertyName}");
    }
}
