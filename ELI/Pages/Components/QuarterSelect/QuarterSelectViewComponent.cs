using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace ELI.Pages.Components.QuarterSelect
{
    public class QuarterSelectViewComponent : ViewComponent
    {
        private readonly ELIContext _context;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public const string Session_CurrentQuarter_Key = "_CurrentQuarter";

        public QuarterSelectViewComponent(ELIContext context, ILogger<QuarterSelectViewComponent> logger, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var quarters = await SetAndGetQuartersAsync();
            return View(quarters);
        }

        private Task<List<Quarter>> SetAndGetQuartersAsync()
        {
            //if current quarter session value is null, query current quarter and set
            if ( _httpContextAccessor.HttpContext.Session.Get<Quarter>(Session_CurrentQuarter_Key) == null )
            {
                //get current quarter and set it in the session
                Quarter curQuarter = _context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != "Z999").OrderByDescending(q => q.Id).Take(1).Single();
                //_logger.LogDebug(curQuarter.Title.ToString());
                _httpContextAccessor.HttpContext.Session.Set<Quarter>(Session_CurrentQuarter_Key, curQuarter);
                Quarter quar = _httpContextAccessor.HttpContext.Session.Get<Quarter>(Session_CurrentQuarter_Key);
                //_logger.LogDebug(quar.Id);
            }

            return _context.Quarters.Where(q => DateTime.Now.Date >= q.FirstClassDay && q.Id != "Z999").OrderByDescending(q => q.Id).Take(10).ToListAsync();

        }
    }
}
