using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCreateTriggerController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCreateTrigger";
        private const string RouteEdit = "cms/settings/settingsCreateTrigger/actions/edit";
        private const string RouteEditSelected = "cms/settings/settingsCreateTrigger/actions/editSelected";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsCreateTriggerController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channel { get; set; }
            public List<int> AllChannelIds { get; set; }
        }

        public class EditRequest : ChannelRequest
        {
            public bool IsCreateChannelIfContentChanged { get; set; }
            public List<int> CreateChannelIdsIfContentChanged { get; set; }
        }

        public class EditSelectedRequest : SiteRequest
        {
            public List<int> ChannelIds { get; set; }
            public List<int> CreateChannelIdsIfContentChanged { get; set; }
        }
    }
}