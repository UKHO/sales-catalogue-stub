using System;
using System.Collections.Generic;
using System.Text;

namespace PostManRequests.Models
{
    public class GetPaperChartResponse

    {
        public string Url { get; set; }
        public string EditionNumber { get; set; }
        public string NmYear { get; set; }
        public string NmNumber { get; set; }
        public string State { get; set; }
    }

    public class ChartProduct
    {
        public string ChartNumber { get; set; }
        public DateTime PublicationDate { get; set; }
        public string State { get; set; }
        public object LatestState { get; set; }
        public DateTime LatestStateStartDate { get; set; }
        public bool HasActiveContentVersion { get; set; }
        public List<Version> Versions { get; set; }
    }

}
