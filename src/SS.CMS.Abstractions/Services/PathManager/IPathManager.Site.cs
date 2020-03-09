using System;
using SS.CMS.Abstractions.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    partial interface IPathManager
    {
        string GetWebRootUrl(params string[] paths);

        string GetWebRootPath(params string[] paths);

        string GetAdminUrl(string relatedUrl);

        string GetHomeUrl(string relatedUrl);

        string GetTemporaryFilesUrl(string relatedUrl);

        string GetSiteTemplatesUrl(string relatedUrl);

        string ParsePluginUrl(string pluginId, string url);

        string GetRootUrlByPhysicalPath(string physicalPath);

        string ParseNavigationUrl(string url);

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        Task<string> ParseNavigationUrlAsync(Site site, string url, bool isLocal);

        Task<string> GetSiteUrlAsync(Site site, bool isLocal);

        Task<string> GetSiteUrlAsync(Site site, string requestPath, bool isLocal);

        string GetLocalSiteUrl(int siteId);

        string GetLocalChannelUrl(int siteId, int channelId);

        string GetLocalContentUrl(int siteId, int channelId, int contentId);

        string GetContentPreviewUrl(int siteId, int channelId, int contentId, int previewId);

        string GetLocalFileUrl(int siteId, int fileTemplateId);

        string GetLocalSpecialUrl(int siteId, int specialId);

        Task<string> GetSiteUrlByPhysicalPathAsync(Site site, string physicalPath, bool isLocal);

        Task<string> GetRemoteSiteUrlAsync(Site site, string requestPath);

        Task<string> GetLocalSiteUrlAsync(Site site, string requestPath = null);

        // 得到发布系统首页地址
        Task<string> GetIndexPageUrlAsync(Site site, bool isLocal);

        Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal);

        Task<string> GetContentUrlAsync(Site site, Content content, bool isLocal);

        Task<string> GetContentUrlAsync(Site site, Channel channel, int contentId, bool isLocal);

        Task<string> GetContentUrlByIdAsync(Site site, Content contentCurrent, bool isLocal);

        Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal);

        Task<string> GetChannelUrlNotComputedAsync(Site site, int channelId, bool isLocal);

        //得到栏目经过计算后的连接地址
        Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal);

        string RemoveDefaultFileName(Site site, string url);

        Task<string> GetInputChannelUrlAsync(Site site, Channel node, bool isLocal);

        string AddVirtualToUrl(string url);

        string GetVirtualUrl(Site site, string url);

        bool IsVirtualUrl(string url);

        bool IsRelativeUrl(string url);

        string GetSiteFilesUrl(string relatedUrl);

        string GetSiteFilesUrl(string apiUrl, string relatedUrl);

        List<Select<string>> GetLinkTypeSelects();

        Task<string> GetSitePathAsync(Site site);

        Task<string> GetSitePathAsync(Site site, params string[] paths);

        Task<string> GetSitePathAsync(int siteId, params string[] paths);

        Task<string> GetIndexPageFilePathAsync(Site site, string createFileFullName, bool root, int currentPageIndex);

        string GetBackupFilePath(Site site, BackupType backupType);

        Task<string> GetUploadDirectoryPathAsync(Site site, string fileExtension);

        Task<string> GetUploadDirectoryPathAsync(Site site, DateTime datetime, string fileExtension);

        Task<string> GetUploadDirectoryPathAsync(Site site, UploadType uploadType);

        Task<string> GetUploadDirectoryPathAsync(Site site, DateTime datetime, UploadType uploadType);

        string GetUploadFileName(Site site, string filePath);

        string GetUploadFileName(string filePath, bool isUploadChangeFileName);

        Task<Site> GetSiteAsync(string path);

        Task<string> GetSiteDirAsync(string path);

        Task<int> GetCurrentSiteIdAsync();

        string AddVirtualToPath(string path);

        Task<string> MapPathAsync(Site site, string virtualPath);

        Task<string> MapPathAsync(Site site, string virtualPath, bool isCopyToSite);

        string MapPath(string directoryPath, string virtualPath);

        //将编辑器中图片上传至本机
        Task<string> SaveImageAsync(Site site, string content);

        string GetTemporaryFilesPath(string relatedPath);

        string GetSiteTemplatesPath(string relatedPath);

        string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath);

        bool IsSystemFile(string fileName);

        bool IsSystemFileForChangeSiteType(string fileName);

        Task<string> GetChannelFilePathRuleAsync(Site site, int channelId);

        Task<string> GetChannelFilePathRuleAsync(int siteId, int channelId);

        Task<string> GetContentFilePathRuleAsync(Site site, int channelId);

        Task<string> GetContentFilePathRuleAsync(int siteId, int channelId);

        Task<string> GetChannelPageFilePathAsync(Site site, int channelId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, int contentId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, Content content, int currentPageIndex);

        bool IsImageExtensionAllowed(Site site, string fileExtention);

        bool IsImageSizeAllowed(Site site, long contentLength);

        bool IsVideoExtensionAllowed(Site site, string fileExtention);

        bool IsVideoSizeAllowed(Site site, int contentLength);

        bool IsFileExtensionAllowed(Site site, string fileExtention);

        bool IsFileSizeAllowed(Site site, int contentLength);

        bool IsUploadExtensionAllowed(UploadType uploadType, Site site, string fileExtention);

        bool IsUploadSizeAllowed(UploadType uploadType, Site site, int contentLength);

        string GetBinDirectoryPath(string relatedPath);

        string GetAdminDirectoryPath(string relatedPath);

        string GetHomeDirectoryPath(string relatedPath);

        string PhysicalSiteFilesPath { get; }

        string GetLibraryFilePath(string virtualUrl);

        Task DeleteSiteFilesAsync(Site site);

        Task ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir);

        Task ChangeToRootAsync(Site site, bool isMoveFiles);

        Task ChangeToSubSiteAsync(Site site, string siteDir, IList<string> directories, IList<string> files);

        bool IsSystemDirectory(string directoryName);
    }
}
