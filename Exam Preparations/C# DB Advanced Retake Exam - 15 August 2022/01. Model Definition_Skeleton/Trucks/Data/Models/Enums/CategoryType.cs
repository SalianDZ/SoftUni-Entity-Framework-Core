using System.Xml.Serialization;

namespace Trucks.Data.Models.Enums
{
    public enum CategoryType
    {
        [XmlEnum("0")]
        Flatbed,
        [XmlEnum("1")]
        Jumbo,
        [XmlEnum("2")]
        Refrigerated,
        [XmlEnum("3")]
        Semi
    }
}
