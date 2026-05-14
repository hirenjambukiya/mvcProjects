using System.Threading.Tasks;
using ApiTester.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ApiTester.Web.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IApiRequestHistoryRepository _historyRepository;

        public HistoryController(IApiRequestHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoryData()
        {
            var data = await _historyRepository.GetAllAsync();
            return Json(new { data = data });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetails(int id)
        {
            var data = await _historyRepository.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _historyRepository.DeleteAsync(id);
            return Json(new { success = success });
        }
    }
}
