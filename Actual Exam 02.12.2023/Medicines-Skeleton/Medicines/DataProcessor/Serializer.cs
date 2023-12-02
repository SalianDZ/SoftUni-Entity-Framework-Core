namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using Medicines.DataProcessor.ExportDtos;
    using Medicines.Utilities;
    using Newtonsoft.Json;
    using System.Globalization;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            var patients = context.Patients
                .Where(p => p.PatientsMedicines.Count >= 1 && p.PatientsMedicines.Any(pc => pc.Medicine.ProductionDate > DateTime.Parse(date)))
                .ToArray()
                .Select(p => new ExportPatientDto()
                {
                    Gender = p.Gender.ToString().ToLower(),
                    Name = p.FullName,
                    AgeGroup = p.AgeGroup.ToString(),
                    Medicines = p.PatientsMedicines.Where(pc => pc.Medicine.ProductionDate > DateTime.Parse(date) && pc.Medicine.PatientsMedicines.Count >= 1)
                    .ToArray()
                        .OrderByDescending(p => p.Medicine.ExpiryDate)
                        .ThenBy(p => p.Medicine.Price)
                        .Select(pm => new ExportMedicineDto()
                        {
                            Category = pm.Medicine.Category.ToString().ToLower(),
                            Name = pm.Medicine.Name,
                            Price = pm.Medicine.Price.ToString("F2"),
                            Producer = pm.Medicine.Producer,
                            BestBefore = pm.Medicine.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Count())
                .ThenBy(p => p.Name)
                .ToArray();

            XmlHelper helper = new();

            return helper.Serialize<ExportPatientDto[]>(patients, "Patients");
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context.Medicines
                .Where(m => m.Pharmacy.IsNonStop &&
                (int)m.Category == medicineCategory)
                .ToArray()
                .Select(m => new
                {
                    m.Name,
                    Price = m.Price.ToString("F2"),
                    Pharmacy = new ExportPharmacyDto()
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name)
                .ToArray();
                
            return JsonConvert.SerializeObject(medicines, Formatting.Indented);
        }
    }
}
