using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin
{
    public partial class LoadingController
    {
        [HttpPost, Route(Route)]
        public ActionResult<StringResult> Submit([FromBody] SubmitRequest request)
        {
            return new StringResult
            {
                Value = _settingsManager.Decrypt(request.RedirectUrl)
            };
        }
    }
}
