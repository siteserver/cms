using System;
using System.Collections.Generic;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    public static class ContentAttribute
    {
        public const string Id = nameof(IContent.Id);
        public const string ChannelId = nameof(IContent.ChannelId);
        public const string SiteId = nameof(IContent.SiteId);
        public const string AddUserName = nameof(IContent.AddUserName);
        public const string LastEditUserName = nameof(IContent.LastEditUserName);
        public const string LastEditDate = nameof(IContent.LastEditDate);
        public const string AdminId = nameof(IContent.AdminId);
        public const string UserId = nameof(IContent.UserId);
        public const string Taxis = nameof(IContent.Taxis);
        public const string GroupNameCollection = nameof(IContent.GroupNameCollection);
        public const string Tags = nameof(IContent.Tags);
        public const string SourceId = nameof(IContent.SourceId);
        public const string ReferenceId = nameof(IContent.ReferenceId);
        public const string Checked = nameof(IContent.Checked);
        public const string CheckedLevel = nameof(IContent.CheckedLevel);
        public const string Hits = nameof(IContent.Hits);
        public const string HitsByDay = nameof(IContent.HitsByDay);
        public const string HitsByWeek = nameof(IContent.HitsByWeek);
        public const string HitsByMonth = nameof(IContent.HitsByMonth);
        public const string LastHitsDate = nameof(IContent.LastHitsDate);
        public const string Downloads = nameof(IContent.Downloads);
        public const string Title = nameof(IContent.Title);
        public const string IsTop = nameof(Model.Content.IsTop);
        public const string IsRecommend = nameof(IsRecommend);
        public const string IsHot = nameof(IsHot);
        public const string IsColor = nameof(IsColor);
        public const string LinkUrl = nameof(IContent.LinkUrl);
        public const string AddDate = nameof(IContent.AddDate);

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
            Checked,
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
            Checked,
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
            LinkUrl
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
