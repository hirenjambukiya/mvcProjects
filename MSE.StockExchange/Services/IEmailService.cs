using System.Threading.Tasks;

namespace MSE.StockExchange.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlMessage);
}
