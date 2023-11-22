using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class CreateController : ControllerBase
    {
        private const string Route = "create";

        private readonly ICreateManager _createManager;
        private readonly IAuthManager _authManager;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISpecialRepository _specialRepository;

        public CreateController(ICreateManager createManager, IAuthManager authManager, ITemplateRepository templateRepository, ISpecialRepository specialRepository)
        {
            _createManager = createManager;
            _authManager = authManager;
            _templateRepository = templateRepository;
            _specialRepository = specialRepository;
        }

        public class ChannelContentId
        {
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public CreateType Type { get; set; }
            public List<int> ChannelIds { get; set; }
            public List<ChannelContentId> ChannelContentIds { get; set; }
            public string Name { get; set; }
        }
    }
}
