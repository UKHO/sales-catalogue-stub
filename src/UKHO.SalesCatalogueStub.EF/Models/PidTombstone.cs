using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("PidTombstones")]
    public class PidTombstone
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public string XmlData { get; set; }

        public DateTime? LastUpdated { get; set; }

    }
}
