using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorLayerCensorController : ControllerBase
    {
        private const string Route = "cms/editor/editorLayerCensor";

        private readonly IAuthManager _authManager;
        private readonly ICensorManager _censorManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public EditorLayerCensorController(IAuthManager authManager, ICensorManager censorManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _censorManager = censorManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class SubmitRequest : ChannelRequest
        {
            public List<string> IgnoreWords { get; set; }
            public string WhiteListWord { get; set; }
            public CensorResult Results { get; set; }
        }

        public class SubmitResultItem
        {
          public string Value { get; set; }
          public string Label { get; set; }
          public int Count { get; set; }
          public string Message { get; set; }
          public List<string> Words { get; set; }
        }

        public class SubmitResult
        {
            public bool IsBadWords { get; set; }
            public List<string> ActiveNames { get; set; }
            public List<SubmitResultItem> Items { get; set; }
        }
    }
}
