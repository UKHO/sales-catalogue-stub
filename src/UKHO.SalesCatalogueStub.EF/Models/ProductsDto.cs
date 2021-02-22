using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("Products")]
    public class ProductsDto
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public string Identifier { get; set; }
        [Column("ProductTypeID")]
        public Guid ProductTypeId { get; set; }
        [Column("ProductSourceFeedID")]
        public Guid? ProductSourceFeedId { get; set; }

        public DateTime? LastUpdated { get; set; }

        public virtual ProductTypesDto ProductType { get; set; }

        public virtual ICollection<ProductEditionsDto> ProductEditions { get; set; }


    }
}
