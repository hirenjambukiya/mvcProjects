using DynamicExcel.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicExcel.Web.Controllers
{
    public class QueryController : Controller
    {
        private readonly IDatabaseConnectionRepository _connectionRepo;
        private readonly IQueryService _queryService;

        public QueryController(IDatabaseConnectionRepository connectionRepo, IQueryService queryService)
        {
            _connectionRepo = connectionRepo;
            _queryService = queryService;
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
        public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return Json(new { success = false, message = "Query cannot be empty." });
            }

            var connection = _connectionRepo.GetById(request.ConnectionId);
            if (connection == null)
            {
                return Json(new { success = false, message = "Invalid database connection." });
            }

            // DataTables pagination parameters from JSON request body
            int skip = request.Start;
            int pageSize = request.Length > 0 ? request.Length : 10;
            string draw = request.Draw;

            var result = await _queryService.ExecuteQueryAsync(connection, request.Query, skip, pageSize);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }

            // Return in DataTables format
            return Json(new
            {
                draw = draw,
                recordsTotal = result.TotalRecords,
                recordsFiltered = result.TotalRecords,
                data = result.Data,
                columns = result.Columns,
                success = true,
                executionTime = result.ExecutionTimeSeconds
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetTables(int connectionId)
        {
            var connection = _connectionRepo.GetById(connectionId);
            if (connection == null)
            {
                return Json(new { success = false, message = "Invalid connection." });
            }

            try
            {
                var connectionString = connection.GetConnectionString();
                using var sqlConnection = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
                await sqlConnection.OpenAsync();

                var sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' ORDER BY TABLE_NAME";
                var tables = await Dapper.SqlMapper.QueryAsync<string>(sqlConnection, sql);

                return Json(new { success = true, tables = tables.ToList() });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

    public class QueryRequest
    {
        public int ConnectionId { get; set; }
        public string Query { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public string Draw { get; set; }
    }
}
