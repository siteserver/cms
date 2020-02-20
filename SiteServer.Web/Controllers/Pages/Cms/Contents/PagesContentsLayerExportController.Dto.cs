using System;
using System.Collections.Generic;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    public partial class PagesContentsLayerExportController
    {
        public class GetResult
        {
            public List<ContentColumn> Value { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ChannelContentIds { get; set; }
            public string ExportType { get; set; }
            public bool IsAllCheckedLevel { get; set; }
            public List<int> CheckedLevelKeys { get; set; }
            public bool IsAllDate { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public bool IsAllColumns { get; set; }
            public List<string> ColumnNames { get; set; }
        }

        public class SubmitResult
        {
            public string Value { get; set; }
            public bool IsSuccess { get; set; }
        }
    }
}