using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ELI.Models;

namespace ELI.Pages
{
    public class IndexModel : EliPageModel
    {
        public IndexModel(ELIContext context, IConfiguration config, ILogger<IndexModel> logger) : base(context, config, logger) { }

        public void OnGet()
        {

        }
    }
}
