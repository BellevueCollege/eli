using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class Levels
    {
        [Key]
        public int LevelID { get; set; }
        public string SID { get; set; }
        public int? ReadLevel { get; set; }
        public string ReadGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? ReadGradePoint { get; set; }
        public int? ReadPlace { get; set; }
        public int? WriteLevel { get; set; }
        public string WriteGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? WriteGradePoint { get; set; }
        public int? WritePlace { get; set; }
        public int? SpeakLevel { get; set; }
        public string SpeakGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? SpeakGradePoint { get; set; }
        public int? SpeakPlace { get; set; }
    }
}
