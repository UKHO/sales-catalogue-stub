using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using UKHO.SalesCatalogueStub.Api.EF.Models;

namespace UKHO.SalesCatalogueStub.Api.EF
{
    public sealed class SalesCatalogueStubDbContext : DbContext
    {
        public SalesCatalogueStubDbContext(DbContextOptions<SalesCatalogueStubDbContext> options)
            : base(options)
        {
            if (Database.GetDbConnection() is SqlConnection dbConnection)
            {
                dbConnection.AccessToken = new AzureServiceTokenProvider()
                    .GetAccessTokenAsync("https://database.windows.net/").Result;
            }
            else
            {
                throw new ApplicationException("Could not configure Db AccessToken as the DbConnection is null");
            }

        }

        public DbSet<ProductsDto> Products { get; set; }
        public DbSet<ProductEditionsDto> ProductEditions { get; set; }

        public DbSet<ProductTypesDto> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ProductTypesDto>()
                .Property(e => e.Name)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProductTypeNameEnum)Enum.Parse(typeof(ProductTypeNameEnum), v));

            modelBuilder
                .Entity<ProductEditionsDto>()
                .Property(e => e.LatestStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProductEditionStatusEnum)Enum.Parse(typeof(ProductEditionStatusEnum), v));

        }
    }
}
