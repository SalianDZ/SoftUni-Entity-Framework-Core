using System.Xml.Serialization;

namespace Trucks.Data.Models.Enums
{
    public enum MakeType
    {
        [XmlEnum("0")]
        Daf,
        [XmlEnum("1")]
        Man,
        [XmlEnum("2")]
        Mercedes,
        [XmlEnum("3")]
        Scania,
        [XmlEnum("4")]
        Volvo
    }
}
