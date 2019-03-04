using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Attributes
{
    public static class ContentAttribute
    {
        public const string Id = nameof(DynamicEntity.Id);
        public const string Guid = nameof(DynamicEntity.Guid);
        public const string LastModifiedDate = nameof(DynamicEntity.LastModifiedDate);
        public const string ChannelId = nameof(ChannelId);
        public const string SiteId = nameof(SiteId);
        public const string AddUserName = nameof(AddUserName);
        public const string LastEditUserName = nameof(LastEditUserName);
        public const string LastEditDate = nameof(LastEditDate);
        public const string AdminId = nameof(AdminId);
        public const string UserId = nameof(UserId);
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
        public const string Content = nameof(Content);

        public const string SubTitle = nameof(SubTitle);
        public const string ImageUrl = nameof(ImageUrl);
        public const string VideoUrl = nameof(VideoUrl);
        public const string FileUrl = nameof(FileUrl);
        public const string Author = nameof(Author);
        public const string Source = nameof(Source);
        public const string Summary = nameof(Summary);

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
