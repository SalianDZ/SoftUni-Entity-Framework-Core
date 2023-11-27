using System.Xml.Serialization;

namespace Footballers.DataProcessor.ExportDto
{
    [XmlType("Footballer")]
    public class ExportFootballersDto
    {
        [XmlElement("Name")]
        public string FootbalerName { get; set; } = null!;
        [XmlElement("Position")]
        public string PositionType { get; set; } = null!;
    }
}
