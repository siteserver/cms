using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Wx
{
    [OpenApiIgnore]
    [Route(Constants.ApiWxPrefix)]
    public partial class PushController : ControllerBase
    {
        private const string Route = "{siteId}";

        private readonly ICacheManager _cacheManager;
        private readonly IWxManager _wxManager;
        private readonly IWxAccountRepository _wxAccountRepository;
        private readonly IWxChatRepository _wxChatRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public PushController(ICacheManager cacheManager, IWxManager wxManager, IWxAccountRepository wxAccountRepository, IWxChatRepository wxChatRepository, IErrorLogRepository errorLogRepository)
        {
            _cacheManager = cacheManager;
            _wxManager = wxManager;
            _wxAccountRepository = wxAccountRepository;
            _wxChatRepository = wxChatRepository;
            _errorLogRepository = errorLogRepository;
        }
    }
}