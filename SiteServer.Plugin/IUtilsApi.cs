using System.Collections.Generic;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 工具类Api接口。
    /// </summary>
    public interface IUtilsApi
    {
        /// <summary>
        /// 根据Web.config中设置的SecretKey加密字符串。
        /// </summary>
        /// <param name="inputString">原始字符串。</param>
        /// <returns>加密后的字符串。</returns>
        string Encrypt(string inputString);

        /// <summary>
        /// 根据Web.config中设置的SecretKey解密字符串。
        /// </summary>
        /// <param name="inputString">原始字符串。</param>
        /// <returns>解密后的字符串。</returns>
        string Decrypt(string inputString);

        /// <summary>
        /// 防XSS攻击过滤。
        /// </summary>
        /// <param name="html">需要过滤的Html。</param>
        /// <returns>过滤后的Html。</returns>
        string FilterXss(string html);

        /// <summary>
        /// 防Sql注入过滤。
        /// </summary>
        /// <param name="sql">需要过滤的SQL语句。</param>
        /// <returns>过滤后的SQL语句。</returns>
        string FilterSql(string sql);

        /// <summary>
        /// 跨站转移文件。
        /// </summary>
        /// <param name="sourceSiteId">原站点Id。</param>
        /// <param name="targetSiteId">转移到站点Id。</param>
        /// <param name="relatedUrls">包含所有需要转移的文件的相对地址列表。</param>
        void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls);

        /// <summary>
        /// 根据后台设置为图片添加水印。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="imagePath">图片文件的地址。</param>
        void AddWaterMark(int siteId, string imagePath);

        /// <summary>
        /// 根据后台设置获取指定上传文件名的路径。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="fileName">需要上传的文件名称。</param>
        /// <returns>需要上传文件的地址。</returns>
        string GetUploadFilePath(int siteId, string fileName);

        /// <summary>
        /// 根据后台设置获取指定上传文件名的Url地址。
        /// </summary>
        /// <param name="siteId">站点Id。</param>
        /// <param name="fileName">上传的文件名称。</param>
        /// <returns>上传文件的Url地址。</returns>
        string GetUploadFileUrl(int siteId, string fileName);

        /// <summary>
        /// 获取系统临时文件夹的绝对路径。
        /// </summary>
        /// <param name="relatedPath">相对路径。</param>
        /// <returns>系统临时文件夹的绝对路径。</returns>
        string GetTemporaryFilesPath(string relatedPath);

        /// <summary>
        /// 获取系统根目录访问Url地址。
        /// </summary>
        /// <param name="relatedUrl">相对地址。</param>
        /// <returns>系统根目录访问Url地址。</returns>
        string GetRootUrl(string relatedUrl = "");

        /// <summary>
        /// 获取管理后台文件访问Url地址。
        /// </summary>
        /// <param name="relatedUrl">相对地址。</param>
        /// <returns>管理后台文件访问Url地址。</returns>
        string GetAdminUrl(string relatedUrl = "");

        /// <summary>
        /// 获取用户中心文件访问Url地址。
        /// </summary>
        /// <param name="relatedUrl">相对地址。</param>
        /// <returns>用户中心文件访问Url地址。</returns>
        string GetHomeUrl(string relatedUrl = "");

        /// <summary>
        /// 获取API访问Url地址。
        /// </summary>
        /// <param name="relatedUrl">相对地址。</param>
        /// <returns>API访问Url地址。</returns>
        string GetApiUrl(string relatedUrl = "");

        /// <summary>
        /// 压缩文件夹。
        /// </summary>
        /// <param name="zipFilePath">压缩后的zip文件绝对地址。</param>
        /// <param name="directoryPath">需要压缩的文件夹绝对地址。</param>
        void CreateZip(string zipFilePath, string directoryPath);

        /// <summary>
        /// 解压缩文件夹。
        /// </summary>
        /// <param name="zipFilePath">需要解压缩的zip文件绝对地址。</param>
        /// <param name="directoryPath">解压缩后的文件夹绝对地址。</param>
        void ExtractZip(string zipFilePath, string directoryPath);
    }
}