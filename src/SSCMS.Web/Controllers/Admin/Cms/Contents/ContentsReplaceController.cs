using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsReplaceController : ControllerBase
    {
        private const string Route = "cms/contents/contentsReplace";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsReplaceController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public List<Option<string>> Attributes { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public IEnumerable<int> ChannelIds { get; set; }
            public List<string> AttributeNames { get; set; }
            public string Replace { get; set; }
            public bool IsCaseSensitive { get; set; }
            public bool IsRegex { get; set; }
            public bool IsDescendant { get; set; }
            public string To { get; set; }
        }
    }
}
