using System;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPathManager
    {
        Task<string> GetSitePathAsync(int siteId, string virtualPath);

        string MapPath(Site siteInfo, string virtualPath);

        string MapPath(Site siteInfo, string virtualPath, bool isCopyToSite);

        string GetUploadFileName(Site siteInfo, string filePath);

        string GetUploadFileName(Site siteInfo, string filePath, bool isUploadChangeFileName);

        string GetUploadSpecialName(Site siteInfo, string filePath, bool isUploadChangeFileName);

        Task<string> GetChannelPageFilePathAsync(Site siteInfo, int channelId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site siteInfo, int channelId, int contentId, int currentPageIndex);

        Task<string> GetContentPageFilePathAsync(Site siteInfo, int channelId, Content contentInfo, int currentPageIndex);

        bool IsImageExtensionAllowed(Site siteInfo, string fileExtention);

        bool IsImageSizeAllowed(Site siteInfo, int contentLength);

        bool IsVideoExtensionAllowed(Site siteInfo, string fileExtention);

        bool IsVideoSizeAllowed(Site siteInfo, int contentLength);

        bool IsFileExtensionAllowed(Site siteInfo, string fileExtention);

        bool IsFileSizeAllowed(Site siteInfo, int contentLength);

        bool IsUploadExtensionAllowed(UploadType uploadType, Site siteInfo, string fileExtention);

        bool IsUploadSizeAllowed(UploadType uploadType, Site siteInfo, int contentLength);

        string GetSitePath(Site siteInfo);

        Task<string> GetSitePathAsync(int siteId, params string[] paths);

        string GetSitePath(Site siteInfo, params string[] paths);

        string GetIndexPageFilePath(Site siteInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex);

        string GetUploadDirectoryPath(Site siteInfo, string fileExtension);

        string GetUploadDirectoryPath(Site siteInfo, DateTime datetime, string fileExtension);

        string GetUploadDirectoryPath(Site siteInfo, UploadType uploadType);

        string GetUploadDirectoryPath(Site siteInfo, DateTime datetime, UploadType uploadType);

        Task<int> GetSiteIdByFilePathAsync(string path);

        Task<string> GetSitePathAsync(int siteId);

        Task<string> GetUploadFilePathAsync(int siteId, string fileName);

        Task<Site> GetSiteInfoByFilePathAsync(string path);

        Task<string> GetSiteDirByFilePathAsync(string path);
    }
}