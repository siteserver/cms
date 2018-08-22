using System.Collections.Generic;

namespace SiteServer.CMS.Model.Attributes
{
    public static class ContentAttribute
    {
        public const string Id = nameof(Id);
        public const string ChannelId = nameof(ChannelId);
        public const string SiteId = nameof(SiteId);
        public const string AddUserName = nameof(AddUserName);
        public const string LastEditUserName = nameof(LastEditUserName);
        public const string WritingUserName = nameof(WritingUserName);
        public const string LastEditDate = nameof(LastEditDate);
        public const string Taxis = nameof(Taxis);
        public const string GroupNameCollection = nameof(GroupNameCollection);
        public const string Tags = nameof(Tags);
        public const string SourceId = nameof(SourceId);
        public const string ReferenceId = nameof(ReferenceId);
        public const string IsChecked = nameof(IsChecked);
        public const string CheckedLevel = nameof(CheckedLevel);
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
        public const string LinkUrl = nameof(LinkUrl);
        public const string AddDate = nameof(AddDate);

        public static string GetFormatStringAttributeName(string attributeName)
        {
            return attributeName + "FormatString";
        }

        public static string GetExtendAttributeName(string attributeName)
        {
            return attributeName + "_Extend";
        }

        public const string CheckUserName = "Check_UserName";            //审核者
        public const string CheckCheckDate = "Check_CheckDate";          //审核时间
        public const string CheckReasons = "Check_Reasons";              //审核原因
        public const string TranslateContentType = "TranslateContentType";    //转移内容类型

        private static List<string> _allAttributesLowercase;

        public static List<string> AllAttributesLowercase => _allAttributesLowercase ?? (_allAttributesLowercase = new List<string>
        {
            Id.ToLower(),
            ChannelId.ToLower(),
            SiteId.ToLower(),
            AddUserName.ToLower(),
            LastEditUserName.ToLower(),
            WritingUserName.ToLower(),
            LastEditDate.ToLower(),
            Taxis.ToLower(),
            GroupNameCollection.ToLower(),
            Tags.ToLower(),
            SourceId.ToLower(),
            ReferenceId.ToLower(),
            IsChecked.ToLower(),
            CheckedLevel.ToLower(),
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
            LinkUrl.ToLower(),
            AddDate.ToLower()
        });
    }
}
