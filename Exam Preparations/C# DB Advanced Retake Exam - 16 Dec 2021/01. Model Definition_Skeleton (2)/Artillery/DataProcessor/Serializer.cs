
namespace Artillery.DataProcessor
{
    using Artillery.Data;
    using Artillery.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Text;
    using System.Text.Json.Nodes;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportShells(ArtilleryContext context, double shellWeight)
        {

            var shells = context.Shells
                .Where(s => s.ShellWeight > shellWeight)
                .OrderBy(s => s.ShellWeight)
                .ToArray()
                .Select(s => new
                {
                    s.ShellWeight,
                    s.Caliber,
                    Guns = s.Guns
                    .Where(g => g.GunType.ToString() == "AntiAircraftGun")
                    .ToArray()
                    .Select(s => new
                    {
                        GunType = s.GunType.ToString(),
                        s.GunWeight,
                        s.BarrelLength,
                        Range = GetRangeMethod(s.Range)
                    })
                    .OrderByDescending(g => g.GunWeight)
                    .ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(shells, Formatting.Indented);
        }

        public static string ExportGuns(ArtilleryContext context, string manufacturer)
        {
            var guns = context.Guns
                .Where(g => g.Manufacturer.ManufacturerName == manufacturer)
                .OrderBy(g => g.BarrelLength)
                .ToArray()
                .Select(g => new ExportGunsDto()
                {
                    Manufacturer = g.Manufacturer.ManufacturerName,
                    GunType = g.GunType.ToString(),
                    GunWeight = g.GunWeight,
                    BarrelLength = g.BarrelLength,
                    Range = g.Range,
                    Countries = g.CountriesGuns.Where(c => c.Country.ArmySize > 4500000)
                    .ToArray()
                        .Select(cg => new ExportCountryDto()
                        {
                            Country = cg.Country.CountryName,
                            ArmySize = cg.Country.ArmySize
                        })
                        .OrderBy(cg => cg.ArmySize)
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot =
                new XmlRootAttribute("Guns");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportGunsDto[]), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, guns, namespaces);

            return sb.ToString().TrimEnd();
        }

        private static string GetRangeMethod(int range)
        {
            return range > 3000 ? "Long-range" : "Regular range";
        }
    }
}
