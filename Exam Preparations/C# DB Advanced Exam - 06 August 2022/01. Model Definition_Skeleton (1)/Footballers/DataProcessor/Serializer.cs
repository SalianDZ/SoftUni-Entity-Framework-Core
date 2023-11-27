using Footballers.Data;
using Footballers.Data.Models.Enums;
using Footballers.DataProcessor.ExportDto;
using Footballers.Utilities;
using Microsoft.Data.SqlClient.Server;
using Newtonsoft.Json;
using System.Globalization;

namespace Footballers.DataProcessor
{
    public class Serializer
    {
        public static string ExportCoachesWithTheirFootballers(FootballersContext context)
        {
            var coaches = context.Coaches
            .Where(c => c.Footballers.Count > 0)
            .OrderByDescending(c => c.Footballers.Count).ThenBy(c => c.Name)
            .Select(c => new ExportCoachDto
            {
                FootballersCount = c.Footballers.Count,
                Name = c.Name,
                Footballers = c.Footballers.Select(f => new ExportFootballersDto
                {
                    FootbalerName = f.Name,
                    PositionType = f.PositionType.ToString()
                })
                .OrderBy(f => f.FootbalerName)
                .ToArray()
            })
            .ToArray();

            XmlHelper helper = new();

            return helper.Serialize(coaches, "Coaches");
        }

        public static string ExportTeamsWithMostFootballers(FootballersContext context, DateTime date)
        {
            var teams = context.Teams
                .Where(t => t.TeamsFootballers.Any(tf => tf.Footballer.ContractStartDate >= date))
                .OrderByDescending(t => t.TeamsFootballers.Where(tf => tf.Footballer.ContractStartDate >= date).Count())
                .ThenBy(t => t.Name)
                .ToArray()
                .Select(t => new
                {
                    t.Name,
                    Footballers = t.TeamsFootballers.Where(tf => tf.Footballer.ContractStartDate >= date)
                        .OrderByDescending(f => f.Footballer.ContractEndDate).ThenBy(f => f.Footballer.Name)
                        .ToArray()
                        .Select(f => new
                        {
                            FootballerName = f.Footballer.Name,
                            ContractStartDate = f.Footballer.ContractStartDate.ToString("d", CultureInfo.InvariantCulture),
                            ContractEndDate = f.Footballer.ContractEndDate.ToString("d", CultureInfo.InvariantCulture),
                            BestSkillType = f.Footballer.BestSkillType.ToString(),
                            PositionType = f.Footballer.PositionType.ToString()
                        })
                        .ToArray()
                })
                .Take(5)
                .ToArray();

            return JsonConvert.SerializeObject(teams, Formatting.Indented);
        }
    }
}
