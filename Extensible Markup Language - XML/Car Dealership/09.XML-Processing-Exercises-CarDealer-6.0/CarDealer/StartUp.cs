﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();
            //string inputXml = File.ReadAllText("../../../Datasets/sales.xml");
            string result = GetTotalSalesByCustomer(context);
            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml) 
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportSupplierDto[] supplierDtos =
                xmlHelper.Deserialize<ImportSupplierDto[]>(inputXml, "Suppliers");

            // The second method is just syntax sugar
            // It is written for user experience
            //ImportSupplierDto[] supplierDtos2 =
            //    xmlHelper
            //        .DeserializeCollection<ImportSupplierDto>(inputXml, "Suppliers")
            //        .ToArray();

            ICollection<Supplier> validSuppliers = new HashSet<Supplier>();
            foreach (ImportSupplierDto supplierDto in supplierDtos)
            {
                if (string.IsNullOrEmpty(supplierDto.Name))
                {
                    continue;
                }

                // Manual mapping without AutoMapper
                //Supplier supplier = new Supplier()
                //{
                //    Name = supplierDto.Name,
                //    IsImporter = supplierDto.IsImporter
                //};
                Supplier supplier = mapper.Map<Supplier>(supplierDto);

                validSuppliers.Add(supplier);
            }

            context.Suppliers.AddRange(validSuppliers);
            context.SaveChanges();

            return $"Successfully imported {validSuppliers.Count}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();
            ImportPartDto[] importPartDtos = xmlHelper.Deserialize<ImportPartDto[]>(inputXml, "Parts");

            ICollection<Part> validParts = new HashSet<Part>();

            foreach (var importPartDto in importPartDtos)
            {
                if (string.IsNullOrEmpty(importPartDto.Name) || !context.Suppliers.Any(s => s.Id == importPartDto.SupplierId))
                {
                    continue;
                }
                Part part = mapper.Map<Part>(importPartDto);
                validParts.Add(part);
            }

            context.Parts.AddRange(validParts);
            context.SaveChanges();
            return $"Successfully imported {validParts.Count}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ImportCarDto[] carDtos = xmlHelper.Deserialize<ImportCarDto[]>(inputXml, "Cars");
            ICollection<Car> validCars = new HashSet<Car>();

            foreach (var carDto in carDtos) 
            {
                if (string.IsNullOrEmpty(carDto.Model) || string.IsNullOrEmpty(carDto.Make))
                {
                    continue;
                }

                Car car = mapper.Map<Car>(carDto);

                foreach (var carDtoPart in carDto.Parts.DistinctBy(p => p.PartId))
                {
                    if (!context.Parts.Any(p => p.Id == carDtoPart.PartId))
                    {
                        continue;
                    }

                    PartCar carPart = new PartCar()
                    {
                        PartId = carDtoPart.PartId
                    };
                    car.PartsCars.Add(carPart);
                }

                validCars.Add(car);
            }

            context.Cars.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ImportCustomerDto[] customerDtos = helper.Deserialize<ImportCustomerDto[]>(inputXml, "Customers");
            ICollection<Customer> validCustomers = new HashSet<Customer>();

            foreach (var customerDto in customerDtos)
            {
                if (string.IsNullOrEmpty(customerDto.Name))
                {
                    continue;
                }
                Customer customer = mapper.Map<Customer>(customerDto);
                validCustomers.Add(customer);
            }

            context.AddRange(validCustomers);
            context.SaveChanges();
            return $"Successfully imported {validCustomers.Count}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ImportSaleDto[] saleDtos = helper.Deserialize<ImportSaleDto[]>(inputXml, "Sales");
            ICollection<Sale> validSales = new HashSet<Sale>();

            foreach (var saleDto in saleDtos)
            {
                if (!context.Cars.Any(c => c.Id == saleDto.CarId))
                {
                    continue;
                }

                Sale sale = mapper.Map<Sale>(saleDto);
                validSales.Add(sale);
            }

            context.AddRange(validSales);
            context.SaveChanges();
            return $"Successfully imported {validSales.Count}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ExportCarDto[] cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make).ThenBy(c => c.Model).Take(10)
                .ProjectTo<ExportCarDto>(mapper.ConfigurationProvider)
                .ToArray();

            
            return helper.Serialize<ExportCarDto[]>(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ExportBMWCarsDto[] cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportBMWCarsDto>(mapper.ConfigurationProvider)
                .ToArray();

            return helper.Serialize<ExportBMWCarsDto[]>(cars, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ExportLocalSuppliersDto[] localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>(mapper.ConfigurationProvider)
                .ToArray();

            return helper.Serialize<ExportLocalSuppliersDto[]>(localSuppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ExportCarWithPartsDto[] cars = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithPartsDto>(mapper.ConfigurationProvider)
                .ToArray();

            return helper.Serialize(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var tempDto = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SalesInfo = c.Sales.Select(s => new
                    {
                        Prices = c.IsYoungDriver
                            ? s.Car.PartsCars.Sum(p => Math.Round((double)p.Part.Price * 0.95, 2))
                            : s.Car.PartsCars.Sum(p => (double)p.Part.Price)
                    }).ToArray(),
                })
                .ToArray();

            ExportTotalSalesDto[] totalSalesDtos = tempDto
                .OrderByDescending(t => t.SalesInfo.Sum(s => s.Prices))
                .Select(t => new ExportTotalSalesDto()
                {
                    FullName = t.FullName,
                    BoughtCars = t.BoughtCars,
                    SpentMoney = t.SalesInfo.Sum(s => s.Prices).ToString("f2")
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("customers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportTotalSalesDto[]), xmlRoot);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, totalSalesDtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        private static IMapper InitializeAutoMapper()
            => new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
    }
}