using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.Model.Attributes
{
    public static class ChannelAttribute
    {
        [StlField(Description = "栏目Id")]
        public const string Id = nameof(ChannelInfo.Id);

        [StlField(Description = "站点Id")]
        public const string SiteId = nameof(ChannelInfo.SiteId);

        [StlField(Description = "内容模型插件Id")]
        public const string ContentModelPluginId = nameof(ChannelInfo.ContentModelPluginId);

        [StlField(Description = "内容关联插件Id列表")]
        public const string ContentRelatedPluginIds = nameof(ChannelInfo.ContentRelatedPluginIds);

        [StlField(Description = "父栏目Id")]
        public const string ParentId = nameof(ChannelInfo.ParentId);

        [StlField(Description = "上级栏目路径")]
        public const string ParentsPath = nameof(ChannelInfo.ParentsPath);

        [StlField(Description = "上级栏目数量")]
        public const string ParentsCount = nameof(ChannelInfo.ParentsCount);

        [StlField(Description = "下级栏目数量")]
        public const string ChildrenCount = nameof(ChannelInfo.ChildrenCount);

        [StlField(Description = "是否最后一级栏目")]
        public const string IsLastNode = nameof(ChannelInfo.IsLastNode);

        [StlField(Description = "栏目索引")]
        public const string IndexName = nameof(ChannelInfo.IndexName);

        [StlField(Description = "栏目组")]
        public const string GroupNameCollection = nameof(ChannelInfo.GroupNameCollection);

        [StlField(Description = "栏目排序")]
        public const string Taxis = nameof(ChannelInfo.Taxis);

        [StlField(Description = "栏目添加时间")]
        public const string AddDate = nameof(ChannelInfo.AddDate);

        [StlField(Description = "栏目图片")]
        public const string ImageUrl = nameof(ChannelInfo.ImageUrl);

        [StlField(Description = "栏目正文")]
        public const string Content = nameof(ChannelInfo.Content);

        [StlField(Description = "包含内容数")]
        public const string ContentNum = nameof(ChannelInfo.ContentNum);

        [StlField(Description = "页面路径")]
        public const string FilePath = nameof(ChannelInfo.FilePath);

        [StlField(Description = "下级栏目页面命名规则")]
        public const string ChannelFilePathRule = nameof(ChannelInfo.ChannelFilePathRule);

        [StlField(Description = "下级内容页面命名规则")]
        public const string ContentFilePathRule = nameof(ChannelInfo.ContentFilePathRule);

        [StlField(Description = "外部链接")]
        public const string LinkUrl = nameof(ChannelInfo.LinkUrl);

        [StlField(Description = "链接类型")]
        public const string LinkType = nameof(ChannelInfo.LinkType);

        [StlField(Description = "栏目模板Id")]
        public const string ChannelTemplateId = nameof(ChannelInfo.ChannelTemplateId);

        [StlField(Description = "内容模板Id")]
        public const string ContentTemplateId = nameof(ChannelInfo.ContentTemplateId);

        [StlField(Description = "关键字列表")]
        public const string Keywords = nameof(ChannelInfo.Keywords);

        [StlField(Description = "页面描述")]
        public const string Description = nameof(ChannelInfo.Description);

        [StlField(Description = "栏目扩展值")]
        public const string ExtendValues = nameof(ExtendValues);

        [StlField(Description = "栏目名称")]
        public const string Title = nameof(Title);

        [StlField(Description = "栏目名称")]
        public const string ChannelName = nameof(ChannelInfo.ChannelName);

        [StlField(Description = "栏目索引")]
        public const string ChannelIndex = nameof(ChannelIndex);

        [StlField(Description = "栏目正文（翻页）")]
        public const string PageContent = nameof(PageContent);

        [StlField(Description = "顺序")]
        public const string ItemIndex = nameof(ItemIndex);

        [StlField(Description = "下级栏目数")]
        public const string CountOfChannels = nameof(CountOfChannels);

        [StlField(Description = "包含内容数")]
        public const string CountOfContents = nameof(CountOfContents);

        [StlField(Description = "包含图片内容数")]
        public const string CountOfImageContents = nameof(CountOfImageContents);

        

        
    }
}
