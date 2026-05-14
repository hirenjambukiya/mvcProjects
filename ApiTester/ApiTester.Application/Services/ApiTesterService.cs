using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using ApiTester.Application.DTOs;
using ApiTester.Domain.Entities;
using ApiTester.Domain.Repositories;
using System.Linq;

namespace ApiTester.Application.Services
{
    public class ApiTesterService : IApiTesterService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IApiRequestHistoryRepository _historyRepository;

        public ApiTesterService(IHttpClientFactory httpClientFactory, IApiRequestHistoryRepository historyRepository)
        {
            _httpClientFactory = httpClientFactory;
            _historyRepository = historyRepository;
        }

        public async Task<ApiResponseDto> ExecuteRequestAsync(ApiRequestDto request)
        {
            var responseDto = new ApiResponseDto();
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

                // Add Headers
                if (request.Headers != null && request.Headers.Any())
                {
                    foreach (var header in request.Headers)
                    {
                        if (!string.IsNullOrWhiteSpace(header.Key))
                        {
                            httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                        }
                    }
                }

                // Add Body for methods that support it
                if (!string.IsNullOrWhiteSpace(request.Body) && 
                    (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
                {
                    httpRequest.Content = new StringContent(request.Body, Encoding.UTF8, "application/json");
                }

                var httpResponse = await client.SendAsync(httpRequest);
                
                stopwatch.Stop();
                
                responseDto.IsSuccess = httpResponse.IsSuccessStatusCode;
                responseDto.StatusCode = (int)httpResponse.StatusCode;
                responseDto.ResponseTime = stopwatch.ElapsedMilliseconds;
                
                if (httpResponse.Content != null)
                {
                    responseDto.ResponseBody = await httpResponse.Content.ReadAsStringAsync();
                    
                    // Format JSON if possible
                    try
                    {
                        var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseDto.ResponseBody);
                        responseDto.ResponseBody = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions { WriteIndented = true });
                    }
                    catch { /* Keep original string if not valid JSON */ }
                }

                foreach (var header in httpResponse.Headers)
                {
                    responseDto.ResponseHeaders.Add(new KeyValuePair<string, string>(header.Key, string.Join(", ", header.Value)));
                }

            }
            catch (TaskCanceledException)
            {
                stopwatch.Stop();
                responseDto.IsSuccess = false;
                responseDto.StatusCode = 408; // Request Timeout
                responseDto.ResponseTime = stopwatch.ElapsedMilliseconds;
                responseDto.ErrorMessage = "Request timed out after 30 seconds.";
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                responseDto.IsSuccess = false;
                responseDto.StatusCode = 500;
                responseDto.ResponseTime = stopwatch.ElapsedMilliseconds;
                responseDto.ErrorMessage = ex.Message;
            }

            // Save to Database
            var history = new ApiRequestHistory
            {
                ApiUrl = request.Url,
                HttpMethod = request.Method,
                RequestHeaders = request.Headers != null && request.Headers.Any() ? JsonSerializer.Serialize(request.Headers) : null,
                RequestJson = request.Body,
                ResponseJson = responseDto.ResponseBody,
                StatusCode = responseDto.StatusCode,
                ResponseTime = responseDto.ResponseTime,
                IsSuccess = responseDto.IsSuccess,
                ErrorMessage = responseDto.ErrorMessage,
                CreatedDate = DateTime.Now
            };

            try
            {
                await _historyRepository.AddAsync(history);
            }
            catch (Exception)
            {
                // In a real application, we would log this to a file logger if DB fails
                // But we don't want to fail the API test if just logging fails
            }

            return responseDto;
        }
    }
}
