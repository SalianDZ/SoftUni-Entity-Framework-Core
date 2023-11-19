namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ExportDto;
    using Invoices.Utilities;
    using Newtonsoft.Json;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            XmlHelper helper = new XmlHelper();
            ExportClientWithTheirInvoicesDto[] clients = context.Clients
                .Where(c => c.Invoices.Any(I => I.IssueDate > date))
                .OrderByDescending(c => c.Invoices.Count())
                .ThenBy(c => c.Name)
                .Select(c => new ExportClientWithTheirInvoicesDto()
                {
                    Name = c.Name,
                    NumberVat = c.NumberVat,
                    InvoicesCount = c.Invoices.Count(),
                    Invoices = c.Invoices.OrderBy(i => i.IssueDate).ThenByDescending(i => i.DueDate)
                    .Select(i => new ExportInvoiceInClientDto()
                    {
                        Number = i.Number,
                        Amount = i.Amount,
                        DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        CurrencyType = i.CurrencyType,
                    }).ToArray()
                })
                .ToArray();

            return helper.Serialize<ExportClientWithTheirInvoicesDto[]>(clients, "Clients");
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            var products = context.Products
                .Where(p => p.ProductsClients.Any(pc => pc.Client != null && pc.Client.Name.Length >= nameLength))
                .OrderByDescending(p => p.ProductsClients.Where(pc => pc.Client != null && pc.Client.Name.Length >= nameLength).Count())
                .ThenBy(p => p.Name)
                .Select(p => new
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients.Where(pc => pc.Client != null && pc.Client.Name.Length >= nameLength)
                    .OrderBy(c => c.Client.Name)
                    .Select(pc => new
                    {
                        Name = pc.Client.Name,
                        NumberVat = pc.Client.NumberVat
                    }).ToArray()
                })
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
    }
}