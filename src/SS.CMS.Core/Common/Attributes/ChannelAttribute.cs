using SS.CMS.Core.StlParser.Models;
using SS.CMS.Models;

namespace SS.CMS.Core.Models.Attributes
{
    public static class ChannelAttribute
    {
        [StlAttribute(Title = "栏目Id")]
        public const string Id = nameof(Channel.Id);
        [StlAttribute(Title = "栏目Guid")]
        public const string Guid = nameof(Channel.Guid);
        [StlAttribute(Title = "创建时间")]
        public const string CreatedDate = nameof(Channel.CreatedDate);
        [StlAttribute(Title = "修改时间")]
        public const string LastModifiedDate = nameof(Channel.LastModifiedDate);

        [StlAttribute(Title = "站点Id")]
        public const string SiteId = nameof(Channel.SiteId);

        [StlAttribute(Title = "内容模型插件Id")]
        public const string ContentModelPluginId = nameof(Channel.ContentModelPluginId);

        [StlAttribute(Title = "内容关联插件Id列表")]
        public const string ContentRelatedPluginIds = nameof(Channel.ContentRelatedPluginIds);

        [StlAttribute(Title = "父栏目Id")]
        public const string ParentId = nameof(Channel.ParentId);

        [StlAttribute(Title = "上级栏目路径")]
        public const string ParentsPath = nameof(Channel.ParentsPath);

        [StlAttribute(Title = "上级栏目数量")]
        public const string ParentsCount = nameof(Channel.ParentsCount);

        [StlAttribute(Title = "下级栏目数量")]
        public const string ChildrenCount = nameof(Channel.ChildrenCount);

        [StlAttribute(Title = "是否最后一级栏目")]
        public const string IsLastNode = nameof(Channel.IsLastNode);

        [StlAttribute(Title = "栏目索引")]
        public const string IndexName = nameof(Channel.IndexName);

        [StlAttribute(Title = "栏目组")]
        public const string GroupNameCollection = nameof(Channel.GroupNameCollection);

        [StlAttribute(Title = "栏目排序")]
        public const string Taxis = nameof(Channel.Taxis);

        [StlAttribute(Title = "栏目图片")]
        public const string ImageUrl = nameof(Channel.ImageUrl);

        [StlAttribute(Title = "栏目正文")]
        public const string Content = nameof(Channel.Content);

        [StlAttribute(Title = "页面路径")]
        public const string FilePath = nameof(Channel.FilePath);

        [StlAttribute(Title = "下级栏目页面命名规则")]
        public const string ChannelFilePathRule = nameof(Channel.ChannelFilePathRule);

        [StlAttribute(Title = "下级内容页面命名规则")]
        public const string ContentFilePathRule = nameof(Channel.ContentFilePathRule);

        [StlAttribute(Title = "外部链接")]
        public const string LinkUrl = nameof(Channel.LinkUrl);

        [StlAttribute(Title = "链接类型")]
        public const string LinkType = nameof(Channel.LinkType);

        [StlAttribute(Title = "栏目模板Id")]
        public const string ChannelTemplateId = nameof(Channel.ChannelTemplateId);

        [StlAttribute(Title = "内容模板Id")]
        public const string ContentTemplateId = nameof(Channel.ContentTemplateId);

        [StlAttribute(Title = "关键字列表")]
        public const string Keywords = nameof(Channel.Keywords);

        [StlAttribute(Title = "页面描述")]
        public const string Description = nameof(Channel.Description);

        [StlAttribute(Title = "栏目扩展值")]
        public const string ExtendValues = nameof(ExtendValues);

        [StlAttribute(Title = "栏目名称")]
        public const string Title = nameof(Title);

        [StlAttribute(Title = "栏目名称")]
        public const string ChannelName = nameof(Channel.ChannelName);

        [StlAttribute(Title = "栏目索引")]
        public const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目正文（翻页）")]
        public const string PageContent = nameof(PageContent);

        [StlAttribute(Title = "顺序")]
        public const string ItemIndex = nameof(ItemIndex);

        [StlAttribute(Title = "下级栏目数")]
        public const string CountOfChannels = nameof(CountOfChannels);

        [StlAttribute(Title = "包含内容数")]
        public const string CountOfContents = nameof(CountOfContents);

        [StlAttribute(Title = "包含图片内容数")]
        public const string CountOfImageContents = nameof(CountOfImageContents);
    }
}
