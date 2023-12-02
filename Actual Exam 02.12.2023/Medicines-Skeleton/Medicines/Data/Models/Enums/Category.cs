using System.Xml.Serialization;

namespace Medicines.Data.Models.Enums
{
    public enum Category
    {
        [XmlEnum("0")]
        Analgesic,
        [XmlEnum("1")]
        Antibiotic,
        [XmlEnum("2")]
        Antiseptic,
        [XmlEnum("3")]
        Sedative,
        [XmlEnum("4")]
        Vaccine
    }
}
