using System.Xml.Serialization;

namespace Footballers.Data.Models.Enums
{
    public enum BestSkillType
    {
        [XmlEnum("0")]
        Defence,
        [XmlEnum("1")]
        Dribble,
        [XmlEnum("2")]
        Pass,
        [XmlEnum("3")]
        Shoot,
        [XmlEnum("4")]
        Speed
    }
}
