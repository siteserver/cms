using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Abstractions;
using SSCMS.Abstractions.Dto.Result;

namespace SSCMS.Controllers.Admin
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
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            return new StringResult
            {
                Value = _settingsManager.Decrypt(request.RedirectUrl)
            };
        }
    }
}