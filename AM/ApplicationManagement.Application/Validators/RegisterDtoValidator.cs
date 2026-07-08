using AMS.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100);
            RuleFor(x => x.Email)
                   .NotEmpty()
                   .EmailAddress()
                   .WithMessage("Valid email required");

            RuleFor(x => x.Password)
                  .NotEmpty()
                  .MinimumLength(6)
                  .WithMessage("Password must be minimum 6 characters");

            RuleFor(x => x.ConfirmPassword)
                  .Equal(x => x.Password)
                  .WithMessage("Passwords do not match");
        }
    }
}
