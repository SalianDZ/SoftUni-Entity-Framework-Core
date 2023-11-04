using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            //string inputJson = File.ReadAllText(@"../../../Datasets/sales.json");
            string result = GetLocalSuppliers(context);
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
                    TraveledDistance = car.TravelledDistance
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

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ImportCustomerDto[] customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            foreach (var customerDto in customerDtos)
            {
                Customer customer = new()
                {
                    Name = customerDto.Name,
                    BirthDate = customerDto.BirthDate,
                    IsYoungDriver = customerDto.IsYoungDriver
                };

                context.Customers.Add(customer);
            }

            context.SaveChanges();
            return $"Successfully imported {customerDtos.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);
            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new 
            {
                c.Name,
                BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                c.IsYoungDriver
            })
                .ToList();

            string customersToJson = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return customersToJson;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carsByToyota = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new 
                {
                c.Id,
                c.Make,
                c.Model,
                c.TraveledDistance
                })
                .ToList();

            string carsToJson = JsonConvert.SerializeObject(carsByToyota, Formatting.Indented);
            return carsToJson;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
           var localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                   {
                        s.Id,
                        s.Name,
                        PartsCount = s.Parts.Count
                   })
                .ToList();

            return JsonConvert.SerializeObject(localSuppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars.Select(p => new
                    {
                        p.Part.Name,
                        Price = $"{p.Part.Price:F2}"
                    })
                });

            string carsToJson = JsonConvert.SerializeObject(carsWithParts, Formatting.Indented);
            return carsToJson;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var totalSales = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new 
            {
                fullName = c.Name,
                boughtCars = c.Sales.Count(),
                spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(cp => cp.Part.Price))
            })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();

            string customersToJson = JsonConvert.SerializeObject(totalSales, Formatting.Indented);
            return customersToJson;
        }
    }
}