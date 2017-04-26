using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class ContentAttribute
	{
        protected ContentAttribute()
		{
		}
		
		public const string Id = "Id";
        public const string NodeId = "NodeId";
        public const string PublishmentSystemId = "PublishmentSystemId";
        public const string AddUserName = "AddUserName";
        public const string LastEditUserName = "LastEditUserName";
        public const string WritingUserName = "WritingUserName";
        public const string LastEditDate = "LastEditDate";
        public const string Taxis = "Taxis";
        public const string ContentGroupNameCollection = "ContentGroupNameCollection";
        public const string Tags = "Tags";
        public const string SourceId = "SourceId";
        public const string ReferenceId = "ReferenceId";
        public const string IsChecked = "IsChecked";
        public const string CheckedLevel = "CheckedLevel";
        public const string Comments = "Comments";
        public const string Photos = "Photos";
        public const string Hits = "Hits";
        public const string HitsByDay = "HitsByDay";
        public const string HitsByWeek = "HitsByWeek";
        public const string HitsByMonth = "HitsByMonth";
        public const string LastHitsDate = "LastHitsDate";
        public const string SettingsXml = "SettingsXml";

        //具体类中实现
        public const string Title = "Title";
        public const string IsTop = "IsTop";
        public const string AddDate = "AddDate";

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
            SettingsXml.ToLower()
        });

	    public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(HiddenAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            Title.ToLower(),
            IsTop.ToLower(),
            AddDate.ToLower()
        });

	    private static List<string> _excludeAttributes;
        public static List<string> ExcludeAttributes => _excludeAttributes ?? (_excludeAttributes = new List<string>
        {
            IsTop.ToLower()
        });
	}
}
