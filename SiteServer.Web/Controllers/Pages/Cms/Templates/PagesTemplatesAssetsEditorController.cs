using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesAssetsEditor")]
    public partial class PagesTemplateAssetsEditorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] FileRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var filePath = PathUtility.GetSitePath(site, request.DirectoryPath, request.FileName);
            var content = string.Empty;
            if (FileUtils.IsFileExists(filePath))
            {
                content = await FileUtils.ReadTextAsync(filePath);
            }
            var extName = PathUtils.GetExtension(filePath).ToLower();
            var path = string.Empty;

            if (StringUtils.EqualsIgnoreCase(extName, ".html"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsIncludeDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, ".css"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsCssDir,
                    string.Empty), request.FileName);
            }
            else if (StringUtils.EqualsIgnoreCase(extName, ".js"))
            {
                path = PageUtils.Combine(StringUtils.ReplaceStartsWithIgnoreCase(request.DirectoryPath, site.TemplatesAssetsJsDir,
                    string.Empty), request.FileName);
            }

            return new GetResult
            {
                Path = StringUtils.TrimSlash(PathUtils.RemoveExtension(path)),
                ExtName = extName,
                Content = content
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ContentResult> Add([FromBody] ContentRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ContentResult>();

            return await SaveFile(request, site, false);
        }

        [HttpPut, Route(Route)]
        public async Task<ContentResult> Edit([FromBody] ContentRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<ContentResult>();

            return await SaveFile(request, site, false);
        }

        private async Task<ContentResult> SaveFile(ContentRequest request, Site site, bool isAdd)
        {
            var filePath = string.Empty; 
            if (StringUtils.EqualsIgnoreCase(request.ExtName, ".html"))
            {
                filePath = PathUtility.GetSitePath(site, site.TemplatesAssetsIncludeDir, request.Path + ".html");
            }
            else if (StringUtils.EqualsIgnoreCase(request.ExtName, ".css"))
            {
                filePath = PathUtility.GetSitePath(site, site.TemplatesAssetsCssDir, request.Path + ".css");
            }
            else if (StringUtils.EqualsIgnoreCase(request.ExtName, ".js"))
            {
                filePath = PathUtility.GetSitePath(site, site.TemplatesAssetsJsDir, request.Path + ".js");
            }

            var filePathToDelete = string.Empty;
            if (isAdd)
            {
                if (FileUtils.IsFileExists(filePath))
                {
                    return Request.BadRequest<ContentResult>("文件新增失败，同名文件已存在！");
                }
            }
            else
            {
                var originalFilePath = PathUtility.GetSitePath(site, request.DirectoryPath, request.FileName);
                if (!StringUtils.EqualsIgnoreCase(originalFilePath, filePath))
                {
                    filePathToDelete = originalFilePath;
                    if (FileUtils.IsFileExists(filePath))
                    {
                        return Request.BadRequest<ContentResult>("文件编辑失败，同名文件已存在！");
                    }
                }
            }

            DirectoryUtils.CreateDirectoryIfNotExists(filePath);
            await FileUtils.WriteTextAsync(filePath, request.Content);
            if (!string.IsNullOrEmpty(filePathToDelete))
            {
                FileUtils.DeleteFileIfExists(filePathToDelete);
            }

            var fileName = PathUtils.GetFileName(filePath);
            var sitePath = PathUtility.GetSitePath(site);
            var directoryPath = StringUtils.ReplaceStartsWithIgnoreCase(filePath, sitePath, string.Empty);
            directoryPath = StringUtils.ReplaceEndsWithIgnoreCase(directoryPath, fileName, string.Empty);
            directoryPath = StringUtils.TrimSlash(directoryPath);

            return new ContentResult
            {
                DirectoryPath = directoryPath,
                FileName = fileName
            };
        }
    }
}
