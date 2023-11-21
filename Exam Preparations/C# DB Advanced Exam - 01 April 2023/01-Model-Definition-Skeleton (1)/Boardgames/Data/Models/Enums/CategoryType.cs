using System.Xml.Serialization;

namespace Boardgames.Data.Models.Enums
{
    public enum CategoryType
    {
        [XmlEnum("0")]
        Abstract = 0,
        [XmlEnum("1")]
        Children = 1,
        [XmlEnum("2")]
        Family = 2,
        [XmlEnum("3")]
        Party = 3,
        [XmlEnum("4")]
        Strategy = 4
    }
}
