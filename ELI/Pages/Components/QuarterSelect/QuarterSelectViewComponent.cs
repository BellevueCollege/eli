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

namespace ELI.Pages.Components.QuarterSelect
{
    public class QuarterSelectViewComponent : ViewComponent
    {
        private readonly ELIContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public QuarterSelectViewComponent(ELIContext context, ILogger<QuarterSelectViewComponent> logger, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var quarters = await SetAndGetQuartersAsync();
            return View(quarters);
        }

        private Task<List<Quarter>> SetAndGetQuartersAsync()
        {
            var settings = _config.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>();
            //if current quarter session value is null, query current quarter and set
            if ( _httpContextAccessor.HttpContext.Session.Get<Quarter>(settings.SessionKey_SelectedQuarter) == null )
            {
                //get current quarter and set it in the session
                Quarter curQuarter = _context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != "Z999").OrderByDescending(q => q.Id).Take(1).Single();
                //_logger.LogDebug(curQuarter.Title.ToString());
                _httpContextAccessor.HttpContext.Session.Set<Quarter>(settings.SessionKey_SelectedQuarter, curQuarter);
                Quarter quar = _httpContextAccessor.HttpContext.Session.Get<Quarter>(settings.SessionKey_SelectedQuarter);
                //_logger.LogDebug(quar.Id);
            }

            return _context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != "Z999").OrderByDescending(q => q.Id).Take(10).ToListAsync();

        }
    }
}
