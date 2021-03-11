using System.Collections.Generic;
using System.Xml.Serialization;
#pragma warning disable 1591

namespace UKHO.SalesCatalogueStub.Api.Models
{
    [XmlRoot(ElementName = "BoundingBox")]
    public class BoundingBox
    {
        [XmlElement(ElementName = "NorthLimit")]
        public double? NorthLimit { get; set; }
        [XmlElement(ElementName = "SouthLimit")]
        public double? SouthLimit { get; set; }
        [XmlElement(ElementName = "EastLimit")]
        public double? EastLimit { get; set; }
        [XmlElement(ElementName = "WestLimit")]
        public double? WestLimit { get; set; }
    }

    [XmlRoot(ElementName = "Position")]
    public class Position
    {
        [XmlAttribute(AttributeName = "latitude")]
        public string Latitude { get; set; }
        [XmlAttribute(AttributeName = "longitude")]
        public string Longitude { get; set; }
    }

    [XmlRoot(ElementName = "Polygon")]
    public class Polygon
    {
        [XmlElement(ElementName = "Position")]
        public List<Position> Position { get; set; }
    }

    [XmlRoot(ElementName = "GeographicLimit")]
    public class GeographicLimit
    {
        [XmlElement(ElementName = "BoundingBox")]
        public BoundingBox BoundingBox { get; set; }
        [XmlElement(ElementName = "Polygon")]
        public Polygon Polygon { get; set; }
    }

    [XmlRoot(ElementName = "Folio")]
    public class Folio
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "ChartStatus")]
    public class ChartStatus
    {
        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "ReplacedByList")]
    public class ReplacedByList
    {
        [XmlElement(ElementName = "ReplacedBy")]
        public string ReplacedBy { get; set; }
    }

    [XmlRoot(ElementName = "Status")]
    public class Status
    {
        [XmlElement(ElementName = "ChartStatus")]
        public ChartStatus ChartStatus { get; set; }
        [XmlElement(ElementName = "ReplacedByList")]
        public ReplacedByList ReplacedByList { get; set; }
    }

    [XmlRoot(ElementName = "Unit")]
    public class Unit
    {
        [XmlElement(ElementName = "ID")]
        public string Id { get; set; }
    }

    [XmlRoot(ElementName = "CD")]
    public class Cd
    {
        [XmlElement(ElementName = "Base")]
        public int? Base { get; set; }
        [XmlElement(ElementName = "Update")]
        public int? Update { get; set; }
    }

    [XmlRoot(ElementName = "Metadata")]
    public class Metadata
    {
        [XmlElement(ElementName = "DatasetTitle")]
        public string DatasetTitle { get; set; }
        [XmlElement(ElementName = "Scale")]
        public string Scale { get; set; }
        [XmlElement(ElementName = "GeographicLimit")]
        public GeographicLimit GeographicLimit { get; set; }
        [XmlElement(ElementName = "Folio")]
        public Folio Folio { get; set; }
        [XmlElement(ElementName = "SAP_IPN")]
        public string SapIpn { get; set; }
        [XmlElement(ElementName = "CatalogueNumber")]
        public string CatalogueNumber { get; set; }
        [XmlElement(ElementName = "Status")]
        public Status Status { get; set; }
        [XmlElement(ElementName = "Unit")]
        public Unit Unit { get; set; }
        [XmlElement(ElementName = "DSNM")]
        public string Dsnm { get; set; }
        [XmlElement(ElementName = "Usage")]
        public string Usage { get; set; }
        [XmlElement(ElementName = "Edtn")]
        public string Edtn { get; set; }
        [XmlElement(ElementName = "Base_isdt")]
        public string BaseIsdt { get; set; }
        [XmlElement(ElementName = "UPDN")]
        public string Updn { get; set; }
        [XmlElement(ElementName = "Last_update_isdt")]
        public string LastUpdateIsdt { get; set; }
        [XmlElement(ElementName = "Last_reissue_UPDN")]
        public string LastReissueUpdn { get; set; }
        [XmlElement(ElementName = "CD")]
        public Cd Cd { get; set; }
    }

    [XmlRoot(ElementName = "ENC")]
    public class Enc
    {
        [XmlElement(ElementName = "ShortName")]
        public string ShortName { get; set; }
        [XmlElement(ElementName = "Metadata")]
        public Metadata Metadata { get; set; }
    }

}
