using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SiteServer.Abstractions;

namespace SiteServer.Web.Core
{
    public static class SharedModel
    {

        public const string ApiUrl = Constants.ApiPrefix;

        public const string AdminUrl = Constants.AdminPrefix;
    }
}
