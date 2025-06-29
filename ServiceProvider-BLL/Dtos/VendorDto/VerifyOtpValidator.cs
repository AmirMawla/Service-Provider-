
using FluentValidation;
using SurvayBasket.Contracts.AccountProfile.cs;

namespace Government.Contracts.AccountProfile.cs
{
    public class VerifyOtpValidator : AbstractValidator<VerifyOtpDto>
    {
        public VerifyOtpValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Otp)
             .NotEmpty();
             
        }
    }
}
