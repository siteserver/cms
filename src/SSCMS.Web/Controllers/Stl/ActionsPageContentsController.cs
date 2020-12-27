using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsPageContentsController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;

        public ActionsPageContentsController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
        }

        public class SubmitRequest : SiteRequest
        {
            public int PageChannelId { get; set; }
            public int TemplateId { get; set; }
            public int TotalNum { get; set; }
            public int PageCount { get; set; }
            public int CurrentPageIndex { get; set; }
            public string StlPageContentsElement { get; set; }
        }

        public class SubmitResult
        {
            public string Html { get; set; }
        }
    }
}
