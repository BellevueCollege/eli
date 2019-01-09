using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ELI.Models;

namespace ELI.Pages
{
    public class StudentSearchModel : EliPageModel
    {
        //private readonly ELIContext _context;

        public StudentSearchModel(ELIContext context, IConfiguration config, ILogger<StudentSearchModel> logger) : base(context, config, logger){}

        public IList<Student> StudentData { get; set; }
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

        public string SidSort { get; set; }
        public string LnameSort { get; set; }
        public string FnameSort { get; set; }
        public string GroupSort { get; set; }
        public string CountrySort { get; set; }
        public string QuarterSort { get; set; }

        public string SortDirSid { get; set; }
        public string SortDirLname { get; set; }
        public string SortDirFname { get; set; }
        public string SortDirCountry { get; set; }
        public string SortDirGroup { get; set; }
        public string SortDirQuarter {get; set;}

        public void OnGet(string sortType)
        {
            //_context.Database.SetCommandTimeout(200);
            //StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getStudentViewData").ToList();
            //StudentData = _context.StudentSearchResults.FromSql("EXEC dbo.usp_getFakeStudentView").OrderByDescending(s => s.ProjectedQuarter).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            StudentData = _context.Students.OrderByDescending(s => s.YearQuarterEnrolled).ThenBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            /***
             * get groups, countries, and quarters to fill filter drop downs
             * need to do before student data is filtered
            ***/
            var g = StudentData.Select(s => s.Group).Distinct();
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

            var q = StudentData.Select(s => s.YearQuarterEnrolled).Distinct();
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
                StudentData = StudentData.Where(s => s.Group.Equals(GroupSearch)).ToList();
            }
            if (!String.IsNullOrEmpty(CountrySearch))
            {
                StudentData = StudentData.Where(s => s.Country.Equals(CountrySearch)).ToList();
            }
            if (!String.IsNullOrEmpty(QuarterSearch))
            {
                StudentData = StudentData.Where(s => s.YearQuarterEnrolled.Equals(QuarterSearch)).ToList();
            }

            /***
             * Now sort data per any inputs 
            ***/
            FnameSort = "fname";
            LnameSort = "lname";
            SidSort = "sid";
            GroupSort = "group";
            CountrySort = "country";
            QuarterSort = "quarter";
            SortDirSid = SortDirFname = SortDirLname = SortDirGroup = SortDirCountry = SortDirQuarter = "bottom";
            if ( !String.IsNullOrEmpty(sortType) )
            {
                switch(sortType)
                {
                    case "lname":
                        StudentData = StudentData.OrderBy(s => s.LastName).ToList();
                        LnameSort = "lname_desc";
                        break;
                    case "lname_desc":
                        StudentData = StudentData.OrderByDescending(s => s.LastName).ToList();
                        LnameSort = "lname";
                        SortDirLname = "top";
                        break;
                    case "fname":
                        StudentData = StudentData.OrderBy(s => s.FirstName).ToList();
                        FnameSort = "fname_desc";
                        break;
                    case "fname_desc":
                        StudentData = StudentData.OrderByDescending(s => s.FirstName).ToList();
                        FnameSort = "fname";
                        SortDirFname = "top";
                        break;
                    case "sid":
                        StudentData = StudentData.OrderBy(s => s.Sid).ToList();
                        SidSort = "sid_desc";
                        break;
                    case "sid_desc":
                        StudentData = StudentData.OrderByDescending(s => s.Sid).ToList();
                        SidSort = "sid";
                        SortDirSid = "top";
                        break;
                    case "group":
                        StudentData = StudentData.OrderBy(s => s.Group).ToList();
                        GroupSort = "group_desc";
                        break;
                    case "group_desc":
                        StudentData = StudentData.OrderByDescending(s => s.Group).ToList();
                        GroupSort = "group";
                        SortDirGroup = "top";
                        break;
                    case "country":
                        StudentData = StudentData.OrderBy(s => s.Country).ToList();
                        CountrySort = "country_desc";
                        break;
                    case "country_desc":
                        StudentData = StudentData.OrderByDescending(s => s.Country).ToList();
                        CountrySort = "country";
                        SortDirCountry = "top";
                        break;
                    case "quarter":
                        StudentData = StudentData.OrderBy(s => s.YearQuarterEnrolled).ToList();
                        QuarterSort = "quarter_desc";
                        break;
                    case "quarter_desc":
                        StudentData = StudentData.OrderByDescending(s => s.YearQuarterEnrolled).ToList();
                        QuarterSort = "quarter";
                        SortDirQuarter = "top";
                        break;
                }
            } else
            {
                FnameSort = "fname";
                LnameSort = "lname";
                SidSort = "sid";
                GroupSort = "group";
                CountrySort = "country";
                QuarterSort = "quarter";
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            //if (! string.IsNullOrEmpty(QuarterSelect)) _logger.LogDebug(QuarterSelect);

            SetSelectedQuarter();
            return RedirectToPage();
        }
    }
}