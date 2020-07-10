using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.ToDel
{
    public partial class ContentsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]ChannelRequest request)
        {
            var sites = new List<SiteResult>();
            var channels = new List<ChannelResult>();
            SiteResult siteResult = null;
            ChannelResult channelResult = null;

            Site site = null;
            Channel channelInfo = null;
            var siteIdList = await _authManager.GetSiteIdsAsync();
            foreach (var siteId in siteIdList)
            {
                var permissionSite = await _siteRepository.GetAsync(siteId);
                if (request.SiteId == siteId)
                {
                    site = permissionSite;
                }
                sites.Add(new SiteResult
                {
                    Id = permissionSite.Id,
                    SiteName = permissionSite.SiteName
                });
            }

            if (site == null && siteIdList.Count > 0)
            {
                site = await _siteRepository.GetAsync(siteIdList[0]);
            }

            if (site != null)
            {
                var channelIdList = await _authManager.GetChannelIdsAsync(site.Id,
                    AuthTypes.ContentPermissions.Add);
                foreach (var permissionChannelId in channelIdList)
                {
                    var permissionChannelInfo = await _channelRepository.GetAsync(permissionChannelId);
                    if (channelInfo == null || request.ChannelId == permissionChannelId)
                    {
                        channelInfo = permissionChannelInfo;
                    }

                    channels.Add(new ChannelResult
                    {
                        Id = permissionChannelInfo.Id,
                        ChannelName =
                            await _channelRepository.GetChannelNameNavigationAsync(site.Id, permissionChannelId)
                    });
                }

                siteResult = new SiteResult
                {
                    Id = site.Id,
                    SiteName = site.SiteName,
                    SiteUrl = await _pathManager.GetSiteUrlAsync(site, false)
                };
            }

            if (channelInfo != null)
            {
                channelResult = new ChannelResult
                {
                    Id = channelInfo.Id,
                    ChannelName = await _channelRepository.GetChannelNameNavigationAsync(site.Id, channelInfo.Id)
                };
            }

            return new GetResult
            {
                Sites = sites,
                Channels = channels,
                Site = siteResult,
                Channel = channelResult
            };
        }

        public class SiteResult
        {
            public int Id { get; set; }
            public string SiteName { get; set; }
            public string SiteUrl { get; set; }
        }

        public class ChannelResult
        {
            public int Id { get; set; }
            public string ChannelName { get; set; }
        }

        public class GetResult
        {
            public List<SiteResult> Sites { get; set; }
            public List<ChannelResult> Channels { get; set; }
            public SiteResult Site { get; set; }
            public ChannelResult Channel { get; set; }
        }
    }
}
