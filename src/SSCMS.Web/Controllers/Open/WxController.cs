using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Open
{
    [OpenApiIgnore]
    [Route(Constants.ApiOpenPrefix)]
    public partial class WxController : ControllerBase
    {
        public const string Route = "wx/{siteId}";

        private readonly IOpenAccountRepository _openAccountRepository;

        public WxController(IOpenAccountRepository openAccountRepository)
        {
            _openAccountRepository = openAccountRepository;
        }
    }
}
