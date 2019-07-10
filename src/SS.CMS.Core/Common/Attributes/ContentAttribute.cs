using System;
using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class ContentAttribute
    {
        public const string Id = nameof(Entity.Id);
        public const string Guid = nameof(Entity.Guid);
        public const string CreatedDate = nameof(Entity.CreatedDate);
        public const string LastModifiedDate = nameof(Entity.LastModifiedDate);
        public const string ChannelId = nameof(CMS.Models.Content.ChannelId);
        public const string SiteId = nameof(CMS.Models.Content.SiteId);
        public const string UserId = nameof(CMS.Models.Content.UserId);
        public const string LastModifiedUserId = nameof(CMS.Models.Content.LastModifiedUserId);
        public const string Taxis = nameof(CMS.Models.Content.Taxis);
        public const string GroupNameCollection = nameof(CMS.Models.Content.GroupNameCollection);
        public const string Tags = nameof(CMS.Models.Content.Tags);
        public const string SourceId = nameof(CMS.Models.Content.SourceId);
        public const string ReferenceId = nameof(CMS.Models.Content.ReferenceId);
        public const string IsChecked = nameof(CMS.Models.Content.IsChecked);
        public const string CheckedLevel = nameof(CMS.Models.Content.CheckedLevel);
        public const string Hits = nameof(CMS.Models.Content.Hits);
        public const string HitsByDay = nameof(CMS.Models.Content.HitsByDay);
        public const string HitsByWeek = nameof(CMS.Models.Content.HitsByWeek);
        public const string HitsByMonth = nameof(CMS.Models.Content.HitsByMonth);
        public const string LastHitsDate = nameof(CMS.Models.Content.LastHitsDate);
        public const string Downloads = nameof(CMS.Models.Content.Downloads);
        public const string ExtendValues = nameof(CMS.Models.Content.ExtendValues);
        public const string Title = nameof(CMS.Models.Content.Title);
        public const string IsTop = nameof(CMS.Models.Content.IsTop);
        public const string IsRecommend = nameof(CMS.Models.Content.IsRecommend);
        public const string IsHot = nameof(CMS.Models.Content.IsHot);
        public const string IsColor = nameof(CMS.Models.Content.IsColor);
        public const string LinkUrl = nameof(CMS.Models.Content.LinkUrl);
        public const string AddDate = nameof(CMS.Models.Content.AddDate);
        public const string Content = nameof(CMS.Models.Content.Body);
        public const string SubTitle = nameof(CMS.Models.Content.SubTitle);
        public const string ImageUrl = nameof(CMS.Models.Content.ImageUrl);
        public const string VideoUrl = nameof(CMS.Models.Content.VideoUrl);
        public const string FileUrl = nameof(CMS.Models.Content.FileUrl);
        public const string Author = nameof(CMS.Models.Content.Author);
        public const string Source = nameof(CMS.Models.Content.Source);
        public const string Summary = nameof(CMS.Models.Content.Summary);

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
        public const string CheckUserId = nameof(CheckUserId);                  //审核者
        public const string CheckDate = nameof(CheckDate);                          //审核时间
        public const string CheckReasons = nameof(CheckReasons);                    //审核原因
        public const string TranslateContentType = nameof(TranslateContentType);    //转移内容类型

        public static readonly Lazy<List<string>> AllAttributes = new Lazy<List<string>>(() => new List<string>
        {
            Id,
            ChannelId,
            SiteId,
            UserId,
            LastModifiedUserId,
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
            ExtendValues,
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
            UserId,
            LastModifiedUserId,
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
            ExtendValues,
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
            UserId,
            LastModifiedUserId,
            SourceId
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
