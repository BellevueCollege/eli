using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ELI.Helpers
{
    /**
     * A very basic authorization requirement class utilizing 
     * the empty marker interface of IAuthorizationRequirement
     * **/
    public class ELIAdminRequirement : IAuthorizationRequirement
    {
    }
}
