using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
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
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var directories = new List<Cascade<string>>();
            var files = new List<KeyValuePair<string, string>>();
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);

            var siteUrl = PageUtility.GetSiteUrl(site, string.Empty, true).TrimEnd('/');

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
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            FileUtils.DeleteFileIfExists(PathUtility.GetSitePath(site, request.DirectoryPath, request.FileName));
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
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateAssets);

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            site.TemplatesAssetsIncludeDir = request.IncludeDir.Trim('/');
            site.TemplatesAssetsCssDir = request.CssDir.Trim('/');
            site.TemplatesAssetsJsDir = request.JsDir.Trim('/');

            await DataProvider.SiteRepository.UpdateAsync(site);
            await auth.AddSiteLogAsync(request.SiteId, "资源文件文件夹设置");

            var directories = new List<Cascade<string>>();
            var files = new List<KeyValuePair<string, string>>();
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsIncludeDir, ExtInclude);
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsCssDir, ExtCss);
            GetDirectoriesAndFiles(directories, files, site, site.TemplatesAssetsJsDir, ExtJs);

            var siteUrl = PageUtility.GetSiteUrl(site, string.Empty, true).TrimEnd('/');

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
