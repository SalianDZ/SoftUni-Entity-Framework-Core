namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using System.Text.RegularExpressions;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.DataProcessor.ImportDto;
    using Boardgames.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper helper = new();

            ImportCreatorsDto[] creatorsDtos = helper.Deserialize<ImportCreatorsDto[]>(xmlString, "Creators");

            StringBuilder sb = new();
            ICollection<Creator> validCreators = new HashSet<Creator>();

            foreach (var creatorDto in creatorsDtos)
            {
                if (!IsValid(creatorDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Creator creator = new Creator()
                {
                    FirstName = creatorDto.FirstName,
                    LastName = creatorDto.LastName,
                };

                foreach (var boardgameDto in creatorDto.BoardGames)
                {
                    if (!IsValid(boardgameDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Boardgame boardgame = new Boardgame()
                    {
                        Name = boardgameDto.Name,
                        Rating = boardgameDto.Rating,
                        YearPublished = boardgameDto.YearPublished,
                        CategoryType = boardgameDto.CategoryType,
                        Mechanics = boardgameDto.Mechanics
                    };

                    creator.Boardgames.Add(boardgame);
                }

                validCreators.Add(creator);
                sb.AppendLine(String.Format(SuccessfullyImportedCreator, creator.FirstName, creator.LastName, creator.Boardgames.Count));
            }

            context.Creators.AddRange(validCreators);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {
            ImportSellersDto[] sellerDtos = JsonConvert.DeserializeObject<ImportSellersDto[]>(jsonString);

            ICollection<Seller> validSellers = new HashSet<Seller>();
            StringBuilder sb = new();
            foreach (var sellerDto in sellerDtos)
            {
                if (!IsValid(sellerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string pattern = @"^www\.[a-zA-Z0-9-]+\.com$";

                if (!Regex.IsMatch(sellerDto.Website, pattern))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller seller = new Seller()
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Country = sellerDto.Country,
                    Website = sellerDto.Website,
                };

                foreach (var boardgameDto in sellerDto.Boardgames.Distinct())
                {
                    Boardgame boardgame = context.Boardgames.Find(boardgameDto);
                    if (boardgame == null || !IsValid(boardgameDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    seller.BoardgamesSellers.Add(new BoardgameSeller()
                    {
                        BoardgameId = boardgame.Id
                    });
                }

                validSellers.Add(seller);
                sb.AppendLine(String.Format(SuccessfullyImportedSeller, seller.Name, seller.BoardgamesSellers.Count()));
            }

            context.Sellers.AddRange(validSellers);
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
