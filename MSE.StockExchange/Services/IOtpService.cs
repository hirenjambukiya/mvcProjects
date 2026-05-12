using System.Threading.Tasks;

namespace MSE.StockExchange.Services;

public interface IOtpService
{
    string GenerateOtp(string purpose, string identifier);
    bool ValidateOtp(string purpose, string identifier, string code);
}
