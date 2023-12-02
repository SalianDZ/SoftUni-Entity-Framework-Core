namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.DataProcessor.ExportDto;
    using Invoices.Utilities;
    using Microsoft.Data.SqlClient.Server;
    using Microsoft.IdentityModel.Protocols.OpenIdConnect;
    using Newtonsoft.Json;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            XmlHelper helper = new XmlHelper();

            var clients = context.Clients
                .Where(c => c.Invoices.Any(i => i.IssueDate != null && i.IssueDate > date))
                .ToArray()
                .Select(i => new ExportClientDto()
                {
                    InvoicesCount = i.Invoices.Count(),
                    ClientName = i.Name,
                    VatNumber = i.NumberVat,
                    Invoices = i.Invoices
                        .ToArray()
                        .OrderBy(i => i.IssueDate)
                        .ThenByDescending(i => i.DueDate)
                        .Select(i => new ExportInvoicesInClientDto()
                        {
                            InvoiceNumber = i.Number,
                            InvoiceAmount = i.Amount,
                            DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture),
                            Currency = i.CurrencyType.ToString()
                        })
                        .ToArray()
                })
                .OrderByDescending(i => i.Invoices.Count())
                .ThenBy(i => i.ClientName)
                .ToArray();

            return helper.Serialize<ExportClientDto[]>(clients, "Clients");
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                .Where(p => p.ProductsClients
                    .Any(pc => pc.Client.ProductsClients.Count >= 1 &&
                            pc.Client.Name.Length >= nameLength))
                .ToArray()
                .Select(p => new
                {
                    p.Name,
                    p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients.Where(pc => pc.Client.ProductsClients.Count >= 1 &&
                            pc.Client.Name.Length >= nameLength)
                        .Select(pc => new
                        {
                            pc.Client.Name,
                            pc.Client.NumberVat
                        })
                        .OrderBy(pc => pc.Name)
                        .ToArray()
                })
                .OrderByDescending(p => p.Clients.Count())
                .ThenBy(p => p.Name)
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}