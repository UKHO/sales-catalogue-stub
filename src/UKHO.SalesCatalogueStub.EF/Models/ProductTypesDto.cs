using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.EF.Models
{
    [Table("ProductTypes")]
    public class ProductTypesDto
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public ProductTypeNameEnum Name { get; set; }

    }
}
