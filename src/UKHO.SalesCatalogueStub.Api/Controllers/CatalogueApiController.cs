using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using UKHO.SalesCatalogueStub.Api.Attributes;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Authorize(Roles = "CatalogueReader")]
    public class CatalogueApiController : ControllerBase
    {
        private readonly IProductEditionService _productEditionService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productEditionService"></param>
        public CatalogueApiController(IProductEditionService productEditionService)
        {
            _productEditionService = productEditionService;
        }

        /// <summary>
        /// Get the data for a catalogue
        /// </summary>
        /// <param name="productType">The type of product         </param>
        /// <param name="catalogueType">The type of catalogue data requested        </param>
        /// <param name="ifModifiedSince">The date and time since items have changed. </param>
        /// <response code="200">A JSON body of data for the requested catalogue</response>
        /// <response code="304">Not modified.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorised.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Not found.</response>
        /// <response code="406">Not acceptable.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpGet]
        [Route("/v1/productData/{productType}/catalogue/{catalogueType}")]
        //[Authorize(AuthenticationSchemes = BearerAuthenticationHandler.SchemeName)]
        [ValidateModelState]
        [SwaggerOperation("GetCatalogue")]
        [SwaggerResponse(statusCode: 200, type: typeof(InlineResponse200), description: "A JSON body of data for the requested catalogue")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDescription), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(DefaultErrorResponse), description: "Not found.")]
        [SwaggerResponse(statusCode: 406, type: typeof(DefaultErrorResponse), description: "Not acceptable.")]
        [SwaggerResponse(statusCode: 500, type: typeof(DefaultErrorResponse), description: "Internal Server Error.")]
        public virtual async Task<IActionResult> GetCatalogue([FromRoute][Required] string productType, [FromRoute][Required] string catalogueType, [FromHeader] DateTime? ifModifiedSince)
        {
            var checkIfCatalogueModified = await _productEditionService.CheckIfCatalogueModified(ifModifiedSince);

            Response?.Headers.Add("Last-Modified", checkIfCatalogueModified.dateEntered?.ToString());

            if (!checkIfCatalogueModified.isModified)
            {
                return StatusCode(304, null);
            }

            var catalogue = await _productEditionService.GetCatalogue();

            return StatusCode(200, catalogue);
        }
    }
}
