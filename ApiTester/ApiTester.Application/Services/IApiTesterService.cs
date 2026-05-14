using System.Threading.Tasks;
using ApiTester.Application.DTOs;

namespace ApiTester.Application.Services
{
    public interface IApiTesterService
    {
        Task<ApiResponseDto> ExecuteRequestAsync(ApiRequestDto request);
    }
}
