using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SSCMS.Web.Controllers.Home
{
    public partial class VerifyMobileController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var user = await _authManager.GetUserAsync();

            return new GetResult
            {
                Mobile = user.Mobile
            };
        }
    }
}
