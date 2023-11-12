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

        }
    }
}
