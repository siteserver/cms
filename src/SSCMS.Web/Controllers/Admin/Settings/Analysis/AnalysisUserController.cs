using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisUserController : ControllerBase
    {
        private const string Route = "settings/analysisUser";

        private readonly IAuthManager _authManager;
        private readonly IStatRepository _statRepository;

        public AnalysisUserController(IAuthManager authManager, IStatRepository statRepository)
        {
            _authManager = authManager;
            _statRepository = statRepository;
        }

        public class GetRequest
        {
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public class GetStat
        {
            public string Date { get; set; }
            public int Register { get; set; }
            public int Login { get; set; }
        }

        public class GetResult
        {
            public List<string> Days { get; set; }
            public List<int> RegisterCount { get; set; }
            public List<int> LoginCount { get; set; }
        }
    }
}
