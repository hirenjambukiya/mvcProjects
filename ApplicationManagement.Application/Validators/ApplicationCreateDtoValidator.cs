using AMS.Application.DTOs;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMS.Application.Validators
{
    public class ApplicationCreateDtoValidator
     : AbstractValidator<ApplicationCreateDto>
    {
        public ApplicationCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Age)
                .InclusiveBetween(1, 100);

            RuleFor(x => x.Gender)
                .NotEmpty();

            RuleFor(x => x.Country)
                .NotEmpty();

            RuleFor(x => x.State)
                .NotEmpty();

            RuleFor(x => x.District)
                .NotEmpty();

            RuleFor(x => x.Pincode)
                .NotEmpty()
                .Length(6);

            RuleFor(x => x.Address)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.File)
                .Must(file =>
                {
                    if (file == null)
                        return true;

                    var allowedExtensions =
                        new[] { ".pdf", ".jpg", ".png" };

                    var extension =
                        Path.GetExtension(file.FileName)
                            .ToLower();

                    return allowedExtensions
                        .Contains(extension);
                })
                .WithMessage(
                    "Only PDF, JPG, PNG allowed");
        }
    }
}
