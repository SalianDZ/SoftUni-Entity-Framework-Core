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
            string inputJson = File.ReadAllText(@"../../../Datasets/cars.json");
            string result = ImportCars(context, inputJson);
            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            ImportSuppliersDto[] suppliersDto =
                JsonConvert.DeserializeObject<ImportSuppliersDto[]>(inputJson);

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();
            foreach (var supplierDto in suppliersDto)
            {
                Supplier supplier = mapper.Map<Supplier>(supplierDto);
                validSuppliers.Add(supplier);
            }

            context.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            ImportPartDto[] parts =
                JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);

            ICollection<Part> validParts = new HashSet<Part>();
            var suppliers = context.Suppliers.ToList();
            foreach (var partDto in parts)
            {
                Part part = mapper.Map<Part>(partDto);
                if (suppliers.Any(s => s.Id == part.SupplierId))
                {
                    validParts.Add(part);
                }
            }

            context.AddRange(validParts);
            context.SaveChanges();

            return $"Successfully imported {validParts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            List<ImportCarDtoJson> cars = JsonConvert.DeserializeObject<List<ImportCarDtoJson>>(inputJson);

            foreach (var car in cars)
            {
                Car currentCar = new Car()
                {
                    Make = car.Make,
                    Model = car.Model,
                    TravelledDistance = car.TravelledDistance
                };

                foreach (var part in car.PartsId)
                {
                    bool isValid = currentCar.PartsCars.FirstOrDefault(p => p.PartId == part) == null;

                    bool isCurrentPartValid = context.Parts.FirstOrDefault(p => p.Id == part) != null;

                    if (isValid && isCurrentPartValid)
                    {
                        currentCar.PartsCars.Add(new PartCar()
                        {
                            PartId = part
                        });
                    }
                }

                context.Cars.Add(currentCar);
            }

            context.SaveChanges();
            return $"Successfully imported {cars.Count}.";
        }
    }
}