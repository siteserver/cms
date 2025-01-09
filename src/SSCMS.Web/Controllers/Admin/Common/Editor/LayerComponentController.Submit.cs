using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.Editor
{
    public partial class LayerComponentController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<StringResult>> List([FromBody] SubmitRequest request)
        {
            var component = await _materialComponentRepository.GetAsync(request.ComponentId);
            var result = component.Content;
            foreach (var pair in request.Parameters)
            {
                result = StringUtils.Replace(result, "{" + pair.Key + "}", pair.Value);
            }

            return new StringResult
            {
                Value = result
            };
        }
    }
}
