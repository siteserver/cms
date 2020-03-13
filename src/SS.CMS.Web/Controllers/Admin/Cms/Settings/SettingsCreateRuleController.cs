using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsCreateRule")]
    public partial class SettingsCreateRuleController : ControllerBase
    {
        private const string Route = "";
        private const string RouteGet = "{siteId:int}/{channelId:int}";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsCreateRuleController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                var filePath = await _pathManager.GetInputChannelUrlAsync(site, entity, false);
                var contentFilePathRule = string.IsNullOrEmpty(entity.ContentFilePathRule)
                    ? await _pathManager.GetContentFilePathRuleAsync(site, summary.Id)
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
        public async Task<ActionResult<ChannelResult>> Get(int siteId, int channelId)
        {
            

            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(siteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(channelId);

            var linkTypes = _pathManager.GetLinkTypeSelects();
            var filePath = string.IsNullOrEmpty(channel.FilePath) ? await _pathManager.GetInputChannelUrlAsync(site, channel, false) : channel.FilePath;
            var channelFilePathRule = string.IsNullOrEmpty(channel.ChannelFilePathRule) ? await _pathManager.GetChannelFilePathRuleAsync(site, channelId) : channel.ChannelFilePathRule;
            var contentFilePathRule = string.IsNullOrEmpty(channel.ContentFilePathRule) ? await _pathManager.GetContentFilePathRuleAsync(site, channelId) : channel.ContentFilePathRule;

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
        public async Task<ActionResult<GetResult>> Submit([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            if (request.ChannelId != request.SiteId)
            {
                channel.LinkUrl = request.LinkUrl;
                channel.LinkType = request.LinkType;

                var filePath = channel.FilePath;
                if (!string.IsNullOrEmpty(request.FilePath) && !StringUtils.EqualsIgnoreCase(filePath, request.FilePath))
                {
                    if (!DirectoryUtils.IsDirectoryNameCompliant(request.FilePath))
                    {
                        return this.Error("栏目页面路径不符合系统要求！");
                    }

                    if (PathUtils.IsDirectoryPath(request.FilePath))
                    {
                        request.FilePath = PageUtils.Combine(request.FilePath, "index.html");
                    }

                    var filePathArrayList = await _channelRepository.GetAllFilePathBySiteIdAsync(request.SiteId);
                    if (filePathArrayList.Contains(request.FilePath))
                    {
                        return this.Error("栏目修改失败，栏目页面路径已存在！");
                    }
                }

                if (request.FilePath != await _pathManager.GetInputChannelUrlAsync(site, channel, false))
                {
                    channel.FilePath = request.FilePath;
                }
            }

            if (!string.IsNullOrEmpty(request.ChannelFilePathRule))
            {
                var filePathRule = request.ChannelFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return this.Error("栏目页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return this.Error("栏目页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (!string.IsNullOrEmpty(request.ContentFilePathRule))
            {
                var filePathRule = request.ContentFilePathRule.Replace("|", string.Empty);
                if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                {
                    return this.Error("内容页面命名规则不符合系统要求！");
                }
                if (PathUtils.IsDirectoryPath(filePathRule))
                {
                    return this.Error("内容页面命名规则必须包含生成文件的后缀！");
                }
            }

            if (request.ChannelFilePathRule != await _pathManager.GetChannelFilePathRuleAsync(site, request.ChannelId))
            {
                channel.ChannelFilePathRule = request.ChannelFilePathRule;
            }
            if (request.ContentFilePathRule != await _pathManager.GetContentFilePathRuleAsync(site, request.ChannelId))
            {
                channel.ContentFilePathRule = request.ContentFilePathRule;
            }

            await _channelRepository.UpdateAsync(channel);

            await _createManager.CreateChannelAsync(request.SiteId, request.ChannelId);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "设置页面生成规则", $"栏目:{channel.ChannelName}");

            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async (summary) =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);
                var filePath = await _pathManager.GetInputChannelUrlAsync(site, entity, false);
                var contentFilePathRule = string.IsNullOrEmpty(entity.ContentFilePathRule)
                    ? await _pathManager.GetContentFilePathRuleAsync(site, summary.Id)
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