using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 站点Api接口。
    /// </summary>
    public interface ISiteApi
    {
        /// <summary>
        /// 通过目录/文件的绝对路径获取站点Id。
        /// </summary>
        /// <param name="path">存储于站点内的目录/文件的绝对路径。</param>
        /// <returns>如果目录/文件存储于站点中，则返回此站点的Id；否则返回 0。</returns>
        int GetSiteIdByFilePath(string path);

        /// <summary>
        /// 获取站点文件夹绝对路径。
        /// </summary>
        /// <param name="siteId">需要获取路径的站点Id。</param>
        /// <returns>站点文件夹的绝对路径。</returns>
        string GetSitePath(int siteId);

        /// <summary>
        /// 获取系统中的所有站点的Id列表。
        /// </summary>
        /// <returns>站点Id列表。</returns>
        List<int> GetSiteIdList();

        /// <summary>
        /// 通过站点Id获取指定站点的对象实体。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <returns>如果站点Id存在，则返回此站点的对象实体；否则返回 null。</returns>
        ISiteInfo GetSiteInfo(int siteId);

        /// <summary>
        /// 获取站点文件的绝对路径。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="virtualPath">相对路径。</param>
        /// <returns>指定站点的文件绝对路径。</returns>
        string GetSitePath(int siteId, string virtualPath);

        /// <summary>
        /// 获取站点访问Url地址。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <returns>站点访问Url地址。</returns>
        string GetSiteUrl(int siteId);

        /// <summary>
        /// 获取站点访问Url地址。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="virtualPath">相对路径。</param>
        /// <returns>站点访问Url地址。</returns>
        string GetSiteUrl(int siteId, string virtualPath);

        /// <summary>
        /// 根据文件的绝对地址计算此文件的访问Url地址。
        /// </summary>
        /// <param name="filePath">文件的绝对路径。</param>
        /// <returns>此文件的访问Url地址。</returns>
        string GetSiteUrlByFilePath(string filePath);
    }
}