using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 内容Api接口。
    /// </summary>
    public interface IContentApi
    {
        /// <summary>
        /// 实例化内容对象。
        /// </summary>
        /// <param name="siteId">内容所属的站点Id。</param>
        /// <param name="channelId">内容所属的栏目Id。</param>
        /// <returns>返回新的内容实例。</returns>
        IContentInfo NewInstance(int siteId, int channelId);

        /// <summary>
        /// 获取内容实例。
        /// </summary>
        /// <param name="siteId">内容所属的站点Id。</param>
        /// <param name="channelId">内容所属的栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <returns>
        /// 如果对应的内容存在，则返回内容实例；否则返回 null。
        /// </returns>
        IContentInfo GetContentInfo(int siteId, int channelId, int contentId);

        /// <summary>
        /// 获取满足条件的内容总数。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="whereString">WHERE SQL语句</param>
        /// <returns>
        /// 如果满足条件的内容存在，则返回内容总数；否则返回 0。
        /// </returns>
        int GetCount(int siteId, int channelId, string whereString);

        /// <summary>
        /// 获取满足条件的翻页内容列表。
        /// 配合GetCount方法，能够实现内容的翻页效果。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="whereString">WHERE SQL语句</param>
        /// <param name="orderString">ORDER SQL语句</param>
        /// <param name="limit">
        /// 返回结果的数量。
        /// 如果不限制，将 limit 值设置为 0。
        /// 如果给定大于0的值，返回的结果将不会超过这个数目。
        /// </param>
        /// <param name="offset">
        /// 返回结果的偏移量。
        /// offset 表示在开始返回行之前跳过这么多行，如果从第一行开始返回，offset 是 0，以此类推。
        /// </param>
        /// <returns>
        /// 如果满足条件的内容列表存在，则返回内容列表；否则返回 null。
        /// </returns>
        List<IContentInfo> GetContentInfoList(int siteId, int channelId, string whereString,
            string orderString, int limit, int offset);

        /// <summary>
        /// 获取指定栏目的所有内容Id的列表。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>
        /// 如果站点与栏目存在，则返回此栏目下的所有内容的Id列表；否则返回 null。
        /// </returns>
        List<int> GetContentIdList(int siteId, int channelId);

        /// <summary>
        /// 获取内容的属性值。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <param name="attributeName">内容属性名称。</param>
        /// <returns>
        /// 如果对应的内容存在，则返回内容的属性值；否则返回 null。
        /// </returns>
        string GetContentValue(int siteId, int channelId, int contentId, string attributeName);

        /// <summary>
        /// 通过站点Id以及栏目Id获取此栏目关联的内容表名称。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>
        /// 如果站点及栏目存在，则返回此栏目关联的内容表名称；否则返回 null。
        /// </returns>
        string GetTableName(int siteId, int channelId);

        /// <summary>
        /// 通过站点Id以及栏目Id获取此栏目关联的内容表字段列表。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>
        /// 如果站点及栏目存在，则返回此栏目关联的内容表字段列表；否则返回 null。
        /// </returns>
        List<TableColumn> GetTableColumns(int siteId, int channelId);

        /// <summary>
        /// 新增内容。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentInfo">需要新增的内容实例。</param>
        /// <returns>
        /// 返回内容实例在数据库中插入后的自增长Id。
        /// </returns>
        int Insert(int siteId, int channelId, IContentInfo contentInfo);

        /// <summary>
        /// 修改内容。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentInfo">需要更新的内容实例。</param>
        void Update(int siteId, int channelId, IContentInfo contentInfo);

        /// <summary>
        /// 删除内容。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">需要删除的内容Id。</param>
        void Delete(int siteId, int channelId, int contentId);

        /// <summary>
        /// 获取内容Url访问地址。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <param name="contentId">内容Id。</param>
        /// <returns>内容Url访问地址。</returns>
        string GetContentUrl(int siteId, int channelId, int contentId);
    }
}