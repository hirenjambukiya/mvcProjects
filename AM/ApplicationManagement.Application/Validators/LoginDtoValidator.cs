using AMS.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Validators
{
    public class LoginDtoValidator:AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                   .NotEmpty()
                   .EmailAddress()
                   .WithMessage("Valid email required");
            RuleFor(x => x.Password)
                   .NotEmpty()
                   .MinimumLength(6)
                   .WithMessage("Password must be minimum 6 characters");
        }
    }
}
