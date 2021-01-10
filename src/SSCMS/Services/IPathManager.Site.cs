using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Services
{
    partial interface IPathManager
    {
        //根据发布系统属性判断是否为相对路径并返回解析后路径
        Task<string> ParseSiteUrlAsync(Site site, string url, bool isLocal);

        Task<string> ParseSitePathAsync(Site site, string virtualPath);

        Task<string> GetSiteUrlAsync(Site site, bool isLocal);

        Task<string> GetSiteUrlAsync(Site site, string requestPath, bool isLocal);

        string GetPreviewSiteUrl(int siteId);

        string GetPreviewChannelUrl(int siteId, int channelId);

        string GetPreviewContentUrl(int siteId, int channelId, int contentId, bool isPreview = false);

        string GetPreviewFileUrl(int siteId, int fileTemplateId);

        string GetPreviewSpecialUrl(int siteId, int specialId);

        Task<string> GetSiteUrlByPhysicalPathAsync(Site site, string physicalPath, bool isLocal);

        Task<string> GetVirtualUrlByPhysicalPathAsync(Site site, string physicalPath);

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

        Task<string> GetBaseUrlAsync(Site site, Template template, int channelId, int contentId);

        string RemoveDefaultFileName(Site site, string url);

        Task<string> GetInputChannelUrlAsync(Site site, Channel node, bool isLocal);

        string AddVirtualToUrl(string url);

        string GetVirtualUrl(Site site, string url);

        bool IsVirtualUrl(string url);

        bool IsRelativeUrl(string url);

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

        Task<Site> GetSiteAsync(string path);

        Task<string> GetSiteDirAsync(string path);

        Task<int> GetCurrentSiteIdAsync();

        string AddVirtualToPath(string path);

        //将编辑器中图片上传至本机
        Task<string> SaveImageAsync(Site site, string content);

        string GetTemporaryFilesPath(string relatedPath);

        string GetSiteTemplatesPath(string relatedPath);

        string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath);

        Task<string> GetChannelFilePathRuleAsync(Site site, int channelId);

        Task<string> GetChannelFilePathRuleAsync(int siteId, int channelId);

        Task<string> GetContentFilePathRuleAsync(Site site, int channelId);

        Task<string> GetContentFilePathRuleAsync(int siteId, int channelId);

        Task<string> GetChannelPageFilePathAsync(Site site, int channelId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, int contentId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site site, int channelId, Content content, int currentPageIndex);

        bool IsImageExtensionAllowed(Site site, string fileExtension);

        bool IsImageSizeAllowed(Site site, long contentLength);

        bool IsVideoExtensionAllowed(Site site, string fileExtension);

        bool IsVideoSizeAllowed(Site site, long contentLength);

        bool IsAudioExtensionAllowed(Site site, string fileExtension);

        bool IsAudioSizeAllowed(Site site, long contentLength);

        bool IsFileExtensionAllowed(Site site, string fileExtension);

        bool IsFileSizeAllowed(Site site, long contentLength);

        string GetBinDirectoryPath(string relatedPath);

        string PhysicalSiteFilesPath { get; }

        Task DeleteSiteFilesAsync(Site site);

        Task ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir);

        Task ChangeToRootAsync(Site site, bool isMoveFiles);

        Task ChangeToSubSiteAsync(Site site, string siteDir, IList<string> directories, IList<string> files);

        bool IsSystemDirectory(string directoryName);

        Task AddWaterMarkAsync(Site site, string imagePath);

        Task MoveFileAsync(Site sourceSite, Site destSite, string relatedUrl);

        Task MoveFileByContentAsync(Site sourceSite, Site destSite, Content content);
    }
}
