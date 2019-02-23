using System;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_TemplateLog")]
    public class TemplateLogInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int TemplateId { get; set; }

        public int SiteId { get; set; }

        public DateTime? AddDate { get; set; }

        public string AddUserName { get; set; }

        public int ContentLength { get; set; }

        [Text]
        public string TemplateContent { get; set; }
    }
}
