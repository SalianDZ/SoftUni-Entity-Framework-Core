namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Footballers.Utilities;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            XmlHelper helper = new();

            ImportCoachDto[] coachDtos = helper.Deserialize<ImportCoachDto[]>(xmlString, "Coaches");

            StringBuilder sb = new();

            ICollection<Coach> validCoaches = new HashSet<Coach>();

            foreach (var coachDto in coachDtos)
            {
                if (!IsValid(coachDto))
                {
                    sb.Append(ErrorMessage);
                    continue;
                }

                bool isNationalityInvalid = string.IsNullOrEmpty(coachDto.Nationality);

                if (isNationalityInvalid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Coach coach = new Coach()
                {
                    Name = coachDto.Name,
                    Nationality = coachDto.Nationality
                };

                foreach (var footballerDto in coachDto.Footballers)
                {
                    if (!IsValid(footballerDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime footballerContractStartDate;
                    bool isFootballerContractStartDateValid = DateTime.TryParseExact(footballerDto.ContractStartDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out footballerContractStartDate);
                    if (!isFootballerContractStartDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    DateTime footballerContractEndDate;
                    bool isFootballerContractEndDateValid = DateTime.TryParseExact(footballerDto.ContractEndDate,
                        "dd/MM/yyyy", CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out footballerContractEndDate);
                    if (!isFootballerContractEndDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (DateTime.Parse(footballerDto.ContractStartDate) >= DateTime.Parse(footballerDto.ContractEndDate))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer footballer = new Footballer()
                    {
                        Name = footballerDto.Name,
                        ContractStartDate = footballerContractStartDate,
                        ContractEndDate = footballerContractEndDate,
                        BestSkillType = (BestSkillType)footballerDto.BestSkillType,
                        PositionType = (PositionType)footballerDto.PositionType
                    };

                    coach.Footballers.Add(footballer);
                }
                sb.AppendLine(String.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));
                validCoaches.Add(coach);
            }

            context.Coaches.AddRange(validCoaches);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {
            ImportTeamDto[] teamDtos = JsonConvert.DeserializeObject<ImportTeamDto[]>(jsonString);

            StringBuilder sb = new();
            ICollection<Team> validTeams = new HashSet<Team>();

            foreach (var teamDto in teamDtos)
            {
                if (!IsValid(teamDto) || teamDto.Trophies <= 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string pattern = @"^[a-zA-Z0-9 .-]+$";
                if (!Regex.IsMatch(teamDto.Name, pattern))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Team team = new Team()
                {
                    Name = teamDto.Name,
                    Nationality = teamDto.Nationality,
                    Trophies = teamDto.Trophies
                };

                foreach (var footballerId in teamDto.Footballers)
                {
                    Footballer footballer = context.Footballers.Find(footballerId);

                    if (footballer == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    team.TeamsFootballers.Add(new TeamFootballer()
                    {
                        FootballerId = footballerId
                    });
                }

                validTeams.Add(team);
                sb.AppendLine(String.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));

            }

            context.Teams.AddRange(validTeams);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
