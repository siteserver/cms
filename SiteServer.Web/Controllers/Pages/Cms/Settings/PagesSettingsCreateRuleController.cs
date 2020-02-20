using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsCreateRule")]
    public partial class PagesSettingsCreateRuleController : ApiController
    {
        private const string Route = "";
        private const string RouteGet = "{siteId:int}/{channelId:int}";

        private readonly ICreateManager _createManager;

        public PagesSettingsCreateRuleController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> List([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GetResult>("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                var filePath = await PageUtility.GetInputChannelUrlAsync(site, entity, false);
                var contentFilePathRule = string.IsNullOrEmpty(entity.ContentFilePathRule)
                    ? await PathUtility.GetContentFilePathRuleAsync(site, summary.Id)
                    : entity.ContentFilePathRule;
                return new
                {
                    entity.IndexName,
                    Count = count,
                    FilePath = filePath,
                    ContentFilePathRule = contentFilePathRule
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }

        [HttpGet, Route(RouteGet)]
        public async Task<ChannelResult> Get(int siteId, int channelId)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<ChannelResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site == null) return Request.BadRequest<ChannelResult>("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(channelId);

            var linkTypes = PageUtility.GetLinkTypeSelects();
            var filePath = string.IsNullOrEmpty(channel.FilePath) ? await PageUtility.GetInputChannelUrlAsync(site, channel, false) : channel.FilePath;
            var channelFilePathRule = string.IsNullOrEmpty(channel.ChannelFilePathRule) ? await PathUtility.GetChannelFilePathRuleAsync(site, channelId) : channel.ChannelFilePathRule;
            var contentFilePathRule = string.IsNullOrEmpty(channel.ContentFilePathRule) ? await PathUtility.GetContentFilePathRuleAsync(site, channelId) : channel.ContentFilePathRule;

            return new ChannelResult
            {
                Channel = channel,
                LinkTypes = linkTypes,
                FilePath = filePath,
                ChannelFilePathRule = channelFilePathRule,
                ContentFilePathRule = contentFilePathRule
            };
        }

        [HttpPut, Route(Route)]
        public async Task<GetResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GetResult>("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            if (request.ChannelId != request.SiteId)
            {
                channel.LinkUrl = request.LinkUrl;
                channel.LinkType = request.LinkType;

                var filePath = channel.FilePath;
                if (!string.IsNullOrEmpty(request.FilePath) && !StringUtils.EqualsIgnoreCase(filePath, request.FilePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                    {
                        return Request.BadRequest<GetResult>("栏目页面路径不符合系统要求！");
                    }

                    if (PathUtils.IsDirectoryPath(request.FilePath))
                    {
                        request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                    }

                    var filePathArrayList = await DataProvider.ChannelRepository.GetAllFilePathBySiteIdAsync(request.SiteId);
                    if (filePathArrayList.Contains(request.FilePath))
                    {
                        return Request.BadRequest<GetResult>("栏目修改失败，栏目页面路径已存在！");
                    }
                }

                if (request.FilePath != await PageUtility.GetInputChannelUrlAsync(site, channel, false))
                {
                    channel.FilePath = request.FilePath;
                }
            }

            if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
            {
                var filePathRule = request.ChannelFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return Request.BadRequest<GetResult>("栏目页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return Request.BadRequest<GetResult>("栏目页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (!string.IsNullOrEmpty(request.ContentFilePathRule))
            {
                var filePathRule = request.ContentFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return Request.BadRequest<GetResult>("内容页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return Request.BadRequest<GetResult>("内容页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (request.ChannelFilePathRule != await PathUtility.GetChannelFilePathRuleAsync(site, request.ChannelId))
            {
                channel.ChannelFilePathRule = request.ChannelFilePathRule;
            }
            if (request.ContentFilePathRule != await PathUtility.GetContentFilePathRuleAsync(site, request.ChannelId))
            {
                channel.ContentFilePathRule = request.ContentFilePathRule;
            }

            await DataProvider.ChannelRepository.UpdateAsync(channel);

            await _createManager.CreateChannelAsync(request.SiteId, request.ChannelId);

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "设置页面生成规则", $"栏目:{channel.ChannelName}");

            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async (summary) =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                var filePath = await PageUtility.GetInputChannelUrlAsync(site, entity, false);
                var contentFilePathRule = string.IsNullOrEmpty(entity.ContentFilePathRule)
                    ? await PathUtility.GetContentFilePathRuleAsync(site, summary.Id)
                    : entity.ContentFilePathRule;
                return new
                {
                    Count = count,
                    FilePath = filePath,
                    ContentFilePathRule = contentFilePathRule
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }
    }
}