using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public partial class ActionsIfController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly IUserGroupRepository _userGroupRepository;

        public ActionsIfController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, IUserGroupRepository userGroupRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _userGroupRepository = userGroupRepository;
        }

        public class GetRequest
        {
            public string Value { get; set; }
            public int Page { get; set; }
        }

        public class GetResult
        {
            public bool Value { get; set; }
            public string Html { get; set; }
        }
    }
}
