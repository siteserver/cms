using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 栏目Api接口。
    /// </summary>
    public interface IChannelApi
    {
        /// <summary>
        /// 实例化指定站点Id的栏目对象。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <returns>返回新的栏目实例。</returns>
        IChannelInfo NewInstance(int siteId);

        /// <summary>
        /// 通过站点Id以及栏目Id获取对应的栏目实例。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>
        /// 如果对应的栏目存在，则返回栏目实例；否则返回 null。
        /// </returns>
        IChannelInfo GetChannelInfo(int siteId, int channelId);

        /// <summary>
        /// 通过站点Id以及栏目索引获取栏目Id。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelIndex">栏目索引。</param>
        /// <returns>
        /// 如果对应的栏目存在，则返回栏目实例；否则返回 0。
        /// </returns>
        int GetChannelId(int siteId, string channelIndex);

        /// <summary>
        /// 通过站点Id以及栏目Id获取栏目名称。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>
        /// 如果对应的栏目存在，则返回栏目名称；否则返回 null。
        /// </returns>
        string GetChannelName(int siteId, int channelId);

        /// <summary>
        /// 通过站点Id获取此站点下的所有栏目Id的列表。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <returns>
        /// 如果站点存在，则返回此站点的所有栏目的Id列表；否则返回 null。
        /// </returns>
        List<int> GetChannelIdList(int siteId);

        /// <summary>
        /// 通过站点Id以及父栏目Id获取父栏目下的栏目Id的列表。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="parentId">父栏目Id</param>
        /// <returns>
        /// 如果站点及父栏目存在，则返回父栏目下的栏目的Id列表；否则返回 null。
        /// </returns>
        List<int> GetChannelIdList(int siteId, int parentId);

        /// <summary>
        /// 新增栏目。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelInfo">需要新增的栏目实例。</param>
        /// <returns>
        /// 返回栏目实例在数据库中插入后的自增长Id。
        /// </returns>
        int Insert(int siteId, IChannelInfo channelInfo);

        /// <summary>
        /// 修改栏目。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelInfo">需要更新的栏目实例。</param>
        void Update(int siteId, IChannelInfo channelInfo);

        /// <summary>
        /// 删除栏目。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">需要删除的栏目Id。</param>
        void Delete(int siteId, int channelId);

        /// <summary>
        /// 获取栏目Url访问地址。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="channelId">栏目Id。</param>
        /// <returns>栏目Url访问地址。</returns>
        string GetChannelUrl(int siteId, int channelId);
    }
}
