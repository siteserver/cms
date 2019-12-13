using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Site
{
    
    [RoutePrefix("pages/settings/siteUrl")]
    public class PagesSiteUrlController : ApiController
    {
        private const string Route = "";
        private const string RouteWeb = "actions/web";
        private const string RouteApi = "actions/api";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSiteUrl))
                {
                    return Unauthorized();
                }

                var rootSiteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();
                var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
                var sites = new List<Abstractions.Site>();
                foreach (var siteId in siteIdList)
                {
                    sites.Add(await DataProvider.SiteRepository.GetAsync(siteId));
                }

                var config = await DataProvider.ConfigRepository.GetAsync();

                return Ok(new
                {
                    Value = sites,
                    RootSiteId = rootSiteId,
                    config.IsSeparatedApi,
                    config.SeparatedApiUrl
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteWeb)]
        public async Task<IHttpActionResult> EditWeb()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSiteUrl))
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var isSeparatedWeb = request.GetPostBool("isSeparatedWeb");
                var separatedWebUrl = request.GetPostString("separatedWebUrl");
                var isSeparatedAssets = request.GetPostBool("isSeparatedAssets");
                var assetsDir = request.GetPostString("assetsDir");
                var separatedAssetsUrl = request.GetPostString("separatedAssetsUrl");

                if (!string.IsNullOrEmpty(separatedWebUrl) && !separatedWebUrl.EndsWith("/"))
                {
                    separatedWebUrl = separatedWebUrl + "/";
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                site.IsSeparatedWeb = isSeparatedWeb;
                site.SeparatedWebUrl = separatedWebUrl;

                site.IsSeparatedAssets = isSeparatedAssets;
                site.SeparatedAssetsUrl = separatedAssetsUrl;
                site.AssetsDir = assetsDir;

                await DataProvider.SiteRepository.UpdateAsync(site);
                await request.AddSiteLogAsync(siteId, "修改站点访问地址");

                var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync(0);
                var sites = new List<Abstractions.Site>();
                foreach (var id in siteIdList)
                {
                    sites.Add(await DataProvider.SiteRepository.GetAsync(id));
                }

                return Ok(new
                {
                    Value = sites
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteApi)]
        public async Task<IHttpActionResult> EditApi()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsSiteUrl))
                {
                    return Unauthorized();
                }

                var isSeparatedApi = request.GetPostBool("isSeparatedApi");
                var separatedApiUrl = request.GetPostString("separatedApiUrl");

                var config = await DataProvider.ConfigRepository.GetAsync();

                config.IsSeparatedApi = isSeparatedApi;
                config.SeparatedApiUrl = separatedApiUrl;

                await DataProvider.ConfigRepository.UpdateAsync(config);

                await request.AddAdminLogAsync("修改API访问地址");

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
