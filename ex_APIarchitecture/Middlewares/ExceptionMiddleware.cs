using ex_APIarchitecture.Common.constants;

namespace ex_APIarchitecture.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.StatusCode = cs_Response.GetStatusCode(cs_Response.ERROR);
                context.Response.ContentType = cs_Response.CT_APPJSON;

                await context.Response.WriteAsJsonAsync(new
                {
                    Status = cs_Response.GetStatusCode(cs_Response.ERROR),
                    Message = cs_Response.ERROR,
                    Detail = ex.Message
                });
            }
        }
    }
}
