using System.ComponentModel.DataAnnotations;

namespace MSE.StockExchange.Models.ViewModels;

public class VerifyLoginOtpViewModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Otp { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = "/";
}
