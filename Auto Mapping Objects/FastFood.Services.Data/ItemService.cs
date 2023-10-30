using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Web.ViewModels.Items;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services.Data
{
    public class ItemService : IItemService
    {
        private readonly IMapper mapper;
        private readonly FastFoodContext context;

        public ItemService(IMapper mapper, FastFoodContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

        public async Task CreateAsync(CreateItemInputModel model)
        {
            Item item = mapper.Map<Item>(model);

            await context.Items.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ItemsAllViewModels>> GetAllAsync()
        => await context.Items
            .ProjectTo<ItemsAllViewModels>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        public async Task<IEnumerable<CreateItemViewModel>> GetAllAvailableCategoriesASync()
        => await context.Categories.ProjectTo<CreateItemViewModel>(mapper.ConfigurationProvider)
            .ToArrayAsync();
    }
}
