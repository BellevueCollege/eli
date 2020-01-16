using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class Levels
    {
        [Key]
        public string Sid { get; set; }
        public string YearQuarterID { get; set; }
        public int? WriteLevel { get; set; }
        public string WriteGrade { get; set; }
        public int? WritePlace { get; set; }
        public int? ReadLevel { get; set; }
        public string ReadGrade { get; set; }
        public int? ReadPlace { get; set; }
        public int? SpeakLevel { get; set; }
        public string SpeakGrade { get; set; }
        public int? SpeakPlace { get; set; }
    }
}
