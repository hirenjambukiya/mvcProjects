using System.Threading.Tasks;
using ApiTester.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiTester.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IApiRequestHistoryRepository _historyRepository;

        public HomeController(IApiRequestHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<IActionResult> Index()
        {
            var totalCalls = await _historyRepository.GetTotalCallsAsync();
            var successCalls = await _historyRepository.GetSuccessCountAsync();
            var failedCalls = await _historyRepository.GetFailedCountAsync();
            var avgResponseTime = await _historyRepository.GetAverageResponseTimeAsync();
            var recentCalls = await _historyRepository.GetRecentCallsAsync(10);

            ViewBag.TotalCalls = totalCalls;
            ViewBag.SuccessCalls = successCalls;
            ViewBag.FailedCalls = failedCalls;
            ViewBag.AvgResponseTime = avgResponseTime;

            return View(recentCalls);
        }
    }
}
