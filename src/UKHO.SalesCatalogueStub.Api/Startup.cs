#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UKHO.Logging.EventHubLogProvider;
using UKHO.SalesCatalogueStub.Api.Configuration;
using UKHO.SalesCatalogueStub.Api.EF;
using UKHO.SalesCatalogueStub.Api.Filters;
using UKHO.SalesCatalogueStub.Api.Middleware;
using UKHO.SalesCatalogueStub.Api.Services;

namespace UKHO.SalesCatalogueStub.Api
{
    public class Startup
    {
        private IHttpContextAccessor _httpContextAccessor;

        private readonly IWebHostEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _hostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }

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

                    c.AddSecurityDefinition("jwtBearerAuth", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    });
                });

            var appRegistrationConfig = new AppRegistrationConfig();
            Configuration.GetSection("AppRegistration").Bind(appRegistrationConfig);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Audience = appRegistrationConfig.ClientId;
                    options.Authority = $"{appRegistrationConfig.MicrosoftOnlineLoginUrl}{appRegistrationConfig.TenantId}";
                });

            var eventhubLoggingConfig = new EventHubLoggingConfig();
            Configuration.GetSection("EventHubLogging").Bind(eventhubLoggingConfig);

            var pidDatabaseConfig = new PidDatabaseConfig();
            Configuration.GetSection("PidDatabase").Bind(pidDatabaseConfig);

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

            var dbConnectionString = ConnectionString.Build(pidDatabaseConfig.ServerInstance, pidDatabaseConfig.Database);

            services.AddDbContext<SalesCatalogueStubDbContext>((serviceProvider, options) =>
                options.UseLazyLoadingProxies().UseSqlServer(dbConnectionString, opts => opts.UseNetTopologySuite()));

            services.AddScoped<IProductEditionService, ProductEditionService>();

            services.AddHealthChecks();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            if (!env.IsDevelopment())
            {
                //TODO: Enable production exception handling (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)

                app.UseHsts();
            }
            
            app.UseRequestResponseLogging();
            app.UseExceptionHandling();

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
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