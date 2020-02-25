using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("admin/loading")]
    public partial class LoadingController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public LoadingController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            return new StringResult
            {
                Value = TranslateUtils.DecryptStringBySecretKey(request.RedirectUrl, WebConfigUtils.SecretKey)
            };
        }
    }
}