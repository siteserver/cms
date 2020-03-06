using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;

namespace SS.CMS.Web.Controllers.Admin
{
    [Route("admin/loading")]
    public partial class LoadingController : ControllerBase
    {
        private const string Route = "";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;

        public LoadingController(ISettingsManager settingsManager, IAuthManager authManager)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            return new StringResult
            {
                Value = _settingsManager.Decrypt(request.RedirectUrl)
            };
        }
    }
}