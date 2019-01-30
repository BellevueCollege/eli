using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ELI.Models;

namespace ELI.Pages
{
    public abstract class EliPageModel : PageModel
    {
        protected readonly IConfiguration _config;
        protected readonly ELIContext _context;
        protected readonly ILogger _logger;

        [BindProperty]
        public string QuarterSelect { get; set; }

        public EliPageModel(ELIContext context, IConfiguration config, ILogger logger)
        {
            _context = context;
            _config = config;
            _logger = logger;
        }

        public void SetSelectedQuarter()
        {
            //_logger.LogDebug(QuarterSelect.ToString());
            if (QuarterSelect != null)
            {
                var settings = GetSettings();
                //get current quarter and set it in the session
                Quarter selQuarter = _context.Quarters.Where(q => q.Id == QuarterSelect).Take(1).Single();
                //_logger.LogDebug(QuarterSelect);
                HttpContext.Session.Set<Quarter>(settings.SessionKey_SelectedQuarter, selQuarter);
                //_logger.LogDebug(HttpContext.Session.Get<Quarter>(settings.SessionKey_SelectedQuarter).Id);
            }
        }

        public Quarter GetSelectedQuarter()
        {
            return HttpContext.Session.Get<Quarter>(GetSettings().SessionKey_SelectedQuarter);
        }

        public ApplicationSettings GetSettings()
        {
            return _config.GetSection(ApplicationSettings.SectionName).Get<ApplicationSettings>();
        }

        public IActionResult OnPostQuarterSet()
        {
            SetSelectedQuarter();
            return RedirectToPage();
        }

    }
}