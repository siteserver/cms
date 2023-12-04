using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.V1
{
    public partial class ActionsController
    {
        [OpenApiOperation("生成页面 API", "生成页面，使用POST发起请求，请求地址为/api/v1/actions/create。")]
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] CreateRequest request)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeOthers))
            {
                return Unauthorized();
            }
            
            var created = false;

            if (request.Type == CreateType.Index)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateIndex))
                {
                    return Unauthorized();
                }

                await _createManager.CreateChannelAsync(request.SiteId, request.SiteId);
                created = true;
            }
            else if (request.Type == CreateType.Channel)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateChannels))
                {
                    return Unauthorized();
                }

                foreach (var channelId in request.ChannelIds)
                {
                    if (channelId <= 0)
                    {
                        continue;
                    }

                    await _createManager.CreateChannelAsync(request.SiteId, channelId);
                    created = true;
                }
            }
            else if (request.Type == CreateType.Content)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateContents))
                {
                    return Unauthorized();
                }

                foreach (var channelContentId in request.ChannelContentIds)
                {
                    if (channelContentId.ChannelId <= 0 || channelContentId.ContentId <= 0)
                    {
                        continue;
                    }

                    await _createManager.CreateContentAsync(request.SiteId, channelContentId.ChannelId, channelContentId.ContentId);
                    created = true;
                }
            }
            else if (request.Type == CreateType.AllContent)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateContents))
                {
                    return Unauthorized();
                }

                foreach (var channelId in request.ChannelIds)
                {
                    if (channelId <= 0)
                    {
                        continue;
                    }
                    
                    await _createManager.CreateAllContentAsync(request.SiteId, channelId);
                    created = true;
                }
            }
            else if (request.Type == CreateType.File)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateFiles))
                {
                    return Unauthorized();
                }

                var templateId = await _templateRepository.GetTemplateIdByTemplateNameAsync(request.SiteId, TemplateType.FileTemplate, request.Name);
                if (templateId > 0)
                {
                    await _createManager.CreateFileAsync(request.SiteId, templateId);
                    created = true;
                }
            }
            else if (request.Type == CreateType.Special)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll)
                 && !await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateSpecials))
                {
                    return Unauthorized();
                }

                var specialId = await _specialRepository.GetSpecialIdByTitleAsync(request.SiteId, request.Name);
                if (specialId > 0)
                {
                    await _createManager.CreateSpecialAsync(request.SiteId, specialId);
                    created = true;
                }
            }
            else if (request.Type == CreateType.All)
            {
                if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.CreateAll))
                {
                    return Unauthorized();
                }

                await _createManager.CreateByAllAsync(request.SiteId);
                created = true;
            }

            return new BoolResult
            {
                Value = created
            };
        }
    }
}
