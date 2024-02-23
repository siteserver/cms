using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesAssetsController
    {
        [HttpPost, Route(RouteConfig)]
        public async Task<ActionResult<BoolResult>> Config([FromBody] ConfigRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.TemplatesAssets))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            site.TemplatesAssetsCssDir = PathUtils.RemoveParentPath(request.CssDir).Trim('/');
            site.TemplatesAssetsJsDir = PathUtils.RemoveParentPath(request.JsDir).Trim('/');
            site.TemplatesAssetsImagesDir = PathUtils.RemoveParentPath(request.ImagesDir).Trim('/');

            if (site.Root)
            {
                if (StringUtils.EqualsIgnoreCase(site.TemplatesAssetsCssDir, Constants.AdminDirectory) || 
                    StringUtils.EqualsIgnoreCase(site.TemplatesAssetsCssDir, DirectoryUtils.SiteFiles.DirectoryName) || 
                    StringUtils.EqualsIgnoreCase(site.TemplatesAssetsJsDir, Constants.AdminDirectory) || 
                    StringUtils.EqualsIgnoreCase(site.TemplatesAssetsJsDir, DirectoryUtils.SiteFiles.DirectoryName) || 
                    StringUtils.EqualsIgnoreCase(site.TemplatesAssetsImagesDir, Constants.AdminDirectory) || 
                    StringUtils.EqualsIgnoreCase(site.TemplatesAssetsImagesDir, DirectoryUtils.SiteFiles.DirectoryName))
                {
                    return this.Error("文件夹路径不能设置为系统目录！");
                }
            }

            await _siteRepository.UpdateAsync(site);
            await _authManager.AddSiteLogAsync(request.SiteId, "资源文件文件夹设置");

            return new BoolResult
            {
                Value = true,
            };
        }
    }
}
