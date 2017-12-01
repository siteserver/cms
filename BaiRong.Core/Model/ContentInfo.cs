using System;
using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Model
{
    public class ContentAttribute
    {
        protected ContentAttribute()
        {
        }

        public const string Id = nameof(Id);
        public const string NodeId = nameof(NodeId);
        public const string PublishmentSystemId = nameof(PublishmentSystemId);
        public const string AddUserName = nameof(AddUserName);
        public const string LastEditUserName = nameof(LastEditUserName);
        public const string WritingUserName = nameof(WritingUserName);
        public const string LastEditDate = nameof(LastEditDate);
        public const string Taxis = nameof(Taxis);
        public const string ContentGroupNameCollection = nameof(ContentGroupNameCollection);
        public const string Tags = nameof(Tags);
        public const string SourceId = nameof(SourceId);
        public const string ReferenceId = nameof(ReferenceId);
        public const string IsChecked = nameof(IsChecked);
        public const string CheckedLevel = nameof(CheckedLevel);
        public const string Comments = nameof(Comments);
        public const string Photos = nameof(Photos);
        public const string Hits = nameof(Hits);
        public const string HitsByDay = nameof(HitsByDay);
        public const string HitsByWeek = nameof(HitsByWeek);
        public const string HitsByMonth = nameof(HitsByMonth);
        public const string LastHitsDate = nameof(LastHitsDate);
        public const string SettingsXml = nameof(SettingsXml);
        public const string Title = nameof(Title);
        public const string IsTop = nameof(IsTop);
        public const string IsRecommend = nameof(IsRecommend);
        public const string IsHot = nameof(IsHot);
        public const string IsColor = nameof(IsColor);
        public const string AddDate = nameof(AddDate);

        //不存在
        public static string GetFormatStringAttributeName(string attributeName)
        {
            return attributeName + "FormatString";
        }

        public static string GetExtendAttributeName(string attributeName)
        {
            return attributeName + "_Extend";
        }

        public const string CheckIsAdmin = "Check_IsAdmin";              //审核者是否为管理员
        public const string CheckUserName = "Check_UserName";            //审核者
        public const string CheckCheckDate = "Check_CheckDate";          //审核时间
        public const string CheckReasons = "Check_Reasons";              //审核原因

        public const string TranslateContentType = "TranslateContentType";    //转移内容类型

        private static List<string> _hiddenAttributes;

        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
            Id.ToLower(),
            NodeId.ToLower(),
            PublishmentSystemId.ToLower(),
            AddUserName.ToLower(),
            LastEditUserName.ToLower(),
            WritingUserName.ToLower(),
            LastEditDate.ToLower(),
            Taxis.ToLower(),
            ContentGroupNameCollection.ToLower(),
            Tags.ToLower(),
            SourceId.ToLower(),
            ReferenceId.ToLower(),
            IsChecked.ToLower(),
            CheckedLevel.ToLower(),
            Comments.ToLower(),
            Photos.ToLower(),
            Hits.ToLower(),
            HitsByDay.ToLower(),
            HitsByWeek.ToLower(),
            HitsByMonth.ToLower(),
            LastHitsDate.ToLower(),
            SettingsXml.ToLower(),
            Title.ToLower(),
            IsTop.ToLower(),
            IsRecommend.ToLower(),
            IsHot.ToLower(),
            IsColor.ToLower(),
            AddDate.ToLower()
        });

        public static List<string> AllAttributes => HiddenAttributes;
    }

    public class ContentInfo : ExtendedAttributes, IContentInfo
	{
		public ContentInfo()
		{
			Id = 0;
			NodeId = 0;
			PublishmentSystemId = 0;
			AddUserName = string.Empty;
			LastEditUserName = string.Empty;
            WritingUserName = string.Empty;
            LastEditDate = DateTime.Now;
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
            IsRecommend = false;
            IsHot = false;
            IsColor = false;
            AddDate = DateTime.Now;
		}

        public ContentInfo(object dataItem) : base(dataItem)
        {

        }

        public int Id
		{
            get { return GetInt(ContentAttribute.Id); }
            set { Set(ContentAttribute.Id, value.ToString()); }
		}

        public int NodeId
        {
            get { return GetInt(ContentAttribute.NodeId); }
            set { Set(ContentAttribute.NodeId, value.ToString()); }
        }

        public int PublishmentSystemId
		{
            get { return GetInt(ContentAttribute.PublishmentSystemId); }
            set { Set(ContentAttribute.PublishmentSystemId, value.ToString()); }
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

        public string ContentGroupNameCollection
		{
            get { return GetString(ContentAttribute.ContentGroupNameCollection); }
            set { Set(ContentAttribute.ContentGroupNameCollection, value); }
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

        public int Comments
        {
            get { return GetInt(ContentAttribute.Comments); }
            set { Set(ContentAttribute.Comments, value.ToString()); }
        }

        public int Photos
        {
            get { return GetInt(ContentAttribute.Photos); }
            set { Set(ContentAttribute.Photos, value.ToString()); }
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

        public string SettingsXml
        {
            get { return GetString(ContentAttribute.SettingsXml); }
            set { Set(ContentAttribute.SettingsXml, value); }
        }
	}
}
