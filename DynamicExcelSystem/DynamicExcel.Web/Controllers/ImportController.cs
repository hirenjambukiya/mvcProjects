using DynamicExcel.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicExcel.Web.Controllers
{
    public class ImportController : Controller
    {
        private readonly IDatabaseConnectionRepository _connectionRepo;
        private readonly IExcelService _excelService;
        private readonly IImportHistoryRepository _historyRepo;

        public ImportController(IDatabaseConnectionRepository connectionRepo, IExcelService excelService, IImportHistoryRepository historyRepo)
        {
            _connectionRepo = connectionRepo;
            _excelService = excelService;
            _historyRepo = historyRepo;
        }

        public IActionResult Index()
        {
            var connections = _connectionRepo.GetAll().ToList();
            if (!connections.Any())
            {
                TempData["WarningMessage"] = "Please configure a database connection first.";
                return RedirectToAction("Index", "Connections");
            }
            
            ViewBag.Connections = connections;
            return View();
        }

        [HttpPost]
        [RequestSizeLimit(104857600)] // 100 MB limit
        public async Task<IActionResult> Upload(IFormFile file, int connectionId)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { success = false, message = "Please select a file." });
            }

            var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return Json(new { success = false, message = "Only Excel files (.xlsx, .xls) are allowed." });
            }

            var connection = _connectionRepo.GetById(connectionId);
            if (connection == null)
            {
                return Json(new { success = false, message = "Invalid database connection selected." });
            }

            using var stream = file.OpenReadStream();
            var result = await _excelService.ImportExcelAsync(stream, file.FileName, connection);

            var history = new DynamicExcel.Core.Entities.ImportHistory
            {
                FileName = file.FileName,
                ConnectionId = connection.Id,
                ConnectionName = connection.ConnectionName,
                ImportDate = System.DateTime.Now,
                TotalSheets = result.TotalSheetsProcessed,
                TotalRecords = result.TotalRecordsImported,
                Success = result.Success,
                ErrorMessage = result.Success ? string.Empty : result.Message,
                ExecutionTimeSeconds = result.ExecutionTimeSeconds
            };

            _historyRepo.Add(history);

            return Json(result);
        }
    }
}
