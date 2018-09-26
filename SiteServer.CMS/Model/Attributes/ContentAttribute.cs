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

        // 计算字段
        public const string Sequence = nameof(Sequence);                            //序号
        public const string PageContent = nameof(PageContent);
        public const string NavigationUrl = nameof(NavigationUrl);
        public const string CheckState = nameof(CheckState);

        // 附加字段
        public const string CheckUserName = nameof(CheckUserName);                  //审核者
        public const string CheckDate = nameof(CheckDate);                          //审核时间
        public const string CheckReasons = nameof(CheckReasons);                    //审核原因
        public const string TranslateContentType = nameof(TranslateContentType);    //转移内容类型

        private static List<string> _allAttributes;

        public static List<string> AllAttributes => _allAttributes ?? (_allAttributes = new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            AddUserName,
            LastEditUserName,
            WritingUserName,
            LastEditDate,
            Taxis,
            GroupNameCollection,
            Tags,
            SourceId,
            ReferenceId,
            IsChecked,
            CheckedLevel,
            Hits,
            HitsByDay,
            HitsByWeek,
            HitsByMonth,
            LastHitsDate,
            SettingsXml,
            Title,
            IsTop,
            IsRecommend,
            IsHot,
            IsColor,
            LinkUrl,
            AddDate
        });

        private static List<string> _hiddenAttributes;

        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
            Sequence,
            PageContent,
            NavigationUrl,
            CheckState,
            CheckUserName,
            CheckDate,
            CheckReasons,
            TranslateContentType
        });

        private static List<string> _metadataAttributes;

        public static List<string> MetadataAttributes => _metadataAttributes ?? (_metadataAttributes = new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            AddUserName,
            LastEditUserName,
            WritingUserName,
            LastEditDate,
            Taxis,
            GroupNameCollection,
            Tags,
            SourceId,
            ReferenceId,
            IsChecked,
            CheckedLevel,
            Hits,
            HitsByDay,
            HitsByWeek,
            HitsByMonth,
            LastHitsDate,
            SettingsXml,
            IsTop,
            IsRecommend,
            IsHot,
            IsColor,
            AddDate,
            LinkUrl
        });
    }
}
