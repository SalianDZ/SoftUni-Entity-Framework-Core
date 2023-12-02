using System.Xml.Serialization;

namespace Medicines.Data.Models.Enums
{
    public enum AgeGroup
    {
        [XmlEnum("0")]
        Child,
        [XmlEnum("1")]
        Adult,
        [XmlEnum("2")]
        Senior
    }
}
