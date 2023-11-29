using Artillery.Data;
using Artillery.Data.Models;
using Artillery.Data.Models.Enums;
using Artillery.DataProcessor.ImportDto;
using Castle.Components.DictionaryAdapter;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor
{
    public class Deserializer
    {
        private const string ErrorMessage =
            "Invalid data.";
        private const string SuccessfulImportCountry =
            "Successfully import {0} with {1} army personnel.";
        private const string SuccessfulImportManufacturer =
            "Successfully import manufacturer {0} founded in {1}.";
        private const string SuccessfulImportShell =
            "Successfully import shell caliber #{0} weight {1} kg.";
        private const string SuccessfulImportGun =
            "Successfully import gun {0} with a total weight of {1} kg. and barrel length of {2} m.";

        public static string ImportCountries(ArtilleryContext context, string xmlString)
        {

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Countries");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportCountriesDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);
            ImportCountriesDto[] deserializedDtos =
                (ImportCountriesDto[])xmlSerializer.Deserialize(reader);


            ICollection<Country> validCountries = new HashSet<Country>();
            StringBuilder sb = new();

            foreach (var countryDto in deserializedDtos)
            {
                if (!IsValid(countryDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Country country = new Country() 
                {
                    CountryName = countryDto.CountryName,
                    ArmySize = countryDto.ArmySize
                };

                validCountries.Add(country);
                sb.AppendLine(String.Format(SuccessfulImportCountry, country.CountryName, country.ArmySize));
            }

            context.Countries.AddRange(validCountries);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportManufacturers(ArtilleryContext context, string xmlString)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Manufacturers");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportManufacturersDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);
            ImportManufacturersDto[] deserializedDtos =
                (ImportManufacturersDto[])xmlSerializer.Deserialize(reader);

            ICollection<Manufacturer> validManufacturers = new HashSet<Manufacturer>();
            StringBuilder sb = new();

            foreach (var manDto in deserializedDtos)
            {
                
                if (!IsValid(manDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (validManufacturers.Any(x => x.ManufacturerName == manDto.ManufacturerName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Manufacturer manufacturer = new Manufacturer()
                {
                    ManufacturerName = manDto.ManufacturerName,
                    Founded = manDto.Founded
                };

                string[] location = manufacturer.Founded.Split(", ").ToArray();

                validManufacturers.Add(manufacturer);
                sb.AppendLine(String.Format(SuccessfulImportManufacturer, manufacturer.ManufacturerName, $"{location[location.Length - 2]}, {location[location.Length - 1]}"));

            }

            context.Manufacturers.AddRange(validManufacturers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportShells(ArtilleryContext context, string xmlString)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Shells");
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportShellsDto[]), xmlRoot);

            using StringReader reader = new StringReader(xmlString);
            ImportShellsDto[] deserializedDtos =
                (ImportShellsDto[])xmlSerializer.Deserialize(reader);

            ICollection<Shell> validShells = new HashSet<Shell>();
            StringBuilder sb = new();

            foreach (var shellDto in deserializedDtos)
            {
                if (!IsValid(shellDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Shell shell = new Shell()
                {
                    ShellWeight = shellDto.ShellWeight,
                    Caliber = shellDto.Caliber
                };

                sb.AppendLine(String.Format(SuccessfulImportShell, shell.Caliber, shell.ShellWeight));
                validShells.Add(shell);
            }   

            context.Shells.AddRange(validShells);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportGuns(ArtilleryContext context, string jsonString)
        {
            ImportGunsDto[] guns = JsonConvert.DeserializeObject<ImportGunsDto[]>(jsonString);

            ICollection<Gun> validGuns = new HashSet<Gun>();
            StringBuilder sb = new();
            foreach (var gunDto in guns)
            {
                if (!IsValid(gunDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!Enum.IsDefined(typeof(GunType), gunDto.GunType))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Gun gun = new Gun()
                {
                    ManufacturerId = gunDto.ManufacturerId,
                    GunWeight = gunDto.GunWeight,
                    BarrelLength = gunDto.BarrelLength,
                    NumberBuild = gunDto.NumberBuild,
                    Range = gunDto.Range,
                    GunType = (GunType) Enum.Parse(typeof(GunType), gunDto.GunType),
                    ShellId = gunDto.ShellId
                };

                foreach (var country in gunDto.Countries)
                {

                    gun.CountriesGuns.Add(new CountryGun()
                    {
                        CountryId = country.Id
                    });
                }

                sb.AppendLine(String.Format(SuccessfulImportGun, gun.GunType, gun.GunWeight, gun.BarrelLength));
                validGuns.Add(gun);
            }

            context.Guns.AddRange(validGuns);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}