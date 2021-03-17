using Microsoft.AspNetCore.Builder;

namespace UKHO.SalesCatalogueStub.Api.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
