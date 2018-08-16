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
    public class StudentSearchModel : PageModel
    {
        private readonly ELIContext _context;

        public StudentSearchModel(ELIContext context)
        {
            _context = context;
        }

        public IList<StudentSearch> StudentData { get; set; }

        public void OnGet()
        {
            _context.Database.SetCommandTimeout(200);
            StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getFakeStudentView").ToList();
            //StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getStudentViewData").ToList();
        }
    }
}