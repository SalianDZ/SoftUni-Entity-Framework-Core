﻿using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Manufacturer")]
    public class ImportManufacturersDto
    {
        [Required]
        [MinLength(4)]
        [MaxLength(40)]
        [XmlElement("ManufacturerName")]
        public string ManufacturerName { get; set; } = null!;

        [Required]
        [MinLength(10)]
        [MaxLength(100)]
        [XmlElement("Founded")]
        public string Founded { get; set; } = null!;
    }
}
