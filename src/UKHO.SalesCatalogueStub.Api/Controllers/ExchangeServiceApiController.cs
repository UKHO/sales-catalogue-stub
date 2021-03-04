using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using UKHO.SalesCatalogueStub.Api.Attributes;
using UKHO.SalesCatalogueStub.Api.Models;
using UKHO.SalesCatalogueStub.Api.Services;
using Products = UKHO.SalesCatalogueStub.Api.Models.Products;

namespace UKHO.SalesCatalogueStub.Api.Controllers
{
    /// <summary>
    /// </summary>
    [ApiController]
    [Authorize(Roles = "ExchangeServiceReader")]
    public class ExchangeServiceApiController : ControllerBase
    {
        private readonly IProductEditionService _productEditionService;

        /// <inheritdoc />
        public ExchangeServiceApiController(IProductEditionService productEditionService)
        {
            _productEditionService = productEditionService;
        }

        /// <summary>
        /// Get the all releasable changes to products since a date
        /// </summary>
        /// <param name="productType">The type of product         </param>
        /// <param name="sinceDateTime">The date and time from which changes are requested. Any changes since the date will be returned.   </param>
        /// <response code="200">A JSON body of product objects</response>
        /// <response code="304">Not modified.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorised.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Not found.</response>
        /// <response code="406">Not acceptable.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpGet]
        [Route("/v1/productData/{productType}/products")]
        //[Authorize(AuthenticationSchemes = BearerAuthenticationHandler.SchemeName)]
        [ValidateModelState]
        [SwaggerOperation("GetProducts")]
        [SwaggerResponse(statusCode: 200, type: typeof(Products), description: "A JSON body of product objects")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDescription), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(DefaultErrorResponse), description: "Not found.")]
        [SwaggerResponse(statusCode: 406, type: typeof(DefaultErrorResponse), description: "Not acceptable.")]
        [SwaggerResponse(statusCode: 500, type: typeof(DefaultErrorResponse), description: "Internal Server Error.")]
        public virtual async Task<IActionResult> GetProducts([FromRoute][Required] string productType, [FromQuery] DateTime? sinceDateTime)
        {
            if (!sinceDateTime.HasValue)
            {
                return StatusCode(400, default(DefaultErrorResponse));
            }

            var productEditions = await _productEditionService.GetProductEditionsSinceDateTime(sinceDateTime.Value);

            return !productEditions.Any() ? StatusCode(304, default(ErrorDescription)) : StatusCode(200, JsonConvert.SerializeObject(productEditions, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        /// <summary>
        /// Get the latest baseline, releasable versions for requested products
        /// </summary>
        /// <param name="productType">The type of product         </param>
        /// <param name="body">The JSON body containing product identifiers.  
        /// </param>
        /// <response code="200">A JSON body of product objects</response>
        /// <response code="304">Not modified.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorised.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Not found.</response>
        /// <response code="406">Not acceptable.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Route("/v1/productData/{productType}/products/productIdentifiers")]
        //[Authorize(AuthenticationSchemes = BearerAuthenticationHandler.SchemeName)]
        [ValidateModelState]
        [SwaggerOperation("PostProductIdentifiers")]
        [SwaggerResponse(statusCode: 200, type: typeof(Products), description: "A JSON body of product objects")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDescription), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(DefaultErrorResponse), description: "Not found.")]
        [SwaggerResponse(statusCode: 406, type: typeof(DefaultErrorResponse), description: "Not acceptable.")]
        [SwaggerResponse(statusCode: 500, type: typeof(DefaultErrorResponse), description: "Internal Server Error.")]
        public virtual IActionResult PostProductIdentifiers([FromRoute][Required] string productType, [FromBody] List<string> body)
        {
            var productVersions = _productEditionService.GetProductEditions(body);

            return !productVersions.Any() ? StatusCode(400, default(ErrorDescription)) : StatusCode(200, JsonConvert.SerializeObject(productVersions, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }

        /// <summary>
        /// Get the latest baseline, releasable versions for requested products since a specified version
        /// </summary>
        /// <param name="productType">The type of product         </param>
        /// <param name="body">The JSON body containing product versions.
        /// </param>
        /// <response code="200">A JSON body of product objects</response>
        /// <response code="304">Not modified.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorised.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Not found.</response>
        /// <response code="406">Not acceptable.</response>
        /// <response code="500">Internal Server Error.</response>
        [HttpPost]
        [Route("/v1/productData/{productType}/products/productVersions")]
        //[Authorize(AuthenticationSchemes = BearerAuthenticationHandler.SchemeName)]
        [ValidateModelState]
        [SwaggerOperation("PostProductVersions")]
        [SwaggerResponse(statusCode: 200, type: typeof(Products), description: "A JSON body of product objects")]
        [SwaggerResponse(statusCode: 400, type: typeof(ErrorDescription), description: "Bad request.")]
        [SwaggerResponse(statusCode: 404, type: typeof(DefaultErrorResponse), description: "Not found.")]
        [SwaggerResponse(statusCode: 406, type: typeof(DefaultErrorResponse), description: "Not acceptable.")]
        [SwaggerResponse(statusCode: 500, type: typeof(DefaultErrorResponse), description: "Internal Server Error.")]
        public virtual IActionResult PostProductVersions([FromRoute][Required] string productType, [FromBody] List<ProductVersionsInner> body)
        {
            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(Products));

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
            string exampleJson = null;
            exampleJson = "{\n  \"productName\" : \"AU895561\",\n  \"editionNumber\" : 4,\n  \"updateNumbers\" : [ 5, 6, 7 ],\n  \"fileSize\" : 100\n}";
            var example = exampleJson != null
            ? JsonConvert.DeserializeObject<Products>(exampleJson)
            : default(Products);            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
