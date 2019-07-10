using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 栏目实体接口。
    /// 对应数据库中的siteserver_Channel表。
    /// </summary>
    public interface IChannelInfo
    {
        /// <summary>
        /// 栏目Id。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 栏目名称。
        /// </summary>
        string ChannelName { get; set; }

        /// <summary>
        /// 栏目所在的站点Id。
        /// </summary>
        int SiteId { get; set; }

        /// <summary>
        /// 内容模型插件设置，只能设置一个内容模型插件，设置后此栏目下的内容表将由插件定义。
        /// </summary>
        string ContentModelPluginId { get; set; }

        /// <summary>
        /// 内容关联插件设置，可以设置多个关联插件。
        /// 多个关联插件以英文逗号隔开。
        /// </summary>
        string ContentRelatedPluginIds { get; set; }

        /// <summary>
        /// 父栏目Id。
        /// </summary>
        int ParentId { get; set; }

        /// <summary>
        /// 上级栏目路径，包含所有上级栏目的Id，以英文逗号分隔。
        /// </summary>
        string ParentsPath { get; set; }

        /// <summary>
        /// 上级栏目数量。
        /// </summary>
        int ParentsCount { get; set; }

        /// <summary>
        /// 下级栏目数量。
        /// </summary>
        int ChildrenCount { get; set; }

        /// <summary>
        /// 是否最后一级栏目。
        /// </summary>
        bool LastNode { get; set; }

        /// <summary>
        /// 栏目索引。
        /// </summary>
        string IndexName { get; set; }

        /// <summary>
        /// 栏目组。
        /// 多个栏目组以英文逗号隔开。
        /// </summary>
        string GroupNameCollection { get; set; }

        /// <summary>
        /// 排序。
        /// </summary>
        int Taxis { get; set; }

        /// <summary>
        /// 栏目添加时间。
        /// </summary>
        DateTime? AddDate { get; set; }

        /// <summary>
        /// 栏目图片，存储图片地址。
        /// </summary>
        string ImageUrl { get; set; }

        /// <summary>
        /// 栏目正文，以编辑器提交信息。
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// 栏目生成页面路径。
        /// </summary>
        string FilePath { get; set; }

        /// <summary>
        /// 下级栏目的页面命名规则。
        /// </summary>
        string ChannelFilePathRule { get; set; }

        /// <summary>
        /// 栏目下内容的页面命名规则。
        /// </summary>
        string ContentFilePathRule { get; set; }

        /// <summary>
        /// 外部链接，设置后链接将指向此地址。
        /// </summary>
        string LinkUrl { get; set; }

        /// <summary>
        /// 链接类型，设置此栏目的链接与子栏目及内容的关系。
        /// </summary>
        string LinkType { get; set; }

        /// <summary>
        /// 栏目模板Id。
        /// </summary>
        int ChannelTemplateId { get; set; }

        /// <summary>
        /// 内容模板Id。
        /// </summary>
        int ContentTemplateId { get; set; }

        /// <summary>
        /// 关键字列表，各关键词间用英文逗号分割。
        /// </summary>
        string Keywords { get; set; }

        /// <summary>
        /// 页面描述。
        /// </summary>
        string Description { get; set; }
    }
}