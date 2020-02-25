using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Api.Preview;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("admin/redirect")]
    public partial class RedirectController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public RedirectController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            var url = string.Empty;

            if (request.SiteId > 0 && request.ChannelId > 0 && request.ContentId > 0)
            {
                var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
                url = await PageUtility.GetContentUrlAsync(site, channelInfo, request.ContentId, request.IsLocal);
            }
            else if (request.SiteId > 0 && request.ChannelId > 0)
            {
                var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
                url = await PageUtility.GetChannelUrlAsync(site, channelInfo, request.IsLocal);
            }
            else if (request.SiteId > 0 && request.FileTemplateId > 0)
            {
                url = await PageUtility.GetFileUrlAsync(site, request.FileTemplateId, request.IsLocal);
            }
            else if (request.SiteId > 0 && request.SpecialId > 0)
            {
                url = await PageUtility.GetSpecialUrlAsync(site, request.SpecialId, request.IsLocal);
            }
            else if (request.SiteId > 0)
            {
                var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
                url = await PageUtility.GetChannelUrlAsync(site, channelInfo, request.IsLocal);
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

            if (string.IsNullOrEmpty(url) || StringUtils.EqualsIgnoreCase(url, PageUtils.UnClickableUrl))
            {
                if (request.SiteId == 0)
                {
                    request.SiteId = await DataProvider.SiteRepository.GetIdByIsRootAsync();
                }
                if (request.SiteId != 0)
                {
                    site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
                    url = site.IsSeparatedWeb
                        ? ApiRoutePreview.GetSiteUrl(request.SiteId)
                        : await site.GetWebUrlAsync();
                }
                else
                {
                    url = PageUtils.ApplicationPath;
                }
            }

            return new StringResult
            {
                Value = url
            };
        }
    }
}