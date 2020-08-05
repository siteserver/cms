using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Wx
{
    [OpenApiIgnore]
    [Route(Constants.ApiWxPrefix)]
    public partial class IndexController : ControllerBase
    {
        public const string Route = "{siteId}";

        private readonly IWxManager _wxManager;
        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public IndexController(IWxManager wxManager, IWxAccountRepository wxAccountRepository, IErrorLogRepository errorLogRepository)
        {
            _wxManager = wxManager;
            _wxAccountRepository = wxAccountRepository;
            _errorLogRepository = errorLogRepository;
        }
    }
}
