using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("LifecycleEvents")]
    public class LifecycleEvent
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public DateTime EventDate { get; set; }

        public string EventReasonText { get; set; }

        [Column("UnitOfSaleID")]
        public Guid UnitOfSaleId { get; set; }

        public virtual EventType EventType { get; set; }

        [Column("EventTypeID")]
        public Guid EventTypeId { get; set; }

        public virtual ProductEdition ProductEdition { get; set; }

        [Column("EditionID")]
        public Guid EditionId { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
