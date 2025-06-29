using FluentValidation;
using Government.Contracts.AccountProfile.cs;


namespace SurvayBasket.Contracts.AccountProfile.cs
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordRequestValidator()
        {


            RuleFor(x => x.NewPassword)
             .NotEmpty()
             .Matches("(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}")
             .MinimumLength(8);


            RuleFor(x => x.Email)
              .NotEmpty()
              .EmailAddress();

            RuleFor(x => x.ResetToken)
              .NotEmpty();


        }
    }
}