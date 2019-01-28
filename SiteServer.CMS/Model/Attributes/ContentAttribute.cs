using System;
using System.Collections.Generic;

namespace SiteServer.CMS.Model.Attributes
{
    public static class ContentAttribute
    {
        public const string Id = nameof(ContentInfo.Id);
        public const string ChannelId = nameof(ContentInfo.ChannelId);
        public const string SiteId = nameof(ContentInfo.SiteId);
        public const string AddUserName = nameof(ContentInfo.AddUserName);
        public const string LastEditUserName = nameof(ContentInfo.LastEditUserName);
        public const string LastEditDate = nameof(ContentInfo.LastEditDate);
        public const string AdminId = nameof(ContentInfo.AdminId);
        public const string UserId = nameof(ContentInfo.UserId);
        public const string Taxis = nameof(ContentInfo.Taxis);
        public const string GroupNameCollection = nameof(ContentInfo.GroupNameCollection);
        public const string Tags = nameof(ContentInfo.Tags);
        public const string SourceId = nameof(ContentInfo.SourceId);
        public const string ReferenceId = nameof(ContentInfo.ReferenceId);
        public const string IsChecked = nameof(ContentInfo.IsChecked);
        public const string CheckedLevel = nameof(ContentInfo.CheckedLevel);
        public const string Hits = nameof(ContentInfo.Hits);
        public const string HitsByDay = nameof(ContentInfo.HitsByDay);
        public const string HitsByWeek = nameof(ContentInfo.HitsByWeek);
        public const string HitsByMonth = nameof(ContentInfo.HitsByMonth);
        public const string LastHitsDate = nameof(ContentInfo.LastHitsDate);
        public const string SettingsXml = nameof(ContentInfo.SettingsXml);
        public const string Title = nameof(ContentInfo.Title);
        public const string IsTop = nameof(ContentInfo.IsTop);
        public const string IsRecommend = nameof(ContentInfo.IsRecommend);
        public const string IsHot = nameof(ContentInfo.IsHot);
        public const string IsColor = nameof(ContentInfo.IsColor);
        public const string LinkUrl = nameof(ContentInfo.LinkUrl);
        public const string AddDate = nameof(ContentInfo.AddDate);

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

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            AddUserName,
            LastEditUserName,
            LastEditDate,
            AdminId,
            UserId,
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

        public static readonly Lazy<List<string>> MetadataAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            AddUserName,
            LastEditUserName,
            LastEditDate,
            AdminId,
            UserId,
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

        public static readonly Lazy<List<string>> IncludedAttributes = new Lazy<List<string>>(() => new List<string>
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

        public static readonly Lazy<List<string>> ExcludedAttributes = new Lazy<List<string>>(() => new List<string>
        {
            SettingsXml
        });

        public static readonly Lazy<List<string>> CalculateAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Sequence,
            AdminId,
            UserId,
            SourceId,
            AddUserName,
            LastEditUserName
        });

        public static readonly Lazy<List<string>> DropAttributes = new Lazy<List<string>>(() => new List<string>
        {
            "WritingUserName",
            "ConsumePoint",
            "Comments",
            "Reply",
            "CheckTaskDate",
            "UnCheckTaskDate"
        });
    }
}
