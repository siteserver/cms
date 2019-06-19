using System;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Services.IPathManager
{
    public partial interface IPathManager
    {
        string GetSitePath(int siteId, string virtualPath);

        string MapPath(SiteInfo siteInfo, string virtualPath);

        string MapPath(SiteInfo siteInfo, string virtualPath, bool isCopyToSite);

        string GetUploadFileName(SiteInfo siteInfo, string filePath);

        string GetUploadFileName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName);

        string GetUploadSpecialName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName);

        string GetChannelFilePathRule(SiteInfo siteInfo, int channelId);

        string GetContentFilePathRule(SiteInfo siteInfo, int channelId);

        string GetChannelPageFilePath(SiteInfo siteInfo, int channelId, int currentPageIndex);

        string GetContentPageFilePath(SiteInfo siteInfo, int channelId, int contentId, int currentPageIndex);

        string GetContentPageFilePath(SiteInfo siteInfo, int channelId, ContentInfo contentInfo, int currentPageIndex);

        bool IsImageExtensionAllowed(SiteInfo siteInfo, string fileExtention);

        bool IsImageSizeAllowed(SiteInfo siteInfo, int contentLength);

        bool IsVideoExtensionAllowed(SiteInfo siteInfo, string fileExtention);

        bool IsVideoSizeAllowed(SiteInfo siteInfo, int contentLength);

        bool IsFileExtensionAllowed(SiteInfo siteInfo, string fileExtention);

        bool IsFileSizeAllowed(SiteInfo siteInfo, int contentLength);

        bool IsUploadExtensionAllowed(UploadType uploadType, SiteInfo siteInfo, string fileExtention);

        bool IsUploadSizeAllowed(UploadType uploadType, SiteInfo siteInfo, int contentLength);

        string GetSitePath(SiteInfo siteInfo);

        string GetSitePath(int siteId, params string[] paths);

        string GetSitePath(SiteInfo siteInfo, params string[] paths);

        string GetIndexPageFilePath(SiteInfo siteInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex);

        string GetUploadDirectoryPath(SiteInfo siteInfo, string fileExtension);

        string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, string fileExtension);

        string GetUploadDirectoryPath(SiteInfo siteInfo, UploadType uploadType);

        string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, UploadType uploadType);

        int GetSiteIdByFilePath(string path);

        string GetSitePath(int siteId);

        string GetUploadFilePath(int siteId, string fileName);

        SiteInfo GetSiteInfoByFilePath(string path);

        string GetSiteDirByFilePath(string path);
    }
}