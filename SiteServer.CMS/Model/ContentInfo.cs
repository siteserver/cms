using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public class ContentInfo : ExtendedAttributes, IContentInfo
	{
		public ContentInfo()
		{
			
		}

        public ContentInfo(IDataReader rdr) : base(rdr)
        {
            Load(SettingsXml);
        }

	    public ContentInfo(IDataRecord record) : base(record)
	    {
	        Load(SettingsXml);
	    }

        public ContentInfo(DataRowView view) : base(view)
	    {
	        Load(SettingsXml);
	    }

	    public ContentInfo(DataRow row) : base(row)
	    {
	        Load(SettingsXml);
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

        public string WritingUserName
        {
            get => GetString(ContentAttribute.WritingUserName);
            set => Set(ContentAttribute.WritingUserName, value);
        }

        public DateTime LastEditDate
		{
            get => GetDateTime(ContentAttribute.LastEditDate, DateTime.Now);
            set => Set(ContentAttribute.LastEditDate, value);
        }

        public int Taxis
        {
            get => GetInt(ContentAttribute.Taxis);
            set => Set(ContentAttribute.Taxis, value);
        }

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

        public DateTime LastHitsDate
        {
            get => GetDateTime(ContentAttribute.LastHitsDate, DateTime.Now);
            set => Set(ContentAttribute.LastHitsDate, value);
        }

        public string Title
		{
            get => GetString(ContentAttribute.Title);
            set => Set(ContentAttribute.Title, value);
        }
      
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

        public DateTime AddDate
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

        [JsonIgnore]
        public string SettingsXml
        {
            get => GetString(ContentAttribute.SettingsXml);
            set => Set(ContentAttribute.SettingsXml, value);
        }

	    public override Dictionary<string, object> ToDictionary()
	    {
	        var dict = base.ToDictionary();

	        var siteInfo = SiteManager.GetSiteInfo(SiteId);

	        if (dict.ContainsKey(BackgroundContentAttribute.ImageUrl))
	        {
	            var imageUrl = (string)dict[BackgroundContentAttribute.ImageUrl];
	            if (!string.IsNullOrEmpty(imageUrl))
	            {
                    dict[BackgroundContentAttribute.ImageUrl] = PageUtility.ParseNavigationUrl(siteInfo, imageUrl, false);
	            }
	        }
	        if (dict.ContainsKey(BackgroundContentAttribute.FileUrl))
	        {
	            var fileUrl = (string)dict[BackgroundContentAttribute.FileUrl];
	            if (!string.IsNullOrEmpty(fileUrl))
	            {
	                dict[BackgroundContentAttribute.FileUrl] = PageUtility.ParseNavigationUrl(siteInfo, fileUrl, false);
	            }
	        }
            dict[BackgroundContentAttribute.NavigationUrl] = PageUtility.GetContentUrl(siteInfo, this, false);

            return dict;
	    }
	}
}
