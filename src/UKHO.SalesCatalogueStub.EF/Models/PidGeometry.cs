using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("PIDGeometries")]
    public class PidGeometry
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Ordinal { get; set; }

        public bool IsBoundingBox { get; set; }

        public bool IsPanel { get; set; }

        [Column("UnitOfSaleID")]
        public Guid? UnitOfSaleId { get; set; }

        [Column("ProductEditionID")]
        public Guid? ProductEditionId { get; set; }

        public virtual ProductEdition ProductEdition { get; set; }

        public virtual Geometry Geom { get; set; }

        public string Scale { get; set; }

    }
}