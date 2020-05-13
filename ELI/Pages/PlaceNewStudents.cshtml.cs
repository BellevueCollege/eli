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
    public class PlaceNewStudentsModel : EliPageModel
    {
        public PlaceNewStudentsModel(ELIContext context, IConfiguration config, ILogger<PlaceNewStudentsModel> logger) : base(context, config, logger){}

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

        /** Page handler method for save scores **/
        public async Task OnPostSaveScoresAsync(string sortType)
        {
            SortType = sortType;
            if (ModelState.IsValid)
            {
                Utility util = new Utility(_config);
                string modUsername = util.getUsernameFromIdentityName(HttpContext.User.Identity.Name);

                //loop through students and add/update scores as necessary
                int i = 0;
                foreach (var student in Students)
                {
                    if (student.Score.EptScore == null && student.Score.OralScore == null && student.Score.WriteScore == null)
                    {
                        //nothing to update, continue
                        continue;
                    }

                    //_logger.LogDebug("student info {0}", JsonConvert.SerializeObject(student));
                    var studentToUpdate = await _context.Students.Include(s => s.Score).FirstOrDefaultAsync(s => s.Sid == student.Sid);

                    //store original scores from database in object to compare to later
                    Scores origScores = new Scores{
                                        Sid = studentToUpdate.Score.Sid,
                                        EptScore = studentToUpdate.Score.EptScore,
                                        EptPlacement = studentToUpdate.Score.EptPlacement,
                                        OralScore = studentToUpdate.Score.OralScore,
                                        OralPlacement = studentToUpdate.Score.OralPlacement,
                                        WriteScore = studentToUpdate.Score.WriteScore,
                                        WritePlacement = studentToUpdate.Score.WritePlacement
                    };

                    //_logger.LogDebug("origScores 1: {0}", JsonConvert.SerializeObject(origScores));
                    //_logger.LogDebug("form scores 1: {0}", JsonConvert.SerializeObject(student.Score));
                    if (studentToUpdate == null)
                    {
                        //if the student was removed for whatever reason, just carry on
                        continue;
                    }
                    _logger.LogDebug("student score update info {0}", JsonConvert.SerializeObject(studentToUpdate));

                    /*** check if values changed and update accordingly ***/
                    bool isScoresChanged = studentToUpdate.Score.IsEqualTo(student.Score);
                    // Ept score/placement logic
                    if ( studentToUpdate.Score.EptScore != student.Score.EptScore )
                    {
                        studentToUpdate.Score.EptScore = student.Score.EptScore;
                        if (student.Score.EptPlacement == null)
                        {
                            student.Score.AssignEptPlacement();
                            studentToUpdate.Score.EptPlacement = student.Score.EptPlacement;
                        }
                    }
                    if ( studentToUpdate.Score.EptPlacement != student.Score.EptPlacement)
                    {
                        studentToUpdate.Score.EptPlacement = student.Score.EptPlacement;
                    }

                    // Oral score/placement logic
                    if (studentToUpdate.Score.OralScore != student.Score.OralScore)
                    {
                        studentToUpdate.Score.OralScore = student.Score.OralScore;
                        if (student.Score.OralPlacement == null)
                        {
                            student.Score.AssignOralPlacement();
                            studentToUpdate.Score.OralPlacement = student.Score.OralPlacement;
                        }
                    }
                    if (studentToUpdate.Score.OralPlacement != student.Score.OralPlacement)
                    {
                        studentToUpdate.Score.OralPlacement = student.Score.OralPlacement;
                    }

                    // Written score/placement logic
                    if (studentToUpdate.Score.WriteScore != student.Score.WriteScore)
                    {
                        studentToUpdate.Score.WriteScore = student.Score.WriteScore;
                        if (student.Score.WritePlacement == null)
                        {
                            student.Score.AssignWritePlacement();
                            studentToUpdate.Score.WritePlacement = student.Score.WritePlacement;
                        }
                    }
                    if (studentToUpdate.Score.WritePlacement != student.Score.WritePlacement)
                    {
                        studentToUpdate.Score.WritePlacement = student.Score.WritePlacement;
                    }

                    //_logger.LogDebug("origScores 2: {0}", JsonConvert.SerializeObject(origScores));
                    //_logger.LogDebug("form scores 2: {0}", JsonConvert.SerializeObject(student.Score));
                    // data has changed so set modify updates
                    if (! origScores.IsEqualTo(student.Score))
                    {
                        _logger.LogDebug("SID: {0} - scores aren't equal so set modify info.", studentToUpdate.Sid);
                        studentToUpdate.Score.ModifyDt = DateTime.Now;
                        studentToUpdate.Score.ModifyUsername = modUsername;
                        _context.Students.Update(studentToUpdate);
                    }
                    
                    // Update model
                    /*if ( await TryUpdateModelAsync<Scores>(
                        studentToUpdate.Score,
                        String.Format("Students[{0}].Score", i),
                        s => s.EptScore,  s => s.EptPlacement, s => s.OralScore, 
                        s => s.OralPlacement, s => s.WriteScore, s => s.WritePlacement) )
                    {*/

                    /*}*/
                    //_logger.LogDebug("success {0}", success);

                    i++;
                }

                //process post info
                await _context.SaveChangesAsync();
                await SetStudents();

                //return RedirectToPage();
      
            }
        }

        private async Task SetStudents()
        {
            Quarter quar = GetSelectedQuarter();
            var queryParams = new Dictionary<string, string>();

            if (quar == null) {
                //_logger.LogDebug("AddScores - SetStudents() - Have to get current quarter.");
                Utility util = new Utility(_config);
                quar = util.getCurrentQuarter(_context);
            }

            //set up initial query
            /** Use IQueryable so additional conditionals can be added before converting to a 
             * collection (at which time the query goes to the db)
             * **/

            // Model State clears so that when page reloads new saved scores will filter/sort properly with their respective students
            ModelState.Clear();
            IQueryable<Student> StudentsIQ = (from s in _context.Students
                                             where s.StuType == StudentType.New 
                                             && s.YearQuarterEnrolled == quar.Id 
                                             orderby s.LastName, s.FirstName      //default ordering
                                             select s).Include(st => st.Score);

            // Based on input search filters, filter student data
            if (!String.IsNullOrEmpty(LnameSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.LastName.StartsWith(LnameSearch));
                queryParams.Add("LnameSearch", LnameSearch);
            }
            if (!String.IsNullOrEmpty(SidSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.Sid.StartsWith(SidSearch));
                queryParams.Add("SidSearch", SidSearch);
            }
            if (!String.IsNullOrEmpty(FnameSearch))
            {
                StudentsIQ = StudentsIQ.Where(s => s.FirstName.StartsWith(FnameSearch));
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