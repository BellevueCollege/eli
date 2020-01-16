using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELI.Models
{
    /**
     * Basic model for Student object
     * **/
    public class Student
    {
        [Key]
        public string Sid { get; set; }
        public string StuType { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public DateTime Dob { get; private set; }
        public string Gender { get; private set; }
        public string Group { get; private set; }
        public string YearQuarterEnrolled { get; private set; }
        public bool IsEnrolled { get; private set; }
        public string Country { get; private set; }
        public string VisaStatus { get; private set; }
        public DateTime AddDate { get; private set; }
        public DateTime ModifyDate { get; set; }
        public Scores Score { get; set; }
        [ForeignKey("SID")]
        public Levels Level { get; set; }
    }    
}