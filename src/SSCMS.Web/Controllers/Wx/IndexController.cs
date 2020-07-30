using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Wx
{
    [OpenApiIgnore]
    [Route(Constants.ApiWxPrefix)]
    public partial class IndexController : ControllerBase
    {
        public const string Route = "{siteId}";

        private readonly IWxAccountRepository _wxAccountRepository;

        public IndexController(IWxAccountRepository wxAccountRepository)
        {
            _wxAccountRepository = wxAccountRepository;
        }
    }
}
