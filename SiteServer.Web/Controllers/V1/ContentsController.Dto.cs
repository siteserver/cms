using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using NSwag.Annotations;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SqlKata;

namespace SiteServer.API.Controllers.V1
{
    public partial class ContentsController
    {
        public const string OpEquals = "=";
        public const string OpIn = "In";
        public const string OpNotIn = "NotIn";
        public const string OpLike = "Like";
        public const string OpNotLike = "NotLike";

        public class ClauseWhere
        {
            public string Column { get; set; }
            public string Operator { get; set; }
            public string Value { get; set; }
        }

        public class ClauseOrder
        {
            public string Column { get; set; }
            public bool Desc { get; set; }
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public int? ChannelId { get; set; }
            public bool? Checked { get; set; }
            public bool? Top { get; set; }
            public bool? Recommend { get; set; }
            public bool? Color { get; set; }
            public bool? Hot { get; set; }
            public List<string> GroupNames { get; set; }
            public List<string> TagNames { get; set; }
            public List<ClauseWhere> Wheres { get; set; }
            public List<ClauseOrder> Orders { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public int TotalCount { get; set; }
            public IEnumerable<ContentInfo> Contents { get; set; }
        }

        public class CheckRequest
        {
            public int SiteId { get; set; }
            public List<ContentSummary> Contents { get; set; }
            public string Reasons { get; set; }
        }

        public class CheckResult
        {
            public List<ContentInfo> Contents { get; set; }
        }
    }
}