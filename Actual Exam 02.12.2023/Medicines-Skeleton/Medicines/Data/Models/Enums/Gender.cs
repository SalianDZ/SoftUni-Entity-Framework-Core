using System.Xml.Serialization;

namespace Medicines.Data.Models.Enums
{
    public enum Gender
    {
        [XmlEnum("0")]
        Male,
        [XmlEnum("1")]
        Female
    }
}
