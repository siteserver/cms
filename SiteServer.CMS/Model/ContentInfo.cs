using System;
using System.Data;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Model
{
    public class ContentInfo : ExtendedAttributes, IContentInfo
	{
		public ContentInfo()
		{
			Id = 0;
            ChannelId = 0;
			SiteId = 0;
			AddUserName = string.Empty;
			LastEditUserName = string.Empty;
            WritingUserName = string.Empty;
            LastEditDate = DateTime.Now;
            Taxis = 0;
            GroupNameCollection = string.Empty;
            Tags = string.Empty;
            SourceId = 0;
            ReferenceId = 0;
			IsChecked = false;
            CheckedLevel = 0;
            Hits = 0;
            HitsByDay = 0;
            HitsByWeek = 0;
            HitsByMonth = 0;
            LastHitsDate = DateTime.Now;
            Title = string.Empty;
            IsTop = false;
            IsRecommend = false;
            IsHot = false;
            IsColor = false;
		    LinkUrl = string.Empty;
            AddDate = DateTime.Now;
		}

        public ContentInfo(object dataItem) : base(dataItem)
        {
            Load(SettingsXml);
        }

        public ContentInfo(IDataReader rdr) : base(rdr)
        {
            Load(SettingsXml);
        }

        public int Id
		{
            get { return GetInt(ContentAttribute.Id); }
            set { Set(ContentAttribute.Id, value.ToString()); }
		}

        public int ChannelId
        {
            get { return GetInt(ContentAttribute.ChannelId); }
            set { Set(ContentAttribute.ChannelId, value.ToString()); }
        }

        public int SiteId
        {
            get { return GetInt(ContentAttribute.SiteId); }
            set { Set(ContentAttribute.SiteId, value.ToString()); }
		}

        public string AddUserName
		{
            get { return GetString(ContentAttribute.AddUserName); }
            set { Set(ContentAttribute.AddUserName, value); }
		}

        public string LastEditUserName
		{
            get { return GetString(ContentAttribute.LastEditUserName); }
            set { Set(ContentAttribute.LastEditUserName, value); }
		}

        public string WritingUserName
        {
            get { return GetString(ContentAttribute.WritingUserName); }
            set { Set(ContentAttribute.WritingUserName, value); }
        }

        public DateTime LastEditDate
		{
            get { return GetDateTime(ContentAttribute.LastEditDate, DateTime.Now); }
            set { Set(ContentAttribute.LastEditDate, DateUtils.GetDateAndTimeString(value)); }
		}

        public int Taxis
        {
            get { return GetInt(ContentAttribute.Taxis); }
            set { Set(ContentAttribute.Taxis, value.ToString()); }
        }

        public string GroupNameCollection
        {
            get { return GetString(ContentAttribute.GroupNameCollection); }
            set { Set(ContentAttribute.GroupNameCollection, value); }
		}

        public string Tags
        {
            get { return GetString(ContentAttribute.Tags); }
            set { Set(ContentAttribute.Tags, value); }
        }

        public int SourceId
        {
            get { return GetInt(ContentAttribute.SourceId); }
            set { Set(ContentAttribute.SourceId, value.ToString()); }
        }

        public int ReferenceId
        {
            get { return GetInt(ContentAttribute.ReferenceId); }
            set { Set(ContentAttribute.ReferenceId, value.ToString()); }
        }

        public bool IsChecked
		{
            get { return GetBool(ContentAttribute.IsChecked); }
            set { Set(ContentAttribute.IsChecked, value.ToString()); }
		}

        public int CheckedLevel
		{
            get { return GetInt(ContentAttribute.CheckedLevel); }
            set { Set(ContentAttribute.CheckedLevel, value.ToString()); }
		}

        public int Hits
        {
            get { return GetInt(ContentAttribute.Hits); }
            set { Set(ContentAttribute.Hits, value.ToString()); }
        }

        public int HitsByDay
        {
            get { return GetInt(ContentAttribute.HitsByDay); }
            set { Set(ContentAttribute.HitsByDay, value.ToString()); }
        }

        public int HitsByWeek
        {
            get { return GetInt(ContentAttribute.HitsByWeek); }
            set { Set(ContentAttribute.HitsByWeek, value.ToString()); }
        }

        public int HitsByMonth
        {
            get { return GetInt(ContentAttribute.HitsByMonth); }
            set { Set(ContentAttribute.HitsByMonth, value.ToString()); }
        }

        public DateTime LastHitsDate
        {
            get { return GetDateTime(ContentAttribute.LastHitsDate, DateTime.Now); }
            set { Set(ContentAttribute.LastHitsDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string Title
		{
            get { return GetString(ContentAttribute.Title); }
            set { Set(ContentAttribute.Title, value); }
		}
      
        public bool IsTop
        {
            get { return GetBool(ContentAttribute.IsTop); }
            set { Set(ContentAttribute.IsTop, value.ToString()); }
        }

        public bool IsRecommend
        {
            get { return GetBool(ContentAttribute.IsRecommend); }
            set { Set(ContentAttribute.IsRecommend, value.ToString()); }
        }

        public bool IsHot
        {
            get { return GetBool(ContentAttribute.IsHot); }
            set { Set(ContentAttribute.IsHot, value.ToString()); }
        }

        public bool IsColor
        {
            get { return GetBool(ContentAttribute.IsColor); }
            set { Set(ContentAttribute.IsColor, value.ToString()); }
        }

        public DateTime AddDate
        {
            get { return GetDateTime(ContentAttribute.AddDate, DateTime.Now); }
            set { Set(ContentAttribute.AddDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string LinkUrl
        {
            get { return GetString(ContentAttribute.LinkUrl); }
            set { Set(ContentAttribute.LinkUrl, value); }
        }

        public string SettingsXml
        {
            get { return GetString(ContentAttribute.SettingsXml); }
            set { Set(ContentAttribute.SettingsXml, value); }
        }
	}
}
