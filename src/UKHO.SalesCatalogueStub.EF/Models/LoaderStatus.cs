using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    public class LoaderStatus
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public Guid? ParentID { get; set; }

        public AreaNameEnum AreaName { get; set; }

        public string Year { get; set; }

        public string Week { get; set; }

        public string SourceFeedPath { get; set; }

        public string CataloguePath { get; set; }

        public DateTime? DateEntered { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public string Status { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string StartedBy { get; set; }

        public bool? HadErrors { get; set; }

    }
}
