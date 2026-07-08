using AMS.Application.Constants;
using AMS.Application.DTOs;
using AMS.Application.Interfaces;
using AMS.Application.Services;
using System.Net;

namespace AMS.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
       
        public ExceptionHandlingMiddleware(RequestDelegate next , ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, ILogService logService)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ErrorMsgs.UnhandledExceptionOccurred);
                await logService.LogAsync(new LogEntryDto
                {
                    Level = "ERROR",
                    Message = ex.Message,
                    Exception = ex.ToString(),
                    UserEmail = httpContext.User?.Identity?.Name
                });
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(
        HttpContext context)
        {
            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType = "text/html";

            return context.Response.WriteAsync(@"
            <html>
                <head>
                    <title>Error</title>

                    <style>
                        body{
                            font-family:Arial;
                            text-align:center;
                            margin-top:100px;
                        }

                        h1{
                            color:red;
                        }
                    </style>
                </head>

                <body>

                    <h1>
                        Something went wrong
                    </h1>

                    <p>
                        Please contact administrator.
                    </p>

                </body>
            </html>");
        }
    }
}

