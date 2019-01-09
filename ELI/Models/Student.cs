using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class Student
    {
        public string Sid { get; private set; }
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
        public DateTime ModifyDate { get; private set; }
    }
}
