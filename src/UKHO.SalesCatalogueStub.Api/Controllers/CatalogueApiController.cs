using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
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
        public virtual IActionResult GetCatalogue([FromRoute][Required] string productType, [FromRoute][Required] string catalogueType, [FromHeader] DateTime? ifModifiedSince)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(InlineResponse200));

            //TODO: Uncomment the next line to return response 304 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(304);

            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400, default(ErrorDescription));

            //TODO: Uncomment the next line to return response 401 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(401);

            //TODO: Uncomment the next line to return response 403 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(403);

            //TODO: Uncomment the next line to return response 404 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(404, default(DefaultErrorResponse));

            //TODO: Uncomment the next line to return response 406 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(406, default(DefaultErrorResponse));

            //TODO: Uncomment the next line to return response 500 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(500, default(DefaultErrorResponse));
            var geoms = _productEditionService.GetCatalogue(null);

            return new ObjectResult(null);
        }
    }
}
