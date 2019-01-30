using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ELI.Models;
using ELI.Helpers;

namespace ELI.Pages
{
    public class AddScoresModel : EliPageModel
    {
        public AddScoresModel(ELIContext context, IConfiguration config, ILogger<AddScoresModel> logger) : base(context, config, logger){}

        [BindProperty]
        public IList<Student> Students { get; set; }

        public async Task OnGetAsync()
        {
            await SetStudents();
        }

        public async Task OnPostAsync()
        {
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

                    _logger.LogDebug("student info {0}", JsonConvert.SerializeObject(student));
                    var studentToUpdate = await _context.Students.Include(s => s.Score).FirstOrDefaultAsync(s => s.Sid == student.Sid);
                    
                    if (studentToUpdate == null)
                    {
                        //if the student was removed for whatever reason, just carry on
                        continue;
                    }
                    _logger.LogDebug("student score update info {0}", JsonConvert.SerializeObject(studentToUpdate));

                    /*** check if values changed and update accordingly ***/

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

                    // data has changed so set modify updates
                    if (!studentToUpdate.Equals(student))
                    {
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

            if (quar == null) {
                //_logger.LogDebug("AddScores - SetStudents() - Have to get current quarter.");
                Utility util = new Utility(_config);
                quar = util.getCurrentQuarter(_context);
            }

            Students = await _context.Students.Include(s => s.Score).Where(s => s.StuType == StudentType.New && s.YearQuarterEnrolled == quar.Id).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToListAsync();
        }
    }
}