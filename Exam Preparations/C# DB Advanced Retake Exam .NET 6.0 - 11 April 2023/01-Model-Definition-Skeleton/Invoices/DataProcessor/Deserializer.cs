namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

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
            ImportClientsDto[] clientDtos =
                helper.Deserialize<ImportClientsDto[]>(xmlString, "Clients");

            ICollection<Client> validClients = new HashSet<Client>();

            StringBuilder sb = new();
            foreach (var clientDto in clientDtos) 
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);    
                    continue;
                }

                Client client = new()
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

                    Address address = new()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country,
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
            ImportInvoiceDto[] invoiceDtos =
                JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);

            StringBuilder sb = new();
            ICollection<Invoice> validInvoices = new HashSet<Invoice>();
            foreach (var invoiceDto in invoiceDtos)
            {
                if (!IsValid(invoiceDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (invoiceDto.DueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture)
                    || invoiceDto.IssueDate == DateTime.ParseExact("01/01/0001", "dd/MM/yyyy", CultureInfo.InvariantCulture))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new()
                {
                    Number = invoiceDto.Number,
                    IssueDate = invoiceDto.IssueDate,
                    DueDate = invoiceDto.DueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId,
                };

                if (invoice.IssueDate > invoice.DueDate)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                validInvoices.Add(invoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        //I should check this solution!!!
        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            ImportProductDto[] productDtos =
                JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString);

            StringBuilder sb = new();
            ICollection<Product> validProducts = new HashSet<Product>();
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
                    Price = productDto.Price,
                    CategoryType = productDto.CategoryType
                };

                foreach (var clientDto in productDto.Clients.Distinct())
                {
                    Client client = context.Clients.Find(clientDto);

                    if (client == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    product.ProductsClients.Add(new ProductClient()
                    {
                        Client = client,
                    });
                }

                validProducts.Add(product);
                sb.AppendLine(String.Format(SuccessfullyImportedProducts, product.Name, product.ProductsClients.Count));
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
