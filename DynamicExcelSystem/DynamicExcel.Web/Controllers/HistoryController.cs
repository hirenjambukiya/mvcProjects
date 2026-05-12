using DynamicExcel.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicExcel.Web.Controllers
{
    public class HistoryController : Controller
    {
        private readonly IImportHistoryRepository _historyRepo;

        public HistoryController(IImportHistoryRepository historyRepo)
        {
            _historyRepo = historyRepo;
        }

        public IActionResult Index()
        {
            var history = _historyRepo.GetAll();
            return View(history);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _historyRepo.Delete(id);
            return Json(new { success = true, message = "History record deleted successfully." });
        }
    }
}
