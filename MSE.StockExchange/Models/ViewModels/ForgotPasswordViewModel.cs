using System.ComponentModel.DataAnnotations;

namespace MSE.StockExchange.Models.ViewModels;

public class ForgotPasswordViewModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string Identifier { get; set; } = string.Empty;
}
