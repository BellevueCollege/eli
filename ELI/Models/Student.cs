using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class Student
    {
        public string Sid { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string Email { get; private set; }
        public string Dob { get; private set; }
        public string Sex { get; private set; }
        public string Program { get; private set; }
        public string ProjectedYRQ { get; private set; }
        public string Country { get; private set; }
        public string Visa { get; private set; }
        public string VisaObtained { get; private set; }
    }
}
