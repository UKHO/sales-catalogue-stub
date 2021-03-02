using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace UKHO.SalesCatalogueStub.Api.EF.Models
{
    [Table("ProductEditions")]
    public class ProductEdition
    {
        [Column("ID")]
        public Guid Id { get; set; }

        public virtual Product Product { get; set; }

        public string EditionIdentifier { get; set; }

        public string EditionNumber { get; set; }

        public DateTime EditionDate { get; set; }

        public string SapIpn { get; set; }

        public ProductEditionStatusEnum LatestStatus { get; set; }

        public string Title { get; set; }

        public DateTime? WithdrawalDate { get; set; }
        [Column("ProductID")]
        public Guid? ProductId { get; set; }
        [Column("PIDTombstoneID")]
        public Guid? PidTombstoneId { get; set; }
        [Column("LifecycleEventID")]
        public Guid? LifecycleEventId { get; set; }

        public DateTime? LastUpdated { get; set; }

        public int? Week { get; set; }

        public string ExportType { get; set; }

        public DateTime? LastUpdateIssueDate { get; set; }

        public int? LastUpdateNumberPrevEdition { get; set; }

        public int? UpdateNumber { get; set; }

        public int? UsageBand { get; set; }

        public DateTime? BaseIssueDate { get; set; }

        public int? LastReissueUpdateNumber { get; set; }
        [Column("BaseCD")]
        public int? BaseCd { get; set; }
        [Column("UpdateCD")]
        public int? UpdateCd { get; set; }

        public DateTime? ChartNewEditionDate { get; set; }

        public DateTime? RasterChartIssueDate { get; set; }

        public string InternationalNo { get; set; }

        public string PublicationFileName { get; set; }

        public string EditionName { get; set; }
        [Column("ProductDownloadSizeKB")]
        public string ProductDownloadSizeKb { get; set; }

        public string PublicationHash { get; set; }

        public string SearchKeywords { get; set; }

        public DateTime? LastUpdateDate { get; set; }

        public string LastUpdateWeekNumber { get; set; }

        public string Position { get; set; }

    }
}
