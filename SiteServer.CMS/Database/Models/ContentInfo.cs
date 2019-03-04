using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Models
{
    public class ContentInfo : DynamicEntity, IContentInfo
    {
        public ContentInfo() { }

        public ContentInfo(IDictionary<string, object> dict) : base(dict)
        {

        }

        //public ContentInfo(IDictionary<string, object> dict)
        //{
        //    foreach (var o in dict)
        //    {
        //        var name = o.Key;
        //        var value = o.Value;

        //        if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Id))
        //        {
        //            Id = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Guid))
        //        {
        //            Guid = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.LastModifiedDate))
        //        {
        //            LastModifiedDate = TranslateUtils.Get<DateTime?>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.ChannelId))
        //        {
        //            ChannelId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.SiteId))
        //        {
        //            SiteId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.AddUserName))
        //        {
        //            AddUserName = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.LastEditUserName))
        //        {
        //            LastEditUserName = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.LastEditDate))
        //        {
        //            LastEditDate = TranslateUtils.Get<DateTime?>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.AdminId))
        //        {
        //            AdminId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.UserId))
        //        {
        //            UserId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Taxis))
        //        {
        //            Taxis = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.GroupNameCollection))
        //        {
        //            GroupNameCollection = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Tags))
        //        {
        //            Tags = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.SourceId))
        //        {
        //            SourceId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.ReferenceId))
        //        {
        //            ReferenceId = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.IsChecked))
        //        {
        //            IsChecked = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, nameof(Checked)))
        //        {
        //            Checked = TranslateUtils.Get<bool>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.CheckedLevel))
        //        {
        //            CheckedLevel = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Hits))
        //        {
        //            Hits = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.HitsByDay))
        //        {
        //            HitsByDay = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.HitsByWeek))
        //        {
        //            HitsByWeek = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.HitsByMonth))
        //        {
        //            HitsByMonth = TranslateUtils.Get<int>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.LastHitsDate))
        //        {
        //            LastHitsDate = TranslateUtils.Get<DateTime?>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Title))
        //        {
        //            Title = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.IsTop))
        //        {
        //            IsTop = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, nameof(Top)))
        //        {
        //            Top = TranslateUtils.Get<bool>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.IsRecommend))
        //        {
        //            IsRecommend = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, nameof(Recommend)))
        //        {
        //            Recommend = TranslateUtils.Get<bool>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.IsHot))
        //        {
        //            IsHot = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, nameof(Hot)))
        //        {
        //            Hot = TranslateUtils.Get<bool>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.IsColor))
        //        {
        //            IsColor = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, nameof(Color)))
        //        {
        //            Color = TranslateUtils.Get<bool>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.AddDate))
        //        {
        //            AddDate = TranslateUtils.Get<DateTime?>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.LinkUrl))
        //        {
        //            LinkUrl = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.SubTitle))
        //        {
        //            SubTitle = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.ImageUrl))
        //        {
        //            ImageUrl = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.VideoUrl))
        //        {
        //            VideoUrl = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.FileUrl))
        //        {
        //            FileUrl = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Author))
        //        {
        //            Author = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Source))
        //        {
        //            Source = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Summary))
        //        {
        //            Summary = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.Content))
        //        {
        //            Content = TranslateUtils.Get<string>(value);
        //        }
        //        else if (StringUtils.EqualsIgnoreCase(name, ContentAttribute.SettingsXml))
        //        {
        //            SettingsXml = TranslateUtils.Get<string>(value);
        //        }
        //        else
        //        {
        //            Set(name, value);
        //        }
        //    }
        //}

        //public IDictionary<string, object> ToDictionary()
        //{
        //    var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        //    foreach (var o in this)
        //    {
        //        dict[o.Key] = o.Value;
        //    }

        //    dict[ContentAttribute.Id] = Id;
        //    dict[ContentAttribute.Guid] = Guid;
        //    dict[ContentAttribute.LastModifiedDate] = LastModifiedDate;
        //    dict[ContentAttribute.ChannelId] = ChannelId;
        //    dict[ContentAttribute.SiteId] = SiteId;
        //    dict[ContentAttribute.AddUserName] = AddUserName;
        //    dict[ContentAttribute.LastEditUserName] = LastEditUserName;
        //    dict[ContentAttribute.LastEditDate] = LastEditDate;
        //    dict[ContentAttribute.AdminId] = AdminId;
        //    dict[ContentAttribute.UserId] = UserId;
        //    dict[ContentAttribute.Taxis] = Taxis;
        //    dict[ContentAttribute.GroupNameCollection] = GroupNameCollection;
        //    dict[ContentAttribute.Tags] = Tags;
        //    dict[ContentAttribute.SourceId] = SourceId;
        //    dict[ContentAttribute.ReferenceId] = ReferenceId;
        //    dict[nameof(Checked)] = Checked;
        //    dict[ContentAttribute.CheckedLevel] = CheckedLevel;
        //    dict[ContentAttribute.Hits] = Hits;
        //    dict[ContentAttribute.HitsByDay] = HitsByDay;
        //    dict[ContentAttribute.HitsByWeek] = HitsByWeek;
        //    dict[ContentAttribute.HitsByMonth] = HitsByMonth;
        //    dict[ContentAttribute.LastHitsDate] = LastHitsDate;
        //    dict[ContentAttribute.Title] = Title;
        //    dict[nameof(Top)] = Top;
        //    dict[nameof(Recommend)] = Recommend;
        //    dict[nameof(Hot)] = Hot;
        //    dict[nameof(Color)] = Color;
        //    dict[ContentAttribute.AddDate] = AddDate;
        //    dict[ContentAttribute.LinkUrl] = LinkUrl;
        //    dict[ContentAttribute.SubTitle] = SubTitle;
        //    dict[ContentAttribute.ImageUrl] = ImageUrl;
        //    dict[ContentAttribute.VideoUrl] = VideoUrl;
        //    dict[ContentAttribute.FileUrl] = FileUrl;
        //    dict[ContentAttribute.Author] = Author;
        //    dict[ContentAttribute.Source] = Source;
        //    dict[ContentAttribute.Summary] = Summary;
        //    dict[ContentAttribute.Content] = Content;

        //    return dict;
        //}

        [TableColumn]
        public int ChannelId { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string AddUserName { get; set; }

        [TableColumn]
        public string LastEditUserName { get; set; }

        [TableColumn]
        public DateTime? LastEditDate { get; set; }

        [TableColumn]
        public int AdminId { get; set; }

        [TableColumn]
        public int UserId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }

        [TableColumn]
        public string GroupNameCollection { get; set; }

        [TableColumn]
        public string Tags { get; set; }

        [TableColumn]
        public int SourceId { get; set; }

        [TableColumn]
        public int ReferenceId { get; set; }

        [TableColumn]
        private string IsChecked { get; set; }

        public bool Checked
        {
            get => IsChecked == "True";
            set => IsChecked = value.ToString();
        }

        [TableColumn]
        public int CheckedLevel { get; set; }

        [TableColumn]
        public int Hits { get; set; }

        [TableColumn]
        public int HitsByDay { get; set; }

        [TableColumn]
        public int HitsByWeek { get; set; }

        [TableColumn]
        public int HitsByMonth { get; set; }

        [TableColumn]
        public DateTime? LastHitsDate { get; set; }

        [TableColumn]
        public string Title { get; set; }

        [TableColumn]
        private string IsTop { get; set; }

        public bool Top
        {
            get => IsTop == "True";
            set => IsTop = value.ToString();
        }

        [TableColumn]
        private string IsRecommend { get; set; }

        public bool Recommend
        {
            get => IsRecommend == "True";
            set => IsRecommend = value.ToString();
        }

        [TableColumn]
        private string IsHot { get; set; }

        public bool Hot
        {
            get => IsHot == "True";
            set => IsHot = value.ToString();
        }

        [TableColumn]
        private string IsColor { get; set; }

        public bool Color
        {
            get => IsColor == "True";
            set => IsColor = value.ToString();
        }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn]
        public string LinkUrl { get; set; }

        [TableColumn]
        public string SubTitle { get; set; }

        [TableColumn]
        public string ImageUrl { get; set; }

        [TableColumn]
        public string VideoUrl { get; set; }

        [TableColumn]
        public string FileUrl { get; set; }

        [TableColumn]
        public string Author { get; set; }

        [TableColumn]
        public string Source { get; set; }

        [TableColumn(Text = true)]
        public string Summary { get; set; }

        [TableColumn(Text = true)]
        public string Content { get; set; }

        [TableColumn(Text = true, Extend = true)]
        public string SettingsXml { get; set; }

        //   public override Dictionary<string, object> ToDictionary()
        //{
        //    var dict = base.ToDictionary();
        //    //dict.Remove(nameof(SettingsXml));

        //       var siteInfo = SiteManager.GetSiteInfo(SiteId);
        //    var channelInfo = ChannelManager.GetChannelInfo(SiteId, ChannelId);
        //    var styleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);

        //    foreach (var styleInfo in styleInfoList)
        //    {
        //        if (styleInfo.Type == InputType.Image || styleInfo.Type == InputType.File || styleInfo.Type == InputType.Video)
        //        {
        //            var value = GetString(styleInfo.AttributeName);
        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                value = PageUtility.ParseNavigationUrl(siteInfo, value, false);
        //            }

        //            dict.Remove(styleInfo.AttributeName);
        //               dict[styleInfo.AttributeName] = value;
        //           }
        //           else if (styleInfo.Type == InputType.TextEditor)
        //        {
        //            var value = GetString(styleInfo.AttributeName);
        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                value = ContentUtility.TextEditorContentDecode(siteInfo, value, false);
        //            }
        //            dict.Remove(styleInfo.AttributeName);
        //               dict[styleInfo.AttributeName] = value;
        //        }
        //        else
        //        {
        //            dict.Remove(styleInfo.AttributeName);
        //               dict[styleInfo.AttributeName] = Get(styleInfo.AttributeName);
        //           }
        //    }

        //    foreach (var attributeName in ContentAttribute.AllAttributes.Value)
        //    {
        //        if (StringUtils.StartsWith(attributeName, "Is"))
        //        {
        //            dict.Remove(attributeName);
        //               dict[attributeName] = GetBool(attributeName);
        //           }
        //        else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Title))
        //        {
        //            var value = GetString(ContentAttribute.Title);
        //            if (siteInfo.IsContentTitleBreakLine)
        //            {
        //                value = value.Replace("  ", "<br />");
        //            }
        //            dict.Remove(attributeName);
        //               dict[attributeName] = value;
        //           }
        //           else
        //        {
        //            dict.Remove(attributeName);
        //               dict[attributeName] = Get(attributeName);
        //           }
        //       }

        //    foreach (var attributeName in ContentAttribute.IncludedAttributes.Value)
        //    {
        //        if (attributeName == ContentAttribute.NavigationUrl)
        //        {
        //            dict.Remove(attributeName);
        //               dict[attributeName] = PageUtility.GetContentUrl(siteInfo, this, false);
        //        }
        //        else if (attributeName == ContentAttribute.CheckState)
        //        {
        //            dict.Remove(attributeName);
        //               dict[attributeName] = CheckManager.GetCheckState(siteInfo, this);
        //        }
        //        else
        //        {
        //            dict.Remove(attributeName);
        //               dict[attributeName] = Get(attributeName);
        //        }
        //    }

        //    foreach (var attributeName in ContentAttribute.ExcludedAttributes.Value)
        //    {
        //        dict.Remove(attributeName);
        //       }

        //    return dict;
        //}

        //private class ContentConverter : JsonConverter
        //{
        //    public override bool CanConvert(Type objectType)
        //    {
        //        return objectType == typeof(IAttributes);
        //    }

        //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        //    {
        //        var attributes = value as IAttributes;
        //        serializer.Serialize(writer, attributes?.ToDictionary());
        //    }

        //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        //        JsonSerializer serializer)
        //    {
        //        var value = (string)reader.Value;
        //        if (string.IsNullOrEmpty(value)) return null;
        //           var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(value);
        //        var content = new ContentInfo(dict);

        //           return content;
        //    }
        //}
    }
}
