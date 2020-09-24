using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Configs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ConfigsHomeController : ControllerBase
    {
        private const string Route = "settings/configsHome";
        private const string RouteUpload = "settings/configsHome/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ConfigsHomeController(IAuthManager authManager, IPathManager pathManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public Config Config { get; set; }
            public List<TableStyle> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsHomeClosed { get; set; }
            public string HomeTitle { get; set; }
            public bool IsHomeLogo { get; set; }
            public string HomeLogoUrl { get; set; }
            public string HomeDefaultAvatarUrl { get; set; }
            public List<string> UserRegistrationAttributes { get; set; }
            public bool IsUserRegistrationGroup { get; set; }
            public bool IsHomeAgreement { get; set; }
            public string HomeAgreementHtml { get; set; }
            public string HomeWelcomeHtml { get; set; }
        }
    }
}
