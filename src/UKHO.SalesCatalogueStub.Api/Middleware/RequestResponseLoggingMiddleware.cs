#pragma warning disable 1591
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace UKHO.SalesCatalogueStub.Api.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                _logger.LogInformation(
                    "Request {host} {method} {url} {queryString} => Status Code {statusCode}",
                    context.Request?.Host,
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Request?.QueryString,
                    context.Response?.StatusCode);
            }
        }

    }
}