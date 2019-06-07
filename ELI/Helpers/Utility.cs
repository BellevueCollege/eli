using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ELI.Helpers
{
    /**
     * Utility class for miscellaneous functions
     * **/
    public class Utility
    {
        private readonly ApplicationSettings _appSettings;

        public Utility(IConfiguration config)
        {
            _appSettings = config.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>();
        }

        // get the current quarter
        public Quarter getCurrentQuarter(ELIContext context)
        {   
            return context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != _appSettings.MaxQuarter).OrderByDescending(q => q.Id).Take(1).Single();
        }

        // get quarters
        public IQueryable<Quarter> getQuarters(ELIContext context, int count = 1)
        {
            return context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != _appSettings.MaxQuarter).OrderByDescending(q => q.Id).Take(count);
        }

        //get quarters as a list
        public List<Quarter> getQuartersList(ELIContext context, int count = 1)
        {
            return context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != _appSettings.MaxQuarter).OrderByDescending(q => q.Id).Take(count).ToList();
        }

        // get just the username string from the user identity
        public string getUsernameFromIdentityName(string identityName)
        {
            string curUsername = identityName;
            if (identityName.IndexOf(@"\") > -1) curUsername = identityName.Substring(identityName.IndexOf(@"\") + 1);
            return curUsername;
        }
    }
}
