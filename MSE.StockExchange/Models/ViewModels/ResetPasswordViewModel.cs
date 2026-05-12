using System.ComponentModel.DataAnnotations;

namespace MSE.StockExchange.Models.ViewModels;

public class ResetPasswordViewModel
{
    [Required]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    public string Otp { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "New Password (Base64 Encrypted)")]
    public string NewClientEncryptedPassword { get; set; } = string.Empty;

    // This is optional if client handles confirmation, but good for model state
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewClientEncryptedPassword", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmClientEncryptedPassword { get; set; } = string.Empty;
}
