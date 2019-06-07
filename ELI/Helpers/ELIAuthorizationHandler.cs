using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ELI.Models;
using Microsoft.Extensions.Logging;

namespace ELI.Helpers
{
    /**
     * The authorization handler class for ELI
     * **/
    public class ELIAuthorizationHandler : AuthorizationHandler<ELIAdminRequirement>
    {
        IOptionsSnapshot<ApplicationSettings> _appSettings;
        private readonly ILogger _logger;

        // basic constructor
        public ELIAuthorizationHandler(IOptionsSnapshot<ApplicationSettings> appSettings, ILogger<ELIAuthorizationHandler> logger)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        /**
         * Handles the authorization requirement for ELI
         * Gets the list of authorized groups from config and checks that 
         * the logged in user has that role.
         * **/
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ELIAdminRequirement requirement)
        {
            string groupsSetting = _appSettings.Value.AuthorizedGroups;
            bool passed = false;

            _logger.LogInformation("Checking authorization groups.");

            if (!String.IsNullOrWhiteSpace(groupsSetting))
            {
                // split lists of groups, then loop through and check each until passed (or not)
                var groups = groupsSetting.Split(",");
                foreach (var group in groups)
                {
                    if (context.User.IsInRole(group))
                    {
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