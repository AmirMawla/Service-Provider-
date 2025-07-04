using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto;
public class BannerRequestValidator : AbstractValidator<BannerRequest>
{
    public BannerRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0).WithMessage("Product ID must be greater than 0.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .Must(url => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out _))
            .WithMessage("Image URL is not a valid format.");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .WithMessage("Discount percentage must be between 0 and 100.");

        RuleFor(x => x.DiscountCode)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.DiscountCode))
            .WithMessage("Discount code must be less than 50 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description  must be less than 500 characters.");
    }
}
