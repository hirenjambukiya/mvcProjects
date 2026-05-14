using System.Threading.Tasks;
using ApiTester.Application.DTOs;
using ApiTester.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiTester.Web.Controllers
{
    public class ApiController : Controller
    {
        private readonly IApiTesterService _apiTesterService;

        public ApiController(IApiTesterService apiTesterService)
        {
            _apiTesterService = apiTesterService;
        }

        public IActionResult Index(int? id)
        {
            ViewBag.LoadHistoryId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Execute([FromBody] ApiRequestDto request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Url) || string.IsNullOrWhiteSpace(request.Method))
            {
                return BadRequest(new { IsSuccess = false, ErrorMessage = "Invalid request parameters." });
            }

            var response = await _apiTesterService.ExecuteRequestAsync(request);
            return Ok(response);
        }
    }
}
