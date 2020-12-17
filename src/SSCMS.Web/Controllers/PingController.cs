using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Services;

namespace SSCMS.Web.Controllers
{
    [Route("api/ping")]
    public partial class PingController : ControllerBase
    {
        private const string Route = "";
        private const string RouteIp = "ip";
        private const string RouteStatus = "status";

        private readonly ISettingsManager _settingsManager;

        public PingController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public class StatusResult
        {
            public string Name { get; set; }
            public string Env { get; set; }
            public bool Containerized { get; set; }
            public string Version { get; set; }
            public bool IsDatabaseWorks { get; set; }
            public bool IsRedisWorks { get; set; }
        }
    }
}
