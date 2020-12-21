using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        string GetRootUrl(params string[] paths);

        string GetRootUrlByPath(string physicalPath);

        string GetRootPath(params string[] paths);

        string GetTemporaryFilesPath(params string[] paths);

        string GetSiteTemplatesUrl(string relatedUrl);

        string ParseUrl(string url);

        string ParsePath(string directoryPath, string virtualPath);

        string GetSiteFilesPath(params string[] paths);

        string GetSiteFilesUrl(params string[] paths);

        string GetSiteFilesUrl(Site site, params string[] paths);

        string GetAdministratorUploadPath(int userId, params string[] paths);

        string GetAdministratorUploadUrl(int userId, params string[] paths);

        string GetUserUploadPath(int userId, params string[] paths);

        string GetUserUploadUrl(int userId, params string[] paths);

        string GetHomeUploadPath(params string[] paths);

        string GetHomeUploadUrl(params string[] paths);

        string GetTemporaryFilesUrl(params string[] paths);

        string DefaultAvatarUrl { get; }

        string GetUserUploadPath(int userId, string relatedPath);

        string GetUserUploadFileName(string filePath);

        string GetUserUploadUrl(int userId, string relatedUrl);

        string GetUserAvatarUrl(User user);

        string GetDownloadApiUrl(Site site, int channelId, int contentId, string fileUrl);

        string GetDownloadApiUrl(Site site, string fileUrl);

        string GetDownloadApiUrl(string filePath);

        string GetDynamicApiUrl(Site site);

        string GetIfApiUrl(Site site);

        string GetPageContentsApiUrl(Site site);

        string GetPageContentsApiParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement);

        string GetTriggerApiUrl(int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect);
    }
}
