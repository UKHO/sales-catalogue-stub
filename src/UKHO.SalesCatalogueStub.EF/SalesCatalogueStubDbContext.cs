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
            if (!Database.IsSqlServer()) return;

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

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductEdition> ProductEditions { get; set; }
        public DbSet<LifecycleEvent> LifecycleEvents { get; set; }
        public DbSet<PidGeometry> PidGeometries { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<ProductEdition>()
                .HasMany<LifecycleEvent>(pe => pe.LifecycleEvents)
                .WithOne(le => le.ProductEdition)
                .HasForeignKey(le => le.EditionId);

            modelBuilder
                .Entity<EventType>()
                .Property(e => e.Name)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProductEditionStatusEnum)Enum.Parse(typeof(ProductEditionStatusEnum), v));

            modelBuilder
                            .Entity<ProductType>()
                            .Property(e => e.Name)
                            .HasConversion(
                                v => v.ToString(),
                                v => (ProductTypeNameEnum)Enum.Parse(typeof(ProductTypeNameEnum), v));

            modelBuilder
                .Entity<ProductEdition>()
                .Property(e => e.LatestStatus)
                .HasConversion(
                    v => v.ToString(),
                    v => (ProductEditionStatusEnum)Enum.Parse(typeof(ProductEditionStatusEnum), v));

        }
    }
}
