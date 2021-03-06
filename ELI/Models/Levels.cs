﻿using System;
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
        public string Sid { get; set; }
        public int? ReadLevel { get; set; }
        public string ReadGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? ReadGradePoint { get; set; }
        [Range(1, 5)]
        public int? ReadPlace { get; set; }
        public int? WriteLevel { get; set; }
        public string WriteGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? WriteGradePoint { get; set; }
        [Range(1, 5)]
        public int? WritePlace { get; set; }
        public int? SpeakLevel { get; set; }
        public string SpeakGrade { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? SpeakGradePoint { get; set; }
        [Range(1, 5)]
        public int? SpeakPlace { get; set; }


        // custom comparator for comparing two scores for equality
        public bool IsEqualTo(Levels s)
        {
            if (ReadPlace == s.ReadPlace && WritePlace == s.WritePlace && SpeakPlace == s.SpeakPlace)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
