using Microsoft.AspNetCore.Mvc.Controllers;
using System.Diagnostics;
using System.Text;

namespace ex_APIarchitecture.Middlewares
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        string logfilename = "RequestResponseLoggingMiddleware_" + DateTime.Now.ToString("dd_MM_yyyy_HH") + ".txt";

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;

            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            await LogRequestAsync(context);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            await LogResponseAsync(context, stopwatch.ElapsedMilliseconds);

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private async Task LogRequestAsync(HttpContext context)
        {
            context.Request.EnableBuffering();

            var request = context.Request;

            string ip =
                request.Headers["X-Forwarded-For"].FirstOrDefault()
                ?? context.Connection.RemoteIpAddress?.ToString()
                ?? "Unknown";

            var endpoint = context.GetEndpoint();
            string controller = "N/A";
            string action = "N/A";
            string route = request.Path;

            if (endpoint != null)
            {
                var descriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (descriptor != null)
                {
                    controller = descriptor.ControllerName;
                    action = descriptor.ActionName;
                    route = descriptor.AttributeRouteInfo?.Template ?? route;
                }
            }

            var requestBody = await ReadStreamAsync(request.Body);
            request.Body.Position = 0;

            var headers = GetHeaders(request.Headers);
            string userAgent = request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";

            string logMessage =
               Environment.NewLine + "============== DateTime: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==============" + Environment.NewLine +
                "HTTP REQUEST" + Environment.NewLine +
                $"IP           : {ip}" + Environment.NewLine +
                $"User-Agent   : {userAgent}" + Environment.NewLine +
                $"Endpoint     : {controller}.{action}" + Environment.NewLine +
                $"Method       : {request.Method}" + Environment.NewLine +
                $"Route        : {route}" + Environment.NewLine +
                $"Headers      :" + Environment.NewLine +
                $"{headers}" + Environment.NewLine +
                $"Body         :" + Environment.NewLine +
                $"{requestBody}" + Environment.NewLine;

            //Helper.DebugLogCommon(
            //    Path.Combine("", logfilename),
            //    logMessage
            //);
        }
        private async Task LogResponseAsync(HttpContext context, long elapsedMs)
        {
            var response = context.Response;
            response.Body.Seek(0, SeekOrigin.Begin);

            var responseBody = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var headers = GetHeaders(response.Headers);

            string logMessage =
               Environment.NewLine + "============== DateTime: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==============" + Environment.NewLine +
                "HTTP RESPONSE" + Environment.NewLine +
                $"Status Code : {response.StatusCode}" + Environment.NewLine +
                $"Time Taken  : {elapsedMs} ms" + Environment.NewLine +
                $"Headers     :" + Environment.NewLine +
                $"{headers}" + Environment.NewLine +
                $"Body        :" + Environment.NewLine +
                $"{responseBody}" + Environment.NewLine;


            //Helper.DebugLogCommon(
            //            Path.Combine(WebConfig.RequestResponseLog, logfilename),
            //            logMessage
            //        );
        }
        private async Task<string> ReadStreamAsync(Stream stream)
        {
            if (!stream.CanRead) return string.Empty;

            stream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
            var text = await reader.ReadToEndAsync();
            stream.Seek(0, SeekOrigin.Begin);
            return text;
        }
        private string GetHeaders(IHeaderDictionary headers)
        {
            var sb = new StringBuilder();
            foreach (var h in headers)
            {
                sb.Append($"{h.Key}:{h.Value}; ");
            }
            return sb.ToString();
        }
    }
}
