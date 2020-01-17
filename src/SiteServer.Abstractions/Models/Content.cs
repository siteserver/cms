using System;
using System.Collections.Generic;
using Datory;
using Datory.Annotations;
using Newtonsoft.Json;


namespace SiteServer.Abstractions
{
    public class Content : Entity
    {
        public Content()
        {

        }

        public Content(IDictionary<string, object> dict) : base(dict)
        {

        }

        [DataColumn]
        public int ChannelId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string AddUserName { get; set; }

        [DataColumn]
        public string LastEditUserName { get; set; }

        [DataColumn]
        public DateTime? LastEditDate { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string GroupNameCollection { get; set; }

        [DataIgnore]
        public List<string> GroupNames
        {
            get => StringUtils.GetStringList(GroupNameCollection);
            set => GroupNameCollection = TranslateUtils.ObjectCollectionToString(value);
        }

        [DataColumn]
        [JsonIgnore]
        private string Tags { get; set; }

        [DataIgnore]
        public List<string> TagNames
        {
            get => StringUtils.GetStringList(Tags);
            set => Tags = TranslateUtils.ObjectCollectionToString(value);
        }

        [DataColumn] 
        public int SourceId { get; set; }

        [DataColumn] 
        public int ReferenceId { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string IsChecked { get; set; }

        [DataIgnore]
        public bool Checked
        {
            get => TranslateUtils.ToBool(IsChecked);
            set => IsChecked = value.ToString();
        }

        [DataColumn]
        public int CheckedLevel { get; set; }

        [DataColumn]
        public int Hits { get; set; }

        [DataColumn]
        public int HitsByDay { get; set; }

        [DataColumn]
        public int HitsByWeek { get; set; }

        [DataColumn]
        public int HitsByMonth { get; set; }

        [DataColumn]
        public DateTime? LastHitsDate { get; set; }

        [DataColumn] 
        public int Downloads { get; set; }

        [DataColumn] 
        public string Title { get; set; }

        [DataColumn]
        [JsonIgnore]
        private string IsTop { get; set; }

        [DataIgnore]
        public bool Top
        {
            get => TranslateUtils.ToBool(IsTop);
            set => IsTop = value.ToString();
        }

        [DataColumn]
        [JsonIgnore]
        private string IsRecommend { get; set; }

        [DataIgnore]
        public bool Recommend
        {
            get => TranslateUtils.ToBool(IsRecommend);
            set => IsRecommend = value.ToString();
        }

        [DataColumn]
        [JsonIgnore]
        private string IsHot { get; set; }

        [DataIgnore]
        public bool Hot
        {
            get => TranslateUtils.ToBool(IsHot);
            set => IsHot = value.ToString();
        }

        [DataColumn]
        [JsonIgnore]
        private string IsColor { get; set; }

        [DataIgnore]
        public bool Color
        {
            get => TranslateUtils.ToBool(IsColor);
            set => IsColor = value.ToString();
        }

        [DataColumn] 
        public string LinkUrl { get; set; }

        [DataColumn] 
        public DateTime? AddDate { get; set; }

        [DataColumn(Text = true, Extend = true)]
        private string SettingsXml { get; set; }

        public string CheckUserName { get; set; } //审核者

        public DateTime? CheckDate { get; set; } //审核时间

        public string CheckReasons { get; set; } //审核原因

        public TranslateContentType TranslateContentType { get; set; } //转移内容类型
    }
}
