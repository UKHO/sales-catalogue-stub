using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("EventTypes")]
    public class EventType
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool StateChanging { get; set; }

        public bool Visible { get; set; }
    }
}
