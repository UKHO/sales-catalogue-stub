
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UKHO.Logging.EventHubLogProvider;
using UKHO.SalesCatalogueStub.Api.Configuration;
using UKHO.SalesCatalogueStub.Api.Filters;

namespace UKHO.SalesCatalogueStub.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _hostingEnvironment;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _hostingEnvironment = environment;
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters.Add(new StringEnumConverter
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    });
                })
                .AddXmlSerializerFormatters();

            services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("1", new OpenApiInfo
                    {
                        Version = "1",
                        Title = "Sales Catalogue Service API Stub",
                        Description = "Sales Catalogue Service API Stub",
                        Contact = new OpenApiContact()
                        {
                            Name = "Team Tamatoa",
                            Url = new Uri("https://github.com/UKHO/sales-catalogue-stub")
                        }
                    });
                    c.CustomSchemaIds(type => type.FullName);
                    c.IncludeXmlComments(
                        $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{_hostingEnvironment.ApplicationName}.xml");
                    // Sets the basePath property in the Swagger document generated
                    c.DocumentFilter<BasePathFilter>("/v1/productData");

                    // Include DataAnnotation attributes on Controller Action parameters as Swagger validation rules (e.g required, pattern, ..)
                    // Use [ValidateModelState] on Actions to actually validate it in C# as well!
                    c.OperationFilter<GeneratePathParamsValidationFilter>();
                });


            var eventhubLoggingConfig = new EventHubLoggingConfig();
            Configuration.GetSection("EventHubLogging").Bind(eventhubLoggingConfig);

            services.AddHttpContextAccessor();

            services.AddLogging(builder =>
            {

                builder.AddAzureWebAppDiagnostics();
                builder.AddEventHub(options =>
                {
                    options.Environment = eventhubLoggingConfig.Environment;
                    options.DefaultMinimumLogLevel =
                        (LogLevel)Enum.Parse(typeof(LogLevel),
                            eventhubLoggingConfig
                                .MinimumLoggingLevel, true);

                    options.MinimumLogLevels["UKHO"] =
                        (LogLevel)Enum.Parse(typeof(LogLevel),
                            eventhubLoggingConfig
                                .UkhoMinimumLoggingLevel,
                            true);

                    options.EventHubConnectionString =
                        eventhubLoggingConfig.EventHubConnectionString;

                    options.EventHubEntityPath =
                        eventhubLoggingConfig.EntityPath;

                    options.System = eventhubLoggingConfig.System;
                    options.Service = eventhubLoggingConfig.Service;
                    options.NodeName = eventhubLoggingConfig.NodeName;
                    options.AdditionalValuesProvider = ConfigAdditionalValuesProvider;
                });
            });

            services.AddHealthChecks();


            var appRegistrationConfig = new AppRegistrationConfig();
            Configuration.GetSection("AzureAD").Bind(appRegistrationConfig);

            if (string.IsNullOrWhiteSpace(appRegistrationConfig.ClientId))
            {
                throw new ApplicationException("Failed to get AD Configuration");
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="httpContextAccessor"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
                app.UseExceptionHandler("/Error");

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/1/swagger.json", "Sales Catalogue Service API Stub");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            _httpContextAccessor = httpContextAccessor;
        }

        private void ConfigAdditionalValuesProvider(IDictionary<string, object> additionalValues)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                additionalValues["_RemoteIPAddress"] =
                    _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

                additionalValues["_User-Agent"] =
                    _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? string.Empty;

                additionalValues["_AssemblyVersion"] = Assembly
                    .GetExecutingAssembly()
                    .GetCustomAttributes<AssemblyFileVersionAttribute>().Single()
                    .Version;

                additionalValues["_X-Correlation-ID"] =
                    _httpContextAccessor.HttpContext.Request.Headers?[""].FirstOrDefault() ?? string.Empty;
            }
        }
    }
}