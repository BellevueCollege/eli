using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ELI.Models;
//using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Logging;

namespace ELI.Helpers
{

    public class ELIAuthorizationHandler : AuthorizationHandler<ELIAdminRequirement>
    {
        IOptionsSnapshot<ApplicationSettings> _appSettings;
        private readonly ILogger _logger;

        public ELIAuthorizationHandler(IOptionsSnapshot<ApplicationSettings> appSettings, ILogger<ELIAuthorizationHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ELIAdminRequirement requirement)
        {
            string groupsSetting = _appSettings.Value.AuthorizedGroups;
            bool passed = false;

            _logger.LogInformation("Checking authorization groups.");

            if (!String.IsNullOrWhiteSpace(groupsSetting))
            {
                var groups = groupsSetting.Split(",");
                foreach (var group in groups)
                {
                    if (context.User.IsInRole(group))
                    {
                        //context.Succeed(requirement);
                        passed = true;
                        break;
                    }
                }
            }

            if (passed)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.FromResult(0);
        }
    }
}