#pragma warning disable 1591
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UKHO.SalesCatalogueStub.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();
        }


        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Request {host} {method} {url} {queryString} => Status Code {statusCode}",
                    context.Request?.Host,
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Request?.QueryString,
                    500);
                context.Response.StatusCode = 500;
                context.Response.Body = Stream.Null;
            }
        }

    }
}