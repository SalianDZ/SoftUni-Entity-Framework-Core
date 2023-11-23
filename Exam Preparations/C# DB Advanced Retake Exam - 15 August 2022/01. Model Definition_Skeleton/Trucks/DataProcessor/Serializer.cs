namespace Trucks.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            XmlHelper helper = new();

            var despatchers = context.Despatchers
                .Where(x => x.Trucks.Count >= 1)
                .OrderByDescending(x => x.Trucks.Count).ThenBy(x => x.Name)
                .Select(d => new ExportDespatcherDto
                {
                    TrucksCount = d.Trucks.Count,
                    DespatcherName = d.Name,
                    Trucks = d.Trucks.Select(t => new ExportTruckInDespatcher
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString()
                    })
                    .OrderBy(x => x.RegistrationNumber)
                    .ToArray()
                }).ToArray();

            return helper.Serialize(despatchers, "Despatchers");
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Where(c => c.ClientsTrucks.Any(ct => ct.Truck.TankCapacity >= capacity))
                .ToArray()
                .Select(c => new
                {
                    c.Name,
                    Trucks = c.ClientsTrucks
                    .Where(ct => ct.Truck.TankCapacity >= capacity)
                    .ToArray()
                    .OrderBy(t => t.Truck.MakeType.ToString())
                    .ThenByDescending(t => t.Truck.CargoCapacity)
                    .Select(ct => new
                    {
                        TruckRegistrationNumber = ct.Truck.RegistrationNumber,
                        ct.Truck.VinNumber,
                        ct.Truck.TankCapacity,
                        ct.Truck.CargoCapacity,
                        CategoryType = ct.Truck.CategoryType.ToString(),
                        MakeType = ct.Truck.MakeType.ToString()
                    })
                    .ToArray()
                })
                .OrderByDescending(c => c.Trucks.Length)
                .ThenBy(t => t.Name)
                .Take(10)
                .ToArray();
            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }
    }
}
