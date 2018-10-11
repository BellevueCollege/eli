using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public SelectList SelectGroups { get; set; }
        public SelectList SelectCountries { get; set; }
        public SelectList SelectQuarters { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public string LnameSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FnameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SidSearch { get; set; }

        [BindProperty(SupportsGet = true)] public string GroupSearch { get; set; }
        [BindProperty(SupportsGet = true)] public string CountrySearch { get; set; }
        [BindProperty(SupportsGet = true)] public string QuarterSearch { get; set; }

        public void OnGet()
        {
            _context.Database.SetCommandTimeout(200);
            //StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getStudentViewData").ToList();
            StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getFakeStudentView").OrderByDescending(s => s.ProjectedQuarter).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();

            /***
             * get groups, countries, and quarters to fill filter drop downs
             * need to do before student data is filtered
            ***/
            var g = StudentData.Select(s => s.Program).Distinct();
            if (!String.IsNullOrEmpty(GroupSearch))
            {
                SelectGroups = new SelectList(g, GroupSearch);
            }
            else SelectGroups = new SelectList(g);

            var c  = StudentData.Select(s => s.Country).Distinct();
            if (!String.IsNullOrEmpty(CountrySearch))
            {
                SelectCountries = new SelectList(c, CountrySearch);
            }
            else SelectCountries = new SelectList(c);

            var q = StudentData.Select(s => s.ProjectedQuarter).Distinct();
            if (!String.IsNullOrEmpty(QuarterSearch))
            {
                SelectQuarters = new SelectList(q, QuarterSearch);
            }
            else SelectQuarters = new SelectList(q);
            /*** end generate drop down filters ***/

            /***
             * Based on input search filters, filter student data
            ***/
            if ( !String.IsNullOrEmpty(LnameSearch) )
            {
                StudentData = StudentData.Where(s => s.LastName.ToLower().StartsWith(LnameSearch.ToLower())).ToList();
            }
            if( !String.IsNullOrEmpty(SidSearch))
            {
                StudentData = StudentData.Where(s => s.Sid.StartsWith(SidSearch)).ToList();
            }
            if (!String.IsNullOrEmpty(FnameSearch))
            {
                StudentData = StudentData.Where(s => s.FirstName.ToLower().StartsWith(FnameSearch.ToLower())).ToList();
            }
            if (!String.IsNullOrEmpty(GroupSearch))
            {
                StudentData = StudentData.Where(s => s.Program.Equals(GroupSearch)).ToList();
            }
            if (!String.IsNullOrEmpty(CountrySearch))
            {
                StudentData = StudentData.Where(s => s.Country.Equals(CountrySearch)).ToList();
            }
            if (!String.IsNullOrEmpty(QuarterSearch))
            {
                StudentData = StudentData.Where(s => s.ProjectedQuarter.Equals(QuarterSearch)).ToList();
            }
        }
    }
}