using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages
{
    
    [RoutePrefix("pages/redirect")]
    public class PagesRedirectController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin)
                {
                    return Unauthorized();
                }

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentId = request.GetPostInt("contentId");
                var fileTemplateId = request.GetPostInt("fileTemplateId");
                var specialId = request.GetPostInt("specialId");
                var isLocal = request.GetPostBool("isLocal");

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                var url = string.Empty;

                if (siteId > 0 && channelId > 0 && contentId > 0)
                {
                    var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                    url = await PageUtility.GetContentUrlAsync(site, channelInfo, contentId, isLocal);
                }
                else if (siteId > 0 && channelId > 0)
                {
                    var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                    url = await PageUtility.GetChannelUrlAsync(site, channelInfo, isLocal);
                }
                else if (siteId > 0 && fileTemplateId > 0)
                {
                    url = await PageUtility.GetFileUrlAsync(site, fileTemplateId, isLocal);
                }
                else if (siteId > 0 && specialId > 0)
                {
                    url = await PageUtility.GetSpecialUrlAsync(site, specialId, isLocal);
                }
                else if (siteId > 0)
                {
                    var channelInfo = await ChannelManager.GetChannelAsync(siteId, siteId);
                    url = await PageUtility.GetChannelUrlAsync(site, channelInfo, isLocal);
                }

                //if (site.IsSeparatedWeb)
                //{
                //    if (siteId > 0 && channelId > 0 && contentId > 0)
                //    {
                //        url = PreviewApi.GetContentUrl(siteId, channelId, contentId);
                //    }
                //    else if (siteId > 0 && channelId > 0)
                //    {
                //        url = PreviewApi.GetChannelUrl(siteId, channelId);
                //    }
                //    else if (siteId > 0 && templateId > 0)
                //    {
                //        url = PreviewApi.GetFileUrl(siteId, templateId);
                //    }
                //    else if (siteId > 0)
                //    {
                //        url = PreviewApi.GetSiteUrl(siteId);
                //    }
                //}
                //else
                //{

                //}

                if (string.IsNullOrEmpty(url) || StringUtils.EqualsIgnoreCase(url, PageUtils.UnclickedUrl))
                {
                    if (siteId == 0)
                    {
                        siteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();
                    }
                    if (siteId != 0)
                    {
                        site = await DataProvider.SiteRepository.GetAsync(siteId);
                        url = site.IsSeparatedWeb
                            ? ApiRoutePreview.GetSiteUrl(siteId)
                            : site.GetWebUrl();
                    }
                    else
                    {
                        url = PageUtils.ApplicationPath;
                    }
                }

                return Ok(new
                {
                    Value = url
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}