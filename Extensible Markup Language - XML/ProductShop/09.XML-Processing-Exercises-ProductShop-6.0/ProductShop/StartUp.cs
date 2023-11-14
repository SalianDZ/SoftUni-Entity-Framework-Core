using AutoMapper;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.ComponentModel;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();
            string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            string result = ImportCategoryProducts(context, inputXml);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new XmlHelper();

            ImportUserDto[] userDtos = helper.Deserialize<ImportUserDto[]>(inputXml, "Users");

            ICollection<User> validUsers = new HashSet<User>();

            foreach (var userDto in userDtos)
            {
                User user = mapper.Map<User>(userDto);
                validUsers.Add(user);
            }

            context.Users.AddRange(validUsers);
            context.SaveChanges();
            return $"Successfully imported {validUsers.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new XmlHelper();

            ImportProductDto[] productDtos = helper.Deserialize<ImportProductDto[]>(inputXml, "Products");
            ICollection<Product> validProducts = new HashSet<Product>();

            foreach (var productDto in productDtos)
            {
                Product product = mapper.Map<Product>(productDto);
                validProducts.Add(product);
            }
            context.Products.AddRange(validProducts);
            context.SaveChanges();
            return $"Successfully imported {validProducts.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();
            ImportCategoryDto[] categoryDtos = helper.Deserialize<ImportCategoryDto[]>(inputXml, "Categories");

            ICollection<Category> validCategories = new HashSet<Category>();

            foreach (var categoryDto in categoryDtos)
            {
                if (string.IsNullOrEmpty(categoryDto.Name))
                {
                    continue;
                }
                Category category = mapper.Map<Category>(categoryDto);
                validCategories.Add(category);
            }
            context.Categories.AddRange(validCategories);
            context.SaveChanges();
            return $"Successfully imported {validCategories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();
            ImportCategoryProductDto[] categoryProductDtos = helper.Deserialize<ImportCategoryProductDto[]>(inputXml, "CategoryProducts");
            ICollection<CategoryProduct> validCategoryProducts = new HashSet<CategoryProduct>();

            foreach (var categoryProductDto in categoryProductDtos)
            {
                if (!context.Categories.Any(c => c.Id == categoryProductDto.CategoryId) || !context.Products.Any(p => p.Id == categoryProductDto.ProductId))
                {
                    continue;
                }
                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryProductDto);
                validCategoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(validCategoryProducts);
            context.SaveChanges();
            return $"Successfully imported {validCategoryProducts.Count}";
        }

        private static IMapper InitializeAutoMapper()
            => new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
    }
}