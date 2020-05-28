using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    [JsonConverter(typeof(ContentConverter))]
    public class ContentInfo : AttributesImpl, IContentInfo
	{
        public ContentInfo()
        {

        }

        public ContentInfo(IDataReader rdr) : base(rdr)
        {

        }

        public ContentInfo(IDataRecord record) : base(record)
        {

        }

        public ContentInfo(DataRowView view) : base(view)
        {

        }

        public ContentInfo(DataRow row) : base(row)
        {

        }

	    public ContentInfo(Dictionary<string, object> dict) : base(dict)
	    {

	    }

	    public ContentInfo(NameValueCollection nvc) : base(nvc)
        {

        }

	    public ContentInfo(object anonymous) : base(anonymous)
	    {

	    }

	    public ContentInfo(ContentInfo contentInfo)
	    {
	        Load(contentInfo);
        }

        public int Id
		{
            get => GetInt(ContentAttribute.Id);
            set => Set(ContentAttribute.Id, value);
        }

        public int ChannelId
        {
            get => GetInt(ContentAttribute.ChannelId);
            set => Set(ContentAttribute.ChannelId, value);
        }

        public int SiteId
        {
            get => GetInt(ContentAttribute.SiteId);
            set => Set(ContentAttribute.SiteId, value);
        }

        public string AddUserName
		{
            get => GetString(ContentAttribute.AddUserName);
            set => Set(ContentAttribute.AddUserName, value);
        }

        public string LastEditUserName
		{
            get => GetString(ContentAttribute.LastEditUserName);
            set => Set(ContentAttribute.LastEditUserName, value);
        }

	    public DateTime? LastEditDate
	    {
	        get => GetDateTime(ContentAttribute.LastEditDate, DateTime.Now);
	        set => Set(ContentAttribute.LastEditDate, value);
	    }

	    public int AdminId
	    {
	        get => GetInt(ContentAttribute.AdminId);
	        set => Set(ContentAttribute.AdminId, value);
	    }

	    public int UserId
	    {
	        get => GetInt(ContentAttribute.UserId);
	        set => Set(ContentAttribute.UserId, value);
	    }

        public int Taxis
        {
            get => GetInt(ContentAttribute.Taxis);
            set => Set(ContentAttribute.Taxis, value);
        }

	    public bool Color { get; set; }

	    public string GroupNameCollection
        {
            get => GetString(ContentAttribute.GroupNameCollection);
            set => Set(ContentAttribute.GroupNameCollection, value);
        }

        public string Tags
        {
            get => GetString(ContentAttribute.Tags);
            set => Set(ContentAttribute.Tags, value);
        }

	    public bool Checked { get; set; }

	    public int SourceId
        {
            get => GetInt(ContentAttribute.SourceId);
            set => Set(ContentAttribute.SourceId, value);
        }

        public int ReferenceId
        {
            get => GetInt(ContentAttribute.ReferenceId);
            set => Set(ContentAttribute.ReferenceId, value);
        }

        public bool IsChecked
		{
            get => GetBool(ContentAttribute.IsChecked);
            set => Set(ContentAttribute.IsChecked, value);
        }

        public int CheckedLevel
		{
            get => GetInt(ContentAttribute.CheckedLevel);
            set => Set(ContentAttribute.CheckedLevel, value);
        }

        public int Hits
        {
            get => GetInt(ContentAttribute.Hits);
            set => Set(ContentAttribute.Hits, value);
        }

        public int HitsByDay
        {
            get => GetInt(ContentAttribute.HitsByDay);
            set => Set(ContentAttribute.HitsByDay, value);
        }

        public int HitsByWeek
        {
            get => GetInt(ContentAttribute.HitsByWeek);
            set => Set(ContentAttribute.HitsByWeek, value);
        }

        public int HitsByMonth
        {
            get => GetInt(ContentAttribute.HitsByMonth);
            set => Set(ContentAttribute.HitsByMonth, value);
        }

	    public DateTime? LastHitsDate
        {
            get => GetDateTime(ContentAttribute.LastHitsDate, DateTime.Now);
            set => Set(ContentAttribute.LastHitsDate, value);
        }

	    public int Downloads
        {
	        get => GetInt(ContentAttribute.Downloads);
	        set => Set(ContentAttribute.Downloads, value);
	    }

        public string Title
		{
            get => GetString(ContentAttribute.Title);
            set => Set(ContentAttribute.Title, value);
        }

	    public bool Top { get; set; }
	    public bool Recommend { get; set; }
	    public bool Hot { get; set; }

	    public bool IsTop
        {
            get => GetBool(ContentAttribute.IsTop);
            set => Set(ContentAttribute.IsTop, value);
        }

        public bool IsRecommend
        {
            get => GetBool(ContentAttribute.IsRecommend);
            set => Set(ContentAttribute.IsRecommend, value);
        }

        public bool IsHot
        {
            get => GetBool(ContentAttribute.IsHot);
            set => Set(ContentAttribute.IsHot, value);
        }

        public bool IsColor
        {
            get => GetBool(ContentAttribute.IsColor);
            set => Set(ContentAttribute.IsColor, value);
        }

        public DateTime? AddDate
        {
            get => GetDateTime(ContentAttribute.AddDate, DateTime.Now);
            set => Set(ContentAttribute.AddDate, value);
        }

        public string LinkUrl
        {
            get => GetString(ContentAttribute.LinkUrl);
            set => Set(ContentAttribute.LinkUrl, value);
        }

	    public string SubTitle
	    {
	        get => GetString(BackgroundContentAttribute.SubTitle);
	        set => Set(BackgroundContentAttribute.SubTitle, value);
	    }

	    public string ImageUrl
	    {
	        get => GetString(BackgroundContentAttribute.ImageUrl);
	        set => Set(BackgroundContentAttribute.ImageUrl, value);
	    }

	    public string VideoUrl
	    {
	        get => GetString(BackgroundContentAttribute.VideoUrl);
	        set => Set(BackgroundContentAttribute.VideoUrl, value);
	    }

	    public string FileUrl
	    {
	        get => GetString(BackgroundContentAttribute.FileUrl);
	        set => Set(BackgroundContentAttribute.FileUrl, value);
	    }

	    public string Author
	    {
	        get => GetString(BackgroundContentAttribute.Author);
	        set => Set(BackgroundContentAttribute.Author, value);
	    }

	    public string Source
	    {
	        get => GetString(BackgroundContentAttribute.Source);
	        set => Set(BackgroundContentAttribute.Source, value);
	    }

	    public string Summary
	    {
	        get => GetString(BackgroundContentAttribute.Summary);
	        set => Set(BackgroundContentAttribute.Summary, value);
	    }

	    public string Content
	    {
	        get => GetString(BackgroundContentAttribute.Content);
	        set => Set(BackgroundContentAttribute.Content, value);
	    }

        public string SettingsXml
        {
            get => GetString(ContentAttribute.SettingsXml);
            set => Set(ContentAttribute.SettingsXml, value);
        }

        public T Get<T>(string name)
        {
            return TranslateUtils.Cast<T>(Get(name));
        }

        public override Dictionary<string, object> ToDictionary()
	    {
	        var dict = base.ToDictionary();
	        //dict.Remove(nameof(SettingsXml));

            var siteInfo = SiteManager.GetSiteInfo(SiteId);
	        var channelInfo = ChannelManager.GetChannelInfo(SiteId, ChannelId);
	        var styleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);

            if (siteInfo == null || channelInfo == null || styleInfoList == null) return dict;

            foreach (var styleInfo in styleInfoList)
	        {
	            if (styleInfo.InputType == InputType.Image || styleInfo.InputType == InputType.File || styleInfo.InputType == InputType.Video)
	            {
	                var value = GetString(styleInfo.AttributeName);
	                if (!string.IsNullOrEmpty(value))
	                {
	                    value = PageUtility.ParseNavigationUrl(siteInfo, value, false);
	                }

                    dict[styleInfo.AttributeName] = value;

                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(styleInfo.AttributeName);
                    dict.Remove(extendAttributeName);

                    var extendValues = GetString(extendAttributeName);
                    dict[$"{styleInfo.AttributeName}Extends"] = 0;
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        var no = 0;
                        var extends = TranslateUtils.StringCollectionToStringList(extendValues);
                        foreach (var extend in extends)
                        {
                            dict[$"{styleInfo.AttributeName}Extend{++no}"] = PageUtility.ParseNavigationUrl(siteInfo, extend, false);
                        }
                        dict[$"{styleInfo.AttributeName}Extends"] = no;
                    }
                }
                else if (styleInfo.InputType == InputType.TextEditor)
	            {
	                var value = GetString(styleInfo.AttributeName);
	                if (!string.IsNullOrEmpty(value))
	                {
	                    value = ContentUtility.TextEditorContentDecode(siteInfo, value, false);
	                }
                    dict[styleInfo.AttributeName] = value;
	            }
	            else
	            {
                    dict[styleInfo.AttributeName] = Get(styleInfo.AttributeName);
                }
	        }

	        foreach (var attributeName in ContentAttribute.AllAttributes.Value)
	        {
	            if (StringUtils.StartsWith(attributeName, "Is"))
	            {
	                dict.Remove(attributeName);
                    dict[attributeName] = GetBool(attributeName);
                }
	            else if (StringUtils.EqualsIgnoreCase(attributeName, ContentAttribute.Title))
	            {
	                var value = GetString(ContentAttribute.Title);
	                if (siteInfo.Additional.IsContentTitleBreakLine)
	                {
	                    value = value.Replace("  ", "<br />");
	                }
	                dict.Remove(attributeName);
                    dict[attributeName] = value;
                }
                else if (StringUtils.EqualsIgnoreCase(attributeName, BackgroundContentAttribute.SubTitle))
                {
                    var value = GetString(BackgroundContentAttribute.SubTitle);
                    if (siteInfo.Additional.IsContentSubTitleBreakLine)
                    {
                        value = value.Replace("  ", "<br />");
                    }
                    dict.Remove(attributeName);
                    dict[attributeName] = value;
                }
                else
	            {
	                dict.Remove(attributeName);
                    dict[attributeName] = Get(attributeName);
                }
            }

	        foreach (var attributeName in ContentAttribute.IncludedAttributes.Value)
	        {
	            if (attributeName == ContentAttribute.NavigationUrl)
	            {
	                dict.Remove(attributeName);
                    dict[attributeName] = PageUtility.GetContentUrl(siteInfo, this, false);
	            }
	            else if (attributeName == ContentAttribute.CheckState)
	            {
	                dict.Remove(attributeName);
                    dict[attributeName] = CheckManager.GetCheckState(siteInfo, this);
	            }
	            else
	            {
	                dict.Remove(attributeName);
                    dict[attributeName] = Get(attributeName);
	            }
	        }

	        foreach (var attributeName in ContentAttribute.ExcludedAttributes.Value)
	        {
	            dict.Remove(attributeName);
            }

	        return dict;
	    }

	    private class ContentConverter : JsonConverter
	    {
	        public override bool CanConvert(Type objectType)
	        {
	            return objectType == typeof(AttributesImpl);
	        }

	        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	        {
	            var attributes = value as AttributesImpl;
	            serializer.Serialize(writer, attributes?.ToDictionary());
	        }

	        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
	            JsonSerializer serializer)
	        {
	            var value = (string)reader.Value;
	            if (string.IsNullOrEmpty(value)) return null;
                var dict = TranslateUtils.JsonDeserialize<Dictionary<string, object>>(value);
	            var content = new ContentInfo(dict);

                return content;
	        }
	    }
    }
}
