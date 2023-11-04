using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static IMapper mapper;
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            string inputJson = File.ReadAllText(@"../../../Datasets/suppliers.json");
            string result = ImportSuppliers(context, inputJson);
            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            ImportSuppliersDto[] suppliers =
                JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();
            foreach (var supplierDto in suppliers)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);
                validSuppliers.Add(supplier);
            }

            context.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }
    }
}