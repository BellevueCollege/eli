using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ELI.Models
{
    /**
     * Basic model for Quarter object
     * **/
    public class Quarter
    {
        [Column("YearQuarterId")]
        public string Id { get; set; }
        public DateTime FirstClassDay { get; set; }
        public DateTime LastClassDay { get; set; }
        public string Title { get; set; }
        public string AcademicYear { get; set; }
    }
}
