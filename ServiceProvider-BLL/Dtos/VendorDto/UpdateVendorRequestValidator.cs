using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto;
public class UpdateVendorRequestValidator : AbstractValidator<UpdateVendorRequest>
{
    public UpdateVendorRequestValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName));

        RuleFor(x => x.BusinessName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.BusinessName));

        RuleFor(x => x.ProfilePictureUrl)
            .Must(BeValidImage!)
            .When(x => x.ProfilePictureUrl != null)
            .WithMessage("Invalid profile image format. Only .jpg, .jpeg, .png are allowed.");

        RuleFor(x => x.CoverImageUrl)
            .Must(BeValidImage!)
            .When(x => x.CoverImageUrl != null)
            .WithMessage("Invalid cover image format. Only .jpg, .jpeg, .png are allowed.");
    }

    private bool BeValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(extension);
    }
}
