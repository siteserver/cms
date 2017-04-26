using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using BaiRong.Core.Model.Attributes;

namespace BaiRong.Core.Model
{
    public class ContentInfo : ExtendedAttributes
	{
		public ContentInfo()
		{
			Id = 0;
			NodeId = 0;
			PublishmentSystemId = 0;
			AddUserName = string.Empty;
			LastEditUserName = string.Empty;
			LastEditDate = DateTime.Now;
            WritingUserName = string.Empty;
            Taxis = 0;
			ContentGroupNameCollection = string.Empty;
            Tags = string.Empty;
            SourceId = 0;
            ReferenceId = 0;
			IsChecked = false;
            CheckedLevel = 0;
            Comments = 0;
            Photos = 0;
            Hits = 0;
            HitsByDay = 0;
            HitsByWeek = 0;
            HitsByMonth = 0;
            LastHitsDate = DateTime.Now;

            Title = string.Empty;
            IsTop = false;
            AddDate = DateTime.Now;
		}

        public ContentInfo(object dataItem)
            : base(dataItem)
		{
		}

		public int Id
		{
            get { return GetInt(ContentAttribute.Id, 0); }
            set { SetExtendedAttribute(ContentAttribute.Id, value.ToString()); }
		}

        public int NodeId
        {
            get { return GetInt(ContentAttribute.NodeId, 0); }
            set { SetExtendedAttribute(ContentAttribute.NodeId, value.ToString()); }
        }

        public int PublishmentSystemId
		{
            get { return GetInt(ContentAttribute.PublishmentSystemId, 0); }
            set { SetExtendedAttribute(ContentAttribute.PublishmentSystemId, value.ToString()); }
		}

        public string AddUserName
		{
            get { return GetExtendedAttribute(ContentAttribute.AddUserName); }
            set { SetExtendedAttribute(ContentAttribute.AddUserName, value); }
		}

        public string LastEditUserName
		{
            get { return GetExtendedAttribute(ContentAttribute.LastEditUserName); }
            set { SetExtendedAttribute(ContentAttribute.LastEditUserName, value); }
		}

        public string WritingUserName
        {
            get { return GetExtendedAttribute(ContentAttribute.WritingUserName); }
            set { SetExtendedAttribute(ContentAttribute.WritingUserName, value); }
        }

        public DateTime LastEditDate
		{
            get { return GetDateTime(ContentAttribute.LastEditDate, DateTime.Now); }
            set { SetExtendedAttribute(ContentAttribute.LastEditDate, DateUtils.GetDateAndTimeString(value)); }
		}

        public int Taxis
        {
            get { return GetInt(ContentAttribute.Taxis, 0); }
            set { SetExtendedAttribute(ContentAttribute.Taxis, value.ToString()); }
        }

        public string ContentGroupNameCollection
		{
            get { return GetExtendedAttribute(ContentAttribute.ContentGroupNameCollection); }
            set { SetExtendedAttribute(ContentAttribute.ContentGroupNameCollection, value); }
		}

        public string Tags
        {
            get { return GetExtendedAttribute(ContentAttribute.Tags); }
            set { SetExtendedAttribute(ContentAttribute.Tags, value); }
        }

        public int SourceId
        {
            get { return GetInt(ContentAttribute.SourceId, 0); }
            set { SetExtendedAttribute(ContentAttribute.SourceId, value.ToString()); }
        }

        public int ReferenceId
        {
            get { return GetInt(ContentAttribute.ReferenceId, 0); }
            set { SetExtendedAttribute(ContentAttribute.ReferenceId, value.ToString()); }
        }

        public bool IsChecked
		{
            get { return GetBool(ContentAttribute.IsChecked, false); }
            set { SetExtendedAttribute(ContentAttribute.IsChecked, value.ToString()); }
		}

        public int CheckedLevel
		{
            get { return GetInt(ContentAttribute.CheckedLevel, 0); }
            set { SetExtendedAttribute(ContentAttribute.CheckedLevel, value.ToString()); }
		}

        public int Comments
        {
            get { return GetInt(ContentAttribute.Comments, 0); }
            set { SetExtendedAttribute(ContentAttribute.Comments, value.ToString()); }
        }

        public int Photos
        {
            get { return GetInt(ContentAttribute.Photos, 0); }
            set { SetExtendedAttribute(ContentAttribute.Photos, value.ToString()); }
        }

        public int Hits
        {
            get { return GetInt(ContentAttribute.Hits, 0); }
            set { SetExtendedAttribute(ContentAttribute.Hits, value.ToString()); }
        }

        public int HitsByDay
        {
            get { return GetInt(ContentAttribute.HitsByDay, 0); }
            set { SetExtendedAttribute(ContentAttribute.HitsByDay, value.ToString()); }
        }

        public int HitsByWeek
        {
            get { return GetInt(ContentAttribute.HitsByWeek, 0); }
            set { SetExtendedAttribute(ContentAttribute.HitsByWeek, value.ToString()); }
        }

        public int HitsByMonth
        {
            get { return GetInt(ContentAttribute.HitsByMonth, 0); }
            set { SetExtendedAttribute(ContentAttribute.HitsByMonth, value.ToString()); }
        }

        public DateTime LastHitsDate
        {
            get { return GetDateTime(ContentAttribute.LastHitsDate, DateTime.Now); }
            set { SetExtendedAttribute(ContentAttribute.LastHitsDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string Title
		{
            get { return GetExtendedAttribute(ContentAttribute.Title); }
            set { SetExtendedAttribute(ContentAttribute.Title, value); }
		}
      
        public bool IsTop
        {
            get { return GetBool(ContentAttribute.IsTop, false); }
            set { SetExtendedAttribute(ContentAttribute.IsTop, value.ToString()); }
        }

        public DateTime AddDate
        {
            get { return GetDateTime(ContentAttribute.AddDate, DateTime.Now); }
            set { SetExtendedAttribute(ContentAttribute.AddDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return ContentAttribute.AllAttributes;
        }
	}
}
