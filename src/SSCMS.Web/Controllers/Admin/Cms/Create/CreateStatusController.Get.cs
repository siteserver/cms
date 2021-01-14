using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreateStatusController
    {
        [HttpGet, Route(Route)]
        public ActionResult<ObjectResult<CreateTaskSummary>> Get([FromQuery] SiteRequest request)
        {
            var summary = _createManager.GetTaskSummary(request.SiteId);

            return new ObjectResult<CreateTaskSummary>
            {
                Value = summary
            };
        }
    }
}
