using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ELI.Helpers;

namespace ELI.Pages.Components.QuarterSelect
{
    /**
     * This class extends ViewComponent to provide 
     * quarter information and set for the session the 
     * selected quarter for use in the template layout.
     * **/
    public class QuarterSelectViewComponent : ViewComponent
    {
        private readonly ELIContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;
        private readonly ApplicationSettings _appSettings;

        public QuarterSelectViewComponent(ELIContext context, ILogger<QuarterSelectViewComponent> logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
            _appSettings = _config.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>();
        }

        /**
         * Calls setting method and returns quarters list to view
         * **/
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var quarters = await SetAndGetQuartersAsync();
            return View(quarters);
        }

        /**
         * Sets the selected quarter to session if one not already 
         * set. Then gets the list of valid quarters and returns
         * **/
        private Task<List<Quarter>> SetAndGetQuartersAsync()
        {
            Utility util = new Utility(_config);
            //if current quarter session value is null, query current quarter and set
            if ( _httpContextAccessor.HttpContext.Session.Get<Quarter>(_appSettings.SessionKey_SelectedQuarter) == null )
            {
                //get current quarter and set it in the session
                Quarter curQuarter = util.getCurrentQuarter(_context);
                _httpContextAccessor.HttpContext.Session.Set<Quarter>(_appSettings.SessionKey_SelectedQuarter, curQuarter);
                Quarter quar = _httpContextAccessor.HttpContext.Session.Get<Quarter>(_appSettings.SessionKey_SelectedQuarter);
                //_logger.LogDebug(quar.Id);
            }

            return util.getQuarters(_context, 10).ToListAsync();
        }
    }
}
