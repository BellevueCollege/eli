using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using ELI.Models;
using ELI.Helpers;

namespace ELI.Pages
{
    public class PlaceReturningStudentsModel : EliPageModel
    {
        public PlaceReturningStudentsModel(ELIContext context, IConfiguration config, ILogger<PlaceReturningStudentsModel> logger) : base(context, config, logger) { }

        [BindProperty]
        public IList<Student> Students { get; set; }

        [BindProperty(SupportsGet = true)]
        public string LnameSearch { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FnameSearch { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SidSearch { get; set; }

        public string SidSort { get; set; }
        public string LnameSort { get; set; }
        public string FnameSort { get; set; }

        public string SortDirSid { get; set; }
        public string SortDirLname { get; set; }
        public string SortDirFname { get; set; }
        public string SortType { get; set; }
        public string CurrentSortType { get; set; }
        public Dictionary<string, string> QueryParams { get; set; }
        public async Task OnGetAsync(string sortType)
        {
            SortType = sortType;
            await SetStudents();
        }

        /** Page handler method for filters **/
        public async Task OnPostApplyFiltersAsync(string sortType)
        {
            SortType = sortType;
            await SetStudents();
        }

        /** Page handler method for placing students **/
        public async Task OnPostAutoPlaceStudentsAsync(string sortType)
        {
            SortType = sortType;
            await SetStudents();

            if (ModelState.IsValid)
            {
                //loop through students and add/update scores as necessary
                int i = 0;
                foreach (var student in Students)
                {
                    if (student.Level.WriteLevel == null && student.Level.ReadLevel == null && student.Level.SpeakLevel == null)
                    {
                        //nothing to update, continue
                        continue;
                    }

                    var studentToUpdate = await _context.Students.Include(s => s.Level).FirstOrDefaultAsync(s => s.Sid == student.Sid);

                    if (studentToUpdate == null)
                    {
                        //if the student was removed for whatever reason, just carry on
                        continue;
                    }

                    // ----- Placement Logic
                    // If Grade is C- (1.70) & above or Pass (P) then next level
                    // Else Grade is D+ & below, F, Not Pass, W, HW then repeat/last level

                    // Reading
                    if ((student.Level.ReadLevel < 5 && student.Level.ReadGradePoint >= 1.70M) || (student.Level.ReadGrade == "P"))
                    {
                        studentToUpdate.Level.ReadPlace = student.Level.ReadLevel + 1;
                        //_logger.LogDebug("set read level: {0}", studentToUpdate.Level.ReadPlace);
                    }
                    else
                    {
                        studentToUpdate.Level.ReadPlace = student.Level.ReadLevel;
                        //_logger.LogDebug("set read level: {0}", studentToUpdate.Level.ReadPlace);
                    }

                    // Writing
                    if ((student.Level.WriteLevel < 5 && student.Level.WriteGradePoint >= 1.70M) || (student.Level.WriteGrade == "P"))
                    {
                        studentToUpdate.Level.WritePlace = student.Level.WriteLevel + 1;
                        //_logger.LogDebug("set write level: {0}", studentToUpdate.Level.WritePlace);
                    }
                    else
                    {
                        studentToUpdate.Level.WritePlace = student.Level.WriteLevel;
                        //_logger.LogDebug("set write level: {0}", studentToUpdate.Level.WritePlace);
                    }

                    // Speaking
                    if ((student.Level.SpeakLevel < 5 && student.Level.SpeakGradePoint >= 1.70M) || (student.Level.SpeakGrade == "P"))
                    {
                        studentToUpdate.Level.SpeakPlace = student.Level.SpeakLevel + 1;
                        //_logger.LogDebug("set speak level: {0}", studentToUpdate.Level.SpeakPlace);
                    }
                    else
                    {
                        studentToUpdate.Level.SpeakPlace = student.Level.SpeakLevel;
                        //_logger.LogDebug("set speak level: {0}", studentToUpdate.Level.SpeakPlace);
                    }

                    _context.Students.Update(studentToUpdate);
                    i++;
                }

                //process post info
                await _context.SaveChangesAsync();
                await SetStudents(); //shows list of students
            }
        }

        private async Task SetStudents()
        {
            Quarter quar = GetSelectedQuarter();
            var queryParams = new Dictionary<string, string>();

            if (quar == null)
            {
                //_logger.LogDebug("AddScores - SetStudents() - Have to get current quarter.");
                Utility util = new Utility(_config);
                quar = util.getCurrentQuarter(_context);
            }

            //set up initial query
            /** Use IQueryable so additional conditionals can be added before converting to a 
             * collection (at which time the query goes to the db)
             * **/
            IQueryable<Student> StudentsIQ = (from s in _context.Students
                                              where s.StuType == StudentType.Returning
                                              && s.YearQuarterEnrolled == quar.Id
                                              orderby s.LastName, s.FirstName      //default ordering
                                              select s).Include(st => st.Level);

            // Based on input search filters, filter student data
            if (!String.IsNullOrEmpty(LnameSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.LastName.ToLower().StartsWith(LnameSearch.ToLower()));
                queryParams.Add("LnameSearch", LnameSearch);
            }
            if (!String.IsNullOrEmpty(SidSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.Sid.StartsWith(SidSearch));
                queryParams.Add("SidSearch", SidSearch);
            }
            if (!String.IsNullOrEmpty(FnameSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.FirstName.ToLower().StartsWith(FnameSearch.ToLower()));
                queryParams.Add("FnameSearch", FnameSearch);
            }

            // Now sort data per any inputs 
            FnameSort = "fname";
            LnameSort = "lname_desc";
            SidSort = "sid";

            //used for determining icon direction
            SortDirSid = SortDirFname = SortDirLname = "down";
            if (!String.IsNullOrEmpty(SortType))
            {
                switch (SortType)
                {
                    case "lname":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.LastName);
                        LnameSort = "lname_desc";
                        break;
                    case "lname_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.LastName);
                        LnameSort = "lname";
                        SortDirLname = "up";
                        break;
                    case "fname":
                        StudentsIQ = StudentsIQ.OrderBy(s => s.FirstName);
                        FnameSort = "fname_desc";
                        break;
                    case "fname_desc":
                        StudentsIQ = StudentsIQ.OrderByDescending(s => s.FirstName);
                        FnameSort = "fname";
                        SortDirFname = "up";
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
                }
                CurrentSortType = SortType;
            }
            else
            {
                FnameSort = "fname";
                LnameSort = "lname_desc";
                SidSort = "sid";
            }

            QueryParams = queryParams;
            Students = await StudentsIQ.ToListAsync();
        }
    }
}