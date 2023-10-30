using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Web.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Services.Data
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper mapper;
        private readonly FastFoodContext context;

        public CategoryService(IMapper mapper, FastFoodContext context)
        {
            this.mapper = mapper;
            this.context = context;
        }

       public async Task CreateASync(CreateCategoryInputModel model)
        {
            Category category = mapper.Map<Category>(model);

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryAllViewModel>> GetAllAsync()
            => await context.Categories
            .ProjectTo<CategoryAllViewModel>(mapper.ConfigurationProvider)
            .ToArrayAsync();
    }
}
