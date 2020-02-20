using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    [RoutePrefix("pages/cms/templates/templatesAssets")]
    public partial class PagesTemplateAssetsController : ApiController
    {
        private const string Route = "";
        private const string RouteConfig = "actions/config";

        private const string ExtInclude = ".html";
        private const string ExtCss = ".css";
        private const string ExtJs = ".js";

        [HttpGet, Route(Route)]
        public async Task<GetResult> List([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var directories = new List<Cascade<string>>();
            var files = new List<KeyValuePair<string, string>>();
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);

            var siteUrl = (await PageUtility.GetSiteUrlAsync(site, string.Empty, true)).TrimEnd('/');

            return new GetResult
            {
                Directories = directories,
                Files = files,
                SiteUrl = siteUrl,
                IncludeDir = site.TemplatesAssetsIncludeDir,
                CssDir = site.TemplatesAssetsCssDir,
                JsDir = site.TemplatesAssetsJsDir
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<BoolResult> Delete([FromBody] FileRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            FileUtils.DeleteFileIfExists(await PathUtility.GetSitePathAsync(site, request.DirectoryPath, request.FileName));
            await auth.AddSiteLogAsync(request.SiteId, "删除资源文件", $"{request.DirectoryPath}:{request.FileName}");

            return new BoolResult
            {
                Value = true
            };
        }

        [HttpPost, Route(RouteConfig)]
        public async Task<GetResult> Config([FromBody] ConfigRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateAssets))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            site.TemplatesAssetsIncludeDir = request.IncludeDir.Trim('/');
            site.TemplatesAssetsCssDir = request.CssDir.Trim('/');
            site.TemplatesAssetsJsDir = request.JsDir.Trim('/');

            await DataProvider.SiteRepository.UpdateAsync(site);
            await auth.AddSiteLogAsync(request.SiteId, "资源文件文件夹设置");

            var directories = new List<Cascade<string>>();
            var files = new List<KeyValuePair<string, string>>();
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
            await GetDirectoriesAndFilesAsync(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);

            var siteUrl = (await PageUtility.GetSiteUrlAsync(site, string.Empty, true)).TrimEnd('/');

            return new GetResult
            {
                Directories = directories,
                Files = files,
                SiteUrl = siteUrl,
                IncludeDir = site.TemplatesAssetsIncludeDir,
                CssDir = site.TemplatesAssetsCssDir,
                JsDir = site.TemplatesAssetsJsDir
            };
        }
    }
}
