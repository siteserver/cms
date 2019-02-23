namespace SiteServer.Plugin
{
    /// <summary>
    /// 站点实体接口。
    /// 对应数据库中的siteserver_Site表。
    /// </summary>
    public interface ISiteInfo
    {
        /// <summary>
        /// 站点Id。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 站点名称。
        /// </summary>
        string SiteName { get; set; }

        /// <summary>
        /// 站点文件夹。
        /// </summary>
        string SiteDir { get; set; }

        /// <summary>
        /// 站点内容表。
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// 是否主站。
        /// </summary>
        bool Root { get; set; }

        /// <summary>
        /// 如果没有上级站点，则返回 0，否则返回上级站点Id。
        /// </summary>
        int ParentId { get; set; }

        /// <summary>
        /// 排序。
        /// </summary>
        int Taxis { get; set; }

        /// <summary>
        /// 站点可扩展属性，以键/值对的方式存储站点数据，键不区分大小写。
        /// 除了存储站点本身数据，还将用于存储站点的自定义字段数据。
        /// </summary>
        IAttributes Attributes { get; }
    }
}
