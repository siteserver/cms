using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ErrorLog")]
    public class ErrorLogInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public string Category { get; set; }

        public string PluginId { get; set; }

        public string Message { get; set; }

        [Text]
        public string Stacktrace { get; set; }

        [Text]
        public string Summary { get; set; }

        public DateTime? AddDate { get; set; }
    }
}
