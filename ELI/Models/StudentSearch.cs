using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    /**
     * Basic model for StudentSearch object
     * **/
    public class StudentSearch
    {
        public string Sid { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string MiddleInitial { get; private set; }
        public string Program { get; private set; }
        public string ProjectedQuarter { get; private set; }
        public string Country { get; private set; }

    }
}