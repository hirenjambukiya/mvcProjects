using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ex_EMSWithAJAX.Validation
{
    public class MinimumAgeAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly int _minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes,"data-val","true");

            MergeAttribute(context.Attributes,"data-val-minimumage",ErrorMessage ??$"Employee must be at least {_minimumAge} years old.");

            MergeAttribute(context.Attributes,"data-val-minimumage-age",_minimumAge.ToString());
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }
            if (value is not DateTime dateOfBirth)
            {
                return new ValidationResult(
                    "Invalid date of birth.");
            }

            var todae = DateTime.Today;
            var age = todae.Year - dateOfBirth.Year;

            if (dateOfBirth.Date > todae.AddYears(-age))
            {
                age--;
            }
            if (age < _minimumAge)
            {
                return new ValidationResult(
                    $"Employee must be at least {_minimumAge} years old.");
            }
            return ValidationResult.Success;

        }
        private static bool MergeAttribute(IDictionary<string, string> attributes,string key,string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);

            return true;
        }
    }
}
