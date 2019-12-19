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
    /** Allows one to search and filter students
     * Extends ELIPageModel to provide basic page 
     * functionality 
     * **/
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
            /** Use IQueryable so additional conditionals can be added before converting to a 
             * collection (at which time the query goes to the db)
             * **/
            IQueryable<Student> StudentsIQ = _context.Students;
            /****************NOTE: 
             * .OrderByDescending(s => s.YearQuarterEnrolled).ThenBy(s => s.LastName).ThenBy(s => s.FirstName);
             * was intially on the end of the line above(IQueryable<Student> StudentsIQ = _context.Students)
             * HOWEVER, this was causing the last name sorting issue described in JIRA bug: ER-83 where sorting by lastname requires user to double click to begin sorting functionality.
             * More research needs to be done to figure out why this snid but of code breaks the lastname sort functionality. 
           ********************/

            /***
             * get groups, countries, and quarters to fill filter drop downs
             * need to do before student data is filtered
            ***/
            //save to separate IQueryable so conditionals can later be used against original before query is sent to database
            IQueryable<Student> StudentsIQForGrouping = StudentsIQ; 
            var g = StudentsIQForGrouping.Select(s => s.Group).Distinct();
            if (!String.IsNullOrEmpty(GroupSearch))
            {
                SelectGroups = new SelectList(g, GroupSearch);
            }
            else SelectGroups = new SelectList(g);

            var c  = StudentsIQForGrouping.Select(s => s.Country).Distinct();
            if (!String.IsNullOrEmpty(CountrySearch))
            {
                SelectCountries = new SelectList(c, CountrySearch);
            }
            else SelectCountries = new SelectList(c);

            var q = StudentsIQForGrouping.Select(s => s.YearQuarterEnrolled).Distinct();
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
                StudentsIQ = StudentsIQ.Where(s => s.LastName.ToLower().StartsWith(LnameSearch.ToLower()));
            }
            if( !String.IsNullOrEmpty(SidSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.Sid.StartsWith(SidSearch));
            }
            if (!String.IsNullOrEmpty(FnameSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.FirstName.ToLower().StartsWith(FnameSearch.ToLower()));
            }
            if (!String.IsNullOrEmpty(GroupSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.Group.Equals(GroupSearch));
            }
            if (!String.IsNullOrEmpty(CountrySearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.Country.Equals(CountrySearch));
            }
            if (!String.IsNullOrEmpty(QuarterSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.YearQuarterEnrolled.Equals(QuarterSearch));
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
            SortDirSid = SortDirFname = SortDirLname = SortDirGroup = SortDirCountry = SortDirQuarter = "down";
            if ( !String.IsNullOrEmpty(sortType) )
            {
                switch(sortType)
                {
                    case "lname_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.LastName);
                        LnameSort = "lname";
                        SortDirLname = "up";
                        break;
                    case "lname":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.LastName);
                        LnameSort = "lname_desc"; 
                        break;
                    case "fname_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.FirstName);
                        FnameSort = "fname";
                        SortDirFname = "up";
                        break;
                    case "fname":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.FirstName);
                        FnameSort = "fname_desc";
                        break;
                    case "sid":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.Sid);
                        SidSort = "sid_desc";
                        break;
                    case "sid_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.Sid);
                        SidSort = "sid";
                        SortDirSid = "up";
                        break;
                    case "group":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.Group);
                        GroupSort = "group_desc";
                        break;
                    case "group_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.Group);
                        GroupSort = "group";
                        SortDirGroup = "up";
                        break;
                    case "country":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.Country);
                        CountrySort = "country_desc";
                        break;
                    case "country_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.Country);
                        CountrySort = "country";
                        SortDirCountry = "up";
                        break;
                    case "quarter":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.YearQuarterEnrolled);
                        QuarterSort = "quarter_desc";
                        break;
                    case "quarter_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.YearQuarterEnrolled);
                        QuarterSort = "quarter";
                        SortDirQuarter = "up";
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

            //Now set to list and save to variable
            StudentData = StudentsIQ.ToList();
        }
    }
}