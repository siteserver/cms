using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerValidateController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var relatedIdentities = ListUtils.GetIntList(request.RelatedIdentities);
            var style = await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, relatedIdentities);

            var options = ListUtils.GetEnums<ValidateType>().Select(validateType =>
                new Select<string>(validateType.GetValue(), validateType.GetDisplayName()));

            return new GetResult
            {
                Options = options,
                Rules = TranslateUtils.JsonDeserialize<IEnumerable<InputStyleRule>>(style.RuleValues)
            };
        }
    }
}
