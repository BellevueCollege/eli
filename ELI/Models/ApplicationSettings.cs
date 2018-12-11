using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELI.Models
{
    public class ApplicationSettings
    {
        public static string SectionName = "ApplicationSettings";
        public string AuthorizedGroups { get; set; }
        public string SessionKey_SelectedQuarter { get; set; }
        public string MaxQuarter { get; set; }
    }
}
