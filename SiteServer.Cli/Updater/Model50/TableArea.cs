using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableArea
    {
        [JsonProperty("areaId")]
        public long AreaId { get; set; }

        [JsonProperty("areaName")]
        public string AreaName { get; set; }

        [JsonProperty("parentId")]
        public long ParentId { get; set; }

        [JsonProperty("parentsPath")]
        public string ParentsPath { get; set; }

        [JsonProperty("parentsCount")]
        public long ParentsCount { get; set; }

        [JsonProperty("childrenCount")]
        public long ChildrenCount { get; set; }

        [JsonProperty("isLastNode")]
        public string IsLastNode { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("countOfAdmin")]
        public long CountOfAdmin { get; set; }
    }

    public partial class TableArea
    {
        public const string OldTableName = "Area";

        public static readonly string NewTableName = DataProvider.AreaDao.TableName;

        public static readonly List<TableColumn> NewColumns = DataProvider.AreaDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(AreaInfo.Id), nameof(AreaId)}
            };

        public static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
