
using AutoMapper;
using FastFood.Models;
using FastFood.Web.ViewModels.Categories;
using FastFood.Web.ViewModels.Items;
using FastFood.Web.ViewModels.Positions;

namespace FastFood.Services.Mapping;
public class FastFoodProfile : Profile
{
    public FastFoodProfile()
    {
        //Positions
        CreateMap<CreatePositionInputModel, Position>()
            .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

        CreateMap<Position, PositionsAllViewModel>()
            .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

        CreateMap<CreateCategoryInputModel, Category>()
            .ForMember(x => x.Name, y => y.MapFrom(x => x.CategoryName));

        CreateMap<Category, CategoryAllViewModel>();

        CreateMap<Category, CreateItemViewModel>()
            .ForMember(d => d.CategoryId, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(src => src.Name));

        CreateMap<CreateItemInputModel, Item>();
        CreateMap<Item, ItemsAllViewModels>()
        .ForMember(d => d.Category, opt => opt.MapFrom(s => s.Category.Name));
    }
}
 