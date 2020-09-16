using System;
using Newtonsoft.Json;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicContent
    {
        private readonly ISettingsManager _settingsManager;
        public TableGovPublicContent(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addUserName")]
        public string AddUserName { get; set; }

        [JsonProperty("lastEditUserName")]
        public string LastEditUserName { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("contentGroupNameCollection")]
        public string ContentGroupNameCollection { get; set; }

        [JsonProperty("groupNameCollection")]
        public string GroupNameCollection { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("sourceID")]
        public long SourceId { get; set; }

        [JsonProperty("referenceID")]
        public long ReferenceId { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("checkedLevel")]
        public long CheckedLevel { get; set; }

        [JsonProperty("comments")]
        public long Comments { get; set; }

        [JsonProperty("hits")]
        public long Hits { get; set; }

        [JsonProperty("hitsByDay")]
        public long HitsByDay { get; set; }

        [JsonProperty("hitsByWeek")]
        public long HitsByWeek { get; set; }

        [JsonProperty("hitsByMonth")]
        public long HitsByMonth { get; set; }

        [JsonProperty("lastHitsDate")]
        public DateTime LastHitsDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("category1ID")]
        public long Category1Id { get; set; }

        [JsonProperty("category2ID")]
        public long Category2Id { get; set; }

        [JsonProperty("category3ID")]
        public long Category3Id { get; set; }

        [JsonProperty("category4ID")]
        public long Category4Id { get; set; }

        [JsonProperty("category5ID")]
        public long Category5Id { get; set; }

        [JsonProperty("category6ID")]
        public long Category6Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("publishDate")]
        public DateTime PublishDate { get; set; }

        [JsonProperty("effectDate")]
        public DateTime EffectDate { get; set; }

        [JsonProperty("isAbolition")]
        public string IsAbolition { get; set; }

        [JsonProperty("abolitionDate")]
        public DateTime AbolitionDate { get; set; }

        [JsonProperty("documentNo")]
        public string DocumentNo { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("isRecommend")]
        public string IsRecommend { get; set; }

        [JsonProperty("isTop")]
        public string IsTop { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("addDate")]
        public DateTime AddDate { get; set; }
    }
}
