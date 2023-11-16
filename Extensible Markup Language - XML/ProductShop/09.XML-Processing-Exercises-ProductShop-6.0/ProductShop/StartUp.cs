using AutoMapper;
using AutoMapper.QueryableExtensions;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();
            //string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            string result = GetUsersWithProducts(context);
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

        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new();

            ExportProductsInRangeDto[] products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .Take(10)
                .ToArray();

            return helper.Serialize<ExportProductsInRangeDto[]>(products, "Products");
        }

        //!!!!Not working due to changed DTO for other exercise
        //public static string GetSoldProducts(ProductShopContext context)
        //{
        //    XmlHelper helper = new();

        //    ExportUsersWithSoldProductsDto[] users = context.Users
        //        .Where(u => u.ProductsSold.Count >= 1)
        //        .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
        //        .Take(5)
        //        .Select(u => new ExportUsersWithSoldProductsDto()
        //        {
        //            FirstName = u.FirstName,
        //            LastName = u.LastName,
        //            SoldProducts = u.ProductsSold.Select(ps => new ExportProductDto()
        //            {
        //                Name = ps.Name,
        //                Price = ps.Price
        //            }).ToArray()
        //        }).ToArray();
        //    return helper.Serialize<ExportUsersWithSoldProductsDto[]>(users, "Users");
        //}

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            XmlHelper hellper = new();

            ExportCategoriesByProductDto[] products = context.Categories
                .Select(c => new ExportCategoriesByProductDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count(),
                    AveragePrice = c.CategoryProducts.Select(cp => cp.Product).Average(cp => cp.Price),
                    TotalRevenue = c.CategoryProducts.Select(cp => cp.Product).Sum(cp => cp.Price)
                })
                .OrderByDescending(p => p.Count).ThenBy(p => p.TotalRevenue)
                .ToArray();

           return hellper.Serialize<ExportCategoriesByProductDto[]>(products, "Categories");
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            XmlHelper helper = new();

            UserInfoOutputModel[] userProducts = context.Users
                .Where(u => u.ProductsSold.Count >= 1)
                .OrderByDescending(u => u.ProductsSold.Count())
                .Select(u => new UserInfoOutputModel()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductInfoOutputModel()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(ps => new ExportProductDto()
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            UserProductOutputModel userProductsDto = new UserProductOutputModel()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = userProducts
            };

            return helper.Serialize<UserProductOutputModel>(userProductsDto, "Users");
        }

        private static IMapper InitializeAutoMapper()
            => new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
    }
}