using System;
using System.Collections.Generic;
using Datory;


namespace SiteServer.Abstractions
{
    [Serializable]
    public class Content : Entity, IContentMin
    {
        public Content()
        {

        }

        public Content(IDictionary<string, object> dict) : base(dict)
        {

        }

        [DataColumn] public int ChannelId { get; set; }

        [DataColumn] public int SiteId { get; set; }

        [DataColumn] public string AddUserName { get; set; }

        [DataColumn] public string LastEditUserName { get; set; }

        [DataColumn] public DateTime? LastEditDate { get; set; }

        [DataColumn] public int AdminId { get; set; }

        [DataColumn] public int UserId { get; set; }

        [DataColumn] public int Taxis { get; set; }

        [DataColumn] public string GroupNameCollection { get; set; }

        public List<string> GroupNames
        {
            get => StringUtils.GetStringList(GroupNameCollection);
            set => GroupNameCollection = TranslateUtils.ObjectCollectionToString(value);
        }

        [DataColumn] public string Tags { get; set; }

        [DataColumn] public int SourceId { get; set; }

        [DataColumn] public int ReferenceId { get; set; }

        [DataColumn] public string IsChecked { get; set; }

        public bool Checked
        {
            get => TranslateUtils.ToBool(IsChecked);
            set => IsChecked = value.ToString();
        }

        [DataColumn] public int CheckedLevel { get; set; }

        [DataColumn] public int Hits { get; set; }

        [DataColumn] public int HitsByDay { get; set; }

        [DataColumn] public int HitsByWeek { get; set; }

        [DataColumn] public int HitsByMonth { get; set; }

        [DataColumn] public DateTime? LastHitsDate { get; set; }

        [DataColumn] public int Downloads { get; set; }

        [DataColumn] public string Title { get; set; }

        public bool Top
        {
            get => TranslateUtils.ToBool(IsTop);
            set => IsTop = value.ToString();
        }

        public bool Recommend
        {
            get => TranslateUtils.ToBool(IsRecommend);
            set => IsRecommend = value.ToString();
        }

        public bool Hot
        {
            get => TranslateUtils.ToBool(IsHot);
            set => IsHot = value.ToString();
        }

        public bool Color
        {
            get => TranslateUtils.ToBool(IsColor);
            set => IsColor = value.ToString();
        }

        [DataColumn] public string IsTop { get; set; }

        [DataColumn] private string IsRecommend { get; set; }

        [DataColumn] private string IsHot { get; set; }

        [DataColumn] private string IsColor { get; set; }

        [DataColumn] public string LinkUrl { get; set; }

        [DataColumn] public DateTime? AddDate { get; set; }

        [DataColumn(Text = true, Extend = true)]
        public string SettingsXml { get; set; }

        public string CheckUserName { get; set; } //审核者

        public DateTime? CheckDate { get; set; } //审核时间

        public string CheckReasons { get; set; } //审核原因

        public string TranslateContentType { get; set; } //转移内容类型
    }

    //   [JsonConverter(typeof(ContentConverter))]
    //   public class Content : AttributesImpl, IContent
    //{
    //       public Content()
    //       {

    //       }

    //       public Content(Dictionary<string, object> dict) : base(dict)
    //    {

    //    }

    //       public Content(Content content)
    //    {
    //        Load(content);
    //       }

    //       public int Id
    //	{
    //           get => GetInt(ContentAttribute.Id);
    //           set => Set(ContentAttribute.Id, value);
    //       }

    //       public int ChannelId
    //       {
    //           get => GetInt(ContentAttribute.ChannelId);
    //           set => Set(ContentAttribute.ChannelId, value);
    //       }

    //       public int SiteId
    //       {
    //           get => GetInt(ContentAttribute.SiteId);
    //           set => Set(ContentAttribute.SiteId, value);
    //       }

    //       public string AddUserName
    //	{
    //           get => GetString(ContentAttribute.AddUserName);
    //           set => Set(ContentAttribute.AddUserName, value);
    //       }

    //       public string LastEditUserName
    //	{
    //           get => GetString(ContentAttribute.LastEditUserName);
    //           set => Set(ContentAttribute.LastEditUserName, value);
    //       }

    //    public DateTime? LastEditDate
    //    {
    //        get => GetDateTime(ContentAttribute.LastEditDate, DateTime.Now);
    //        set => Set(ContentAttribute.LastEditDate, value);
    //    }

    //    public int AdminId
    //    {
    //        get => GetInt(ContentAttribute.AdminId);
    //        set => Set(ContentAttribute.AdminId, value);
    //    }

    //    public int UserId
    //    {
    //        get => GetInt(ContentAttribute.UserId);
    //        set => Set(ContentAttribute.UserId, value);
    //    }

    //       public int Taxis
    //       {
    //           get => GetInt(ContentAttribute.Taxis);
    //           set => Set(ContentAttribute.Taxis, value);
    //       }

    //    public bool Color { get; set; }

    //    public string GroupNameCollection
    //       {
    //           get => GetString(ContentAttribute.GroupNameCollection);
    //           set => Set(ContentAttribute.GroupNameCollection, value);
    //       }

    //       public string Tags
    //       {
    //           get => GetString(ContentAttribute.Tags);
    //           set => Set(ContentAttribute.Tags, value);
    //       }

    //       public int SourceId
    //       {
    //           get => GetInt(ContentAttribute.SourceId);
    //           set => Set(ContentAttribute.SourceId, value);
    //       }

    //       public int ReferenceId
    //       {
    //           get => GetInt(ContentAttribute.ReferenceId);
    //           set => Set(ContentAttribute.ReferenceId, value);
    //       }

    //       public bool Checked
    //       {
    //           get => GetBool(ContentAttribute.Checked);
    //           set => Set(ContentAttribute.Checked, value);
    //       }

    //       public int CheckedLevel
    //	{
    //           get => GetInt(ContentAttribute.CheckedLevel);
    //           set => Set(ContentAttribute.CheckedLevel, value);
    //       }

    //       public int Hits
    //       {
    //           get => GetInt(ContentAttribute.Hits);
    //           set => Set(ContentAttribute.Hits, value);
    //       }

    //       public int HitsByDay
    //       {
    //           get => GetInt(ContentAttribute.HitsByDay);
    //           set => Set(ContentAttribute.HitsByDay, value);
    //       }

    //       public int HitsByWeek
    //       {
    //           get => GetInt(ContentAttribute.HitsByWeek);
    //           set => Set(ContentAttribute.HitsByWeek, value);
    //       }

    //       public int HitsByMonth
    //       {
    //           get => GetInt(ContentAttribute.HitsByMonth);
    //           set => Set(ContentAttribute.HitsByMonth, value);
    //       }

    //    public DateTime? LastHitsDate
    //       {
    //           get => GetDateTime(ContentAttribute.LastHitsDate, DateTime.Now);
    //           set => Set(ContentAttribute.LastHitsDate, value);
    //       }

    //    public int Downloads
    //       {
    //        get => GetInt(ContentAttribute.Downloads);
    //        set => Set(ContentAttribute.Downloads, value);
    //    }

    //       public string Title
    //	{
    //           get => GetString(ContentAttribute.Title);
    //           set => Set(ContentAttribute.Title, value);
    //       }

    //    public bool Top { get; set; }
    //    public bool Recommend { get; set; }
    //    public bool Hot { get; set; }

    //    public bool IsTop
    //       {
    //           get => GetBool(ContentAttribute.IsTop);
    //           set => Set(ContentAttribute.IsTop, value);
    //       }

    //       public bool IsRecommend
    //       {
    //           get => GetBool(ContentAttribute.IsRecommend);
    //           set => Set(ContentAttribute.IsRecommend, value);
    //       }

    //       public bool IsHot
    //       {
    //           get => GetBool(ContentAttribute.IsHot);
    //           set => Set(ContentAttribute.IsHot, value);
    //       }

    //       public bool IsColor
    //       {
    //           get => GetBool(ContentAttribute.IsColor);
    //           set => Set(ContentAttribute.IsColor, value);
    //       }

    //       public DateTime? AddDate
    //       {
    //           get => GetDateTime(ContentAttribute.AddDate, DateTime.Now);
    //           set => Set(ContentAttribute.AddDate, value);
    //       }

    //       public string LinkUrl
    //       {
    //           get => GetString(ContentAttribute.LinkUrl);
    //           set => Set(ContentAttribute.LinkUrl, value);
    //       }

    //    public string SubTitle
    //    {
    //        get => GetString(ContentAttribute.SubTitle);
    //        set => Set(ContentAttribute.SubTitle, value);
    //    }

    //    public string ImageUrl
    //    {
    //        get => GetString(ContentAttribute.ImageUrl);
    //        set => Set(ContentAttribute.ImageUrl, value);
    //    }

    //    public string VideoUrl
    //    {
    //        get => GetString(ContentAttribute.VideoUrl);
    //        set => Set(ContentAttribute.VideoUrl, value);
    //    }

    //    public string FileUrl
    //    {
    //        get => GetString(ContentAttribute.FileUrl);
    //        set => Set(ContentAttribute.FileUrl, value);
    //    }

