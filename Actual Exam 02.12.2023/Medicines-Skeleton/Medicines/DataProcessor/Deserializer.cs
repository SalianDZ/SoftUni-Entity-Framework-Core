namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.Data.Models;
    using Medicines.Data.Models.Enums;
    using Medicines.DataProcessor.ImportDtos;
    using Medicines.Utilities;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            ImportPatientDto[] patientDtos = JsonConvert.DeserializeObject<ImportPatientDto[]>(jsonString);

            StringBuilder sb = new StringBuilder();
            ICollection<Patient> validPatients = new HashSet<Patient>();

            foreach (var patientDto in patientDtos)
            {
                if (!IsValid(patientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (patientDto.AgeGroup < 0 && patientDto.AgeGroup > 2)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (patientDto.Gender < 0 && patientDto.Gender > 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Patient patient = new Patient()
                {
                    FullName = patientDto.FullName,
                    AgeGroup = (AgeGroup) patientDto.AgeGroup,
                    Gender = (Gender) patientDto.Gender
                };

                foreach (var medicineId in patientDto.Medicines)
                {
                    if (patient.PatientsMedicines.Any(x => x.MedicineId == medicineId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    patient.PatientsMedicines.Add(new PatientMedicine()
                    {
                        MedicineId = medicineId
                    });
                }

                validPatients.Add(patient);
                sb.AppendLine(String.Format(SuccessfullyImportedPatient, patient.FullName, patient.PatientsMedicines.Count()));
            }

            context.Patients.AddRange(validPatients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            XmlHelper helper = new();

            ImportPharmacyDto[] pharmacyDtos
                = helper.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            ICollection<Pharmacy> validPharmacies = new HashSet<Pharmacy>();    
            StringBuilder sb = new StringBuilder();

            foreach (var pharmacyDto in pharmacyDtos)
            {
                if (!IsValid(pharmacyDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                string pattern = @"^\(\d{3}\) \d{3}-\d{4}$";

                if (!Regex.IsMatch(pharmacyDto.PhoneNumber, pattern))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (pharmacyDto.IsNonStop.ToLower() != "true" && pharmacyDto.IsNonStop.ToLower() != "false")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Pharmacy pharmacy = new Pharmacy()
                {
                    Name = pharmacyDto.Name,
                    PhoneNumber = pharmacyDto.PhoneNumber,
                    IsNonStop = bool.Parse(pharmacyDto.IsNonStop)
                };

                foreach (var medicineDto in pharmacyDto.Medicines)
                {
                    if (!IsValid(medicineDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if ((int)medicineDto.Category < 0 && (int)medicineDto.Category > 4)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (DateTime.Parse(medicineDto.ProductionDate) >= DateTime.Parse(medicineDto.ExpiryDate))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (pharmacy.Medicines.Any(m => m.Name == medicineDto.Name && m.Producer == medicineDto.Producer))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Medicine medicine = new Medicine()
                    {
                        Category = (Category) Enum.ToObject(typeof(Category), medicineDto.Category),
                        Name = medicineDto.Name,
                        Price = medicineDto.Price,
                        ProductionDate = DateTime.ParseExact(medicineDto.ProductionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        ExpiryDate = DateTime.ParseExact(medicineDto.ExpiryDate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        Producer = medicineDto.Producer
                    };

                    pharmacy.Medicines.Add(medicine);
                }

                validPharmacies.Add(pharmacy);
                sb.AppendLine(String.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count()));
            }

            context.Pharmacies.AddRange(validPharmacies);
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
