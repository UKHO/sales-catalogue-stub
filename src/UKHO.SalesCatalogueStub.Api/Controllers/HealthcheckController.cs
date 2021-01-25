using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace UKHO.SalesCatalogueStub.Api.Controllers
{
    /// <summary>
    ///     Healthcheck Controller
    /// </summary>
    [Route("/")]
    [ApiController]
    public class Healthcheck1Controller : ControllerBase
    {
        /// <summary>
        ///     Healthcheck
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("healthcheck")]
        public IActionResult Get()
        {
            //_logger.LogInformation("Healthcheck GET");
            return Ok(Assembly.GetExecutingAssembly().GetName().Name + " is ok.");
        }
    }
}