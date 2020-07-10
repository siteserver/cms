using System;
using System.Collections.Generic;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    public partial class ContentsLayerExportController
    {
        public class GetResult
        {
            public List<ContentColumn> Columns { get; set; }
            public List<KeyValuePair<int, string>> CheckedLevels { get; set; }
            public int CheckedLevel { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public string ExportType { get; set; }
            public bool IsAllCheckedLevel { get; set; }
            public List<int> CheckedLevelKeys { get; set; }
            public bool IsAllDate { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public List<string> ColumnNames { get; set; }
        }

        public class SubmitResult
        {
            public string Value { get; set; }
            public bool IsSuccess { get; set; }
        }
    }
}
