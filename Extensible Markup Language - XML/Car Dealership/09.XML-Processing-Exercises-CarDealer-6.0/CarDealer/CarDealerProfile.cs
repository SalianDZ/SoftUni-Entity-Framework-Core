using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //ImportMaps
            CreateMap<ImportSupplierDto, Supplier>();

            CreateMap<ImportPartDto, Part>();

            CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());

            CreateMap<ImportCustomerDto, Customer>();

            CreateMap<ImportSaleDto, Sale>();

            //ExportMaps
            CreateMap<Car, ExportCarDto>();

            CreateMap<Car, ExportBMWCarsDto>();

            CreateMap<Supplier, ExportLocalSuppliersDto>()
            .ForMember(
                dest => dest.PartsCount,
                opt => opt.MapFrom(src => src.Parts.Count)
            );

            CreateMap<Part, ExportCarPartDto>();

            CreateMap<Car, ExportCarWithPartsDto>()
                .ForMember(d => d.Parts,
                opt => opt.MapFrom(s => s.PartsCars.Select(pc => pc.Part).OrderByDescending(p => p.Price).ToArray()));

            CreateMap<Customer, ExportTotalSalesDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name))
            .ForMember(d => d.BoughtCars, opt => opt.MapFrom(s => s.Sales.Count()))
            .ForMember(d => d.SpentMoney, opt => opt.MapFrom(s => s.Sales.Sum(x => x.Car.PartsCars.Sum(pc => pc.Part.Price))));
        }
    }
}
