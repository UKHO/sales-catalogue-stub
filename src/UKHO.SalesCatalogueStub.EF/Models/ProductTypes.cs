using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("ProductTypes")]
    public class ProductTypes
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public ProductTypeNameEnum Name { get; set; }

    }
}
