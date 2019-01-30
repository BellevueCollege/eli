using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ELI.Models;

namespace ELI.Pages
{
    public class StudentDetailModel : PageModel
    {
        private readonly ELIContext _context;

        public StudentDetailModel(ELIContext context)
        {
            _context = context;
        }

        public string StudentID { get; set; }
        public Student Student { get; set; }
        public IList<StudentClassDetail> StudentClassDetails { get; set; }

        public void OnGet(string id)
        {
            if ( id != null )
            {
                StudentID = id;
                Student = _context.Students.Include(s => s.Score).Where(s => s.Sid == StudentID).Single();
                StudentClassDetails = _context.StudentClassDetails.Where(s => s.Sid == StudentID).OrderByDescending(s => s.YearQuarterID).ThenBy(s => s.ItemNumber).ToList();
            }

        }
    }
}