using System.ComponentModel.DataAnnotations;

namespace MSE.StockExchange.Models.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    // This property will hold the client-side encrypted payload.
    // The actual Password field above will be cleared or ignored on the server.
    public string ClientEncryptedPassword { get; set; } = string.Empty;
}
