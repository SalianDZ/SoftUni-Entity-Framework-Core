using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Data;
using System.Xml.XPath;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new();
            //string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");
            string result = GetCategoriesByProductsCount(context);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            UserDto[] usersDto = JsonConvert.DeserializeObject<UserDto[]>(inputJson);

            User[] users = mapper.Map<UserDto[], User[]>(usersDto);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            ProductDto[] productsDto = JsonConvert.DeserializeObject<ProductDto[]>(inputJson);

            Product[] products = mapper.Map<Product[]>(productsDto);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            CategoryDto[] categoriesDto = JsonConvert.DeserializeObject<CategoryDto[]>(inputJson);

            foreach (var categorydto in categoriesDto)
            {
                Category category = mapper.Map<Category>(categorydto);

                if (category.Name != null)
                {
                    context.Categories.Add(category);
                }
            }

            context.SaveChanges();
            return $"Successfully imported {context.Categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            CategoryProductDto[] categoriesProductDto = JsonConvert.DeserializeObject<CategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> validEntries = new HashSet<CategoryProduct>();

            foreach (var cpdto in categoriesProductDto)
            {
                //if (!context.Categories.Any(c => c.Id == cpdto.CategoryId) ||
                //    !context.Products.Any(p => p.Id == cpdto.ProductId))
                //{
                //    continue;
                //}

                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(cpdto);
                validEntries.Add(categoryProduct);
            }

            context.CategoriesProducts.AddRange(validEntries);
            context.SaveChanges();

            return $"Successfully imported {validEntries.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName,
                })
                .AsNoTracking()
                .ToList();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithSoldProducts = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold.Where(ps => ps.Buyer != null)
                    .Select(p => new
                    { 
                        name = p.Name,
                        price = p.Price,
                        buyerFirstName = p.Buyer.FirstName,
                        buyerLastName = p.Buyer.LastName
                    }).ToArray()
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(usersWithSoldProducts, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count())
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count(),
                    averagePrice = $"{c.CategoriesProducts.Average(cp => cp.Product.Price):f2}",
                    totalRevenue = $"{c.CategoriesProducts.Sum(cp => cp.Product.Price)}"
                })
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        private static IContractResolver ConfigureCamelCaseNaming()
        {
            return new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };
        }
    }
}