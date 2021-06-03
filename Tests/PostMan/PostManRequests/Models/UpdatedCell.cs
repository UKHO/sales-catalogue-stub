using System;

namespace PostManRequests.Models
{
    public class UpdatedCell
        //Note if you type prop it fills it out for you
    {
        public string productName { get; set; }
        public DateTime? baseCellIssueDate { get; set; }
        public int baseCellEditionNumber { get; set; }
        public DateTime? issueDateLatestUpdate { get; set; }
        public int? latestUpdateNumber { get; set; }
        public int fileSize { get; set; }
        public float cellLimitSouthernmostLatitude { get; set; }
        public float cellLimitWesternmostLatitude { get; set; }
        public float cellLimitNorthernmostLatitude { get; set; }
        public float cellLimitEasternmostLatitude { get; set; }
        public Datacoveragecoordinate[] dataCoverageCoordinates { get; set; }
        public bool compression { get; set; }
        public bool encryption { get; set; }
        public int baseCellUpdateNumber { get; set; }
        public object lastUpdateNumberPreviousEdition { get; set; }
        public string baseCellLocation { get; set; }
        public object[] cancelledCellReplacements { get; set; }
        public DateTime? issueDatePreviousUpdate { get; set; }
        public int? cancelledEditionNumber { get; set; }
    }
    public class Datacoveragecoordinate
    {
        public object latitude { get; set; }
        public object longitude { get; set; }
    }
}