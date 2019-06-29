using System;
using System.Collections.Generic;
using System.Drawing;

namespace SS.CMS.Api.Controllers.Cms
{
    public partial class CmsController
    {
        public class InfoResponse
        {
            public string ServerName { get; set; }
            public string ContentRootPath { get; set; }
            public string WebRootPath { get; set; }
            public string AdminHostName { get; set; }
            public string RemoteIpAddress { get; set; }
            public string TargetFramework { get; set; }
            public string ProductVersion { get; set; }
            public string PluginVersion { get; set; }
            public DateTime? UpdateDate { get; set; }
            public string DatabaseType { get; set; }
            public string Database { get; set; }
        }
    }
}
