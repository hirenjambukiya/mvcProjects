using System;
using Microsoft.Extensions.Caching.Memory;

namespace MSE.StockExchange.Services;

public class OtpService : IOtpService
{
    private readonly IMemoryCache _memoryCache;
    private static readonly Random _random = new Random();

    public OtpService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public string GenerateOtp(string purpose, string identifier)
    {
        // Generate a 6-digit number
        int number = _random.Next(100000, 999999);
        string code = number.ToString();

        // Create a unique key
        string cacheKey = $"OTP_{purpose}_{identifier}";

        // Set cache options - 5 minutes absolute expiration
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        // Save data in cache
        _memoryCache.Set(cacheKey, code, cacheEntryOptions);

        return code;
    }

    public bool ValidateOtp(string purpose, string identifier, string code)
    {
        string cacheKey = $"OTP_{purpose}_{identifier}";

        if (_memoryCache.TryGetValue(cacheKey, out string? cachedCode))
        {
            if (cachedCode == code)
            {
                // OTP is valid, remove it so it cannot be used again
                _memoryCache.Remove(cacheKey);
                return true;
            }
        }

        return false;
    }
}
