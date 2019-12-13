using System;
using System.Collections.Generic;


namespace SiteServer.Abstractions
{
    public static class ContentAttribute
    {
        public const string Id = nameof(Abstractions.Content.Id);
        public const string Guid = nameof(Abstractions.Content.Guid);
        public const string CreatedDate = nameof(Abstractions.Content.CreatedDate);
        public const string LastModifiedDate = nameof(Abstractions.Content.LastModifiedDate);
        public const string ChannelId = nameof(Abstractions.Content.ChannelId);
        public const string SiteId = nameof(Abstractions.Content.SiteId);
        public const string AddUserName = nameof(Abstractions.Content.AddUserName);
        public const string LastEditUserName = nameof(Abstractions.Content.LastEditUserName);
        public const string LastEditDate = nameof(Abstractions.Content.LastEditDate);
        public const string AdminId = nameof(Abstractions.Content.AdminId);
        public const string UserId = nameof(Abstractions.Content.UserId);
        public const string Taxis = nameof(Abstractions.Content.Taxis);
        public const string GroupNameCollection = nameof(Abstractions.Content.GroupNameCollection);
        public const string Tags = nameof(Abstractions.Content.Tags);
        public const string SourceId = nameof(Abstractions.Content.SourceId);
        public const string ReferenceId = nameof(Abstractions.Content.ReferenceId);
        public const string IsChecked = nameof(Abstractions.Content.IsChecked);
        public const string CheckedLevel = nameof(Abstractions.Content.CheckedLevel);
        public const string Hits = nameof(Abstractions.Content.Hits);
        public const string HitsByDay = nameof(Abstractions.Content.HitsByDay);
        public const string HitsByWeek = nameof(Abstractions.Content.HitsByWeek);
        public const string HitsByMonth = nameof(Abstractions.Content.HitsByMonth);
        public const string LastHitsDate = nameof(Abstractions.Content.LastHitsDate);
        public const string Downloads = nameof(Abstractions.Content.Downloads);
        public const string Title = nameof(Abstractions.Content.Title);
        public const string IsTop = nameof(Abstractions.Content.IsTop);
        public const string IsRecommend = nameof(IsRecommend);
        public const string IsHot = nameof(IsHot);
        public const string IsColor = nameof(IsColor);
        public const string LinkUrl = nameof(Abstractions.Content.LinkUrl);
        public const string AddDate = nameof(Abstractions.Content.AddDate);

        public const string SubTitle = nameof(SubTitle);
        public const string ImageUrl = nameof(ImageUrl);
        public const string VideoUrl = nameof(VideoUrl);
        public const string FileUrl = nameof(FileUrl);
        public const string Summary = nameof(Summary);
        public const string Author = nameof(Author);
        public const string Source = nameof(Source);
        public const string Content = nameof(Content);
        public const string SettingsXml = nameof(SettingsXml);

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

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            Guid,
            CreatedDate,
            LastModifiedDate,
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
            Downloads,
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
            Guid,
            CreatedDate,
            LastModifiedDate,
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
            Downloads,
            IsTop,
            IsRecommend,
            IsHot,
            IsColor,
            AddDate,
            LinkUrl,
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
            "UnCheckTaskDate",
            "Photos",
            "Teleplays",
            "MemberName"
        });
    }
}