    //    public string Author
    //    {
    //        get => GetString(ContentAttribute.Author);
    //        set => Set(ContentAttribute.Author, value);
    //    }

    //    public string Source
    //    {
    //        get => GetString(ContentAttribute.Source);
    //        set => Set(ContentAttribute.Source, value);
    //    }

    //    public string Summary
    //    {
    //        get => GetString(ContentAttribute.Summary);
    //        set => Set(ContentAttribute.Summary, value);
    //    }

    //    public string Body
    //    {
    //        get => GetString(ContentAttribute.Content);
    //        set => Set(ContentAttribute.Content, value);
    //    }

    //       public T Get<T>(string name)
    //       {
    //           return TranslateUtils.Cast<T>(Get(name));
    //       }

    //       public override Dictionary<string, object> ToDictionary()
    //    {
    //        var dict = base.ToDictionary();
    //        //dict.Remove(nameof(SettingsXml));

    //           var site = DataProvider.SiteDao.GetAsync(SiteId).GetAwaiter().GetResult();
    //        var channelInfo = ChannelManager.GetChannelAsync(SiteId, ChannelId).GetAwaiter().GetResult();
    //        var styleInfoList = TableStyleManager.GetContentStyleListAsync(site, channelInfo).GetAwaiter().GetResult();

    //        foreach (var styleInfo in styleInfoList)
    //        {
    //            if (styleInfo.InputType == InputType.Image || styleInfo.InputType == InputType.File || styleInfo.InputType == InputType.Video)
    //            {
    //                var value = GetString(styleInfo.AttributeName);
    //                if (!string.IsNullOrEmpty(value))
    //                {
    //                    value = PageUtility.ParseNavigationUrl(site, value, false);
    //                }

    //                dict.Remove(styleInfo.AttributeName);
    //                   dict[styleInfo.AttributeName] = value;
    //               }
    //               else if (styleInfo.InputType == InputType.TextEditor)
    //            {
    //                var value = GetString(styleInfo.AttributeName);
    //                if (!string.IsNullOrEmpty(value))
    //                {
    //                    value = ContentUtility.TextEditorContentDecode(site, value, false);
    //                }
    //                dict.Remove(styleInfo.AttributeName);
    //                   dict[styleInfo.AttributeName] = value;
    //            }
    //            else
    //            {
    //                dict.Remove(styleInfo.AttributeName);
    //                   dict[styleInfo.AttributeName] = Get(styleInfo.AttributeName);
    //               }
    //        }

    //        foreach (var attributeName in ContentAttribute.AllAttributes.Value)
    //        {
    //            if (StringUtils.StartsWith(attributeName, "Is"))
    //            {
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = GetBool(attributeName);
    //               }
    //            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Title))
    //            {
    //                var value = GetString(ContentAttribute.Title);
    //                if (site.IsContentTitleBreakLine)
    //                {
    //                    value = value.Replace("  ", "<br />");
    //                }
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = value;
    //               }
    //               else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.SubTitle))
    //               {
    //                   var value = GetString(ContentAttribute.SubTitle);
    //                   if (site.IsContentSubTitleBreakLine)
    //                   {
    //                       value = value.Replace("  ", "<br />");
    //                   }
    //                   dict.Remove(attributeName);
    //                   dict[attributeName] = value;
    //               }
    //               else
    //            {
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = Get(attributeName);
    //               }
    //           }

    //        foreach (var attributeName in ContentAttribute.IncludedAttributes.Value)
    //        {
    //            if (attributeName == ContentAttribute.NavigationUrl)
    //            {
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = PageUtility.GetContentUrlAsync(site, this, false);
    //            }
    //            else if (attributeName == ContentAttribute.CheckState)
    //            {
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = CheckManager.GetCheckState(site, this);
    //            }
    //            else
    //            {
    //                dict.Remove(attributeName);
    //                   dict[attributeName] = Get(attributeName);
    //            }
    //        }

    //           return dict;
    //    }

    //    private class ContentConverter : JsonConverter
    //    {
    //        public override bool CanConvert(Type objectType)
    //        {
    //            return objectType == typeof(AttributesImpl);
    //        }

    //        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //        {
    //            var attributes = value as AttributesImpl;
    //            serializer.Serialize(writer, attributes?.ToDictionary());
    //        }

    //        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
    //            JsonSerializer serializer)
    //        {
    //            var value = (string)reader.Value;
    //            if (string.IsNullOrEmpty(value)) return null;
    //               var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(value);
    //            var content = new Content(dict);

    //               return content;
    //        }
    //    }
    //   }
}
