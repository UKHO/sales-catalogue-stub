using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.IO;
using System.Threading.Tasks;

namespace UKHO.SalesCatalogueStub.Api.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {

            await LogRequest(context);
            await LogResponse(context);

        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();

            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);
            _logger.LogInformation(
                "Request {host} {method} {url} {queryString} => {statusCode} {body}",
                context.Request?.Host,
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Request?.QueryString,
                context.Response?.StatusCode,
                ReadStreamInChunks(requestStream)
                );
            context.Request.Body.Position = 0;
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(
                "Response {host} {method} {url} {queryString} => {statusCode} {body}",
                context.Request?.Host,
                context.Request?.Method,
                context.Request?.Path.Value,
                context.Request?.QueryString,
                context.Response?.StatusCode,
                responseText
            );

            await responseBody.CopyToAsync(originalBodyStream);
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }
}