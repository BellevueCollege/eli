using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ELI.Models;

namespace ELI.Helpers
{

    public class ELIAuthorizationHandler : AuthorizationHandler<ELIAdminRequirement>
    {
        IOptionsSnapshot<ApplicationSettings> _appSettings;

        public ELIAuthorizationHandler(IOptionsSnapshot<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ELIAdminRequirement requirement)
        {
            string groupsSetting = _appSettings.Value.AuthorizedGroups;
            bool passed = false;

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