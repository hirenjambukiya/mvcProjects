using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ex_RemoteValidation.CustomValidation
{
    public class NoSpecialCharactersAttribute : ValidationAttribute, IClientModelValidator
    {
        public override bool IsValid(object input)
        {
            var str = input as string;
            return !string.IsNullOrWhiteSpace(str) && str.All(char.IsLetterOrDigit);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val-nospecialcharacters", ErrorMessage ?? "No special characters allowed.");
        }
    }
}
