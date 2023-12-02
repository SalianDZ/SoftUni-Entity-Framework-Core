namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            XmlHelper helper = new();

            var clientDtos = helper.Deserialize<ImportClientsDto[]>(xmlString, "Clients");

            ICollection<Client> validClients = new HashSet<Client>();
            StringBuilder sb = new();

            foreach (var clientDto in clientDtos)
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat
                };

                foreach (var addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    { 
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country
                    };

                    client.Addresses.Add(address);
                }

                validClients.Add(client);
                sb.AppendLine(String.Format(SuccessfullyImportedClients, client.Name));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            ImportInvoicesDto[] invoiceDtos = JsonConvert.DeserializeObject < ImportInvoicesDto[]>(jsonString);

            ICollection<Invoice> validInvoices = new HashSet<Invoice>();
            StringBuilder sb = new StringBuilder();

            foreach (var invoiceDto in invoiceDtos)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (DateTime.Parse(invoiceDto.IssueDate) > DateTime.Parse(invoiceDto.DueDate))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }


                Invoice invoice = new Invoice() 
                {
                    Number = invoiceDto.Number,
                    IssueDate = DateTime.Parse(invoiceDto.IssueDate, CultureInfo.InvariantCulture),
                    DueDate = DateTime.Parse(invoiceDto.DueDate, CultureInfo.InvariantCulture),
                    Amount = invoiceDto.Amount,
                    CurrencyType = invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                validInvoices.Add(invoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            ImportProductsDto[] productDtos =
                JsonConvert.DeserializeObject<ImportProductsDto[]>(jsonString);

            ICollection<Product> validProducts = new HashSet<Product>();
            StringBuilder sb = new StringBuilder();

            foreach (var productDto in productDtos)
            {
                if (!IsValid(productDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product product = new Product() 
                {
                    Name = productDto.Name,
                    Price  = productDto.Price,
                    CategoryType = productDto.CategoryType
                };

                foreach (var clientId in productDto.Clients)
                {
                    if (!context.Clients.Any(c => c.Id == clientId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    product.ProductsClients.Add(new ProductClient()
                    {
                        ClientId = clientId,
                    });
                }

                validProducts.Add(product);
                sb.AppendLine(String.Format(SuccessfullyImportedProducts, product.Name, product.ProductsClients.Count()));
            }

            context.Products.AddRange(validProducts);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
