using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto;
using SSCMS.Dto.Result;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/tableStyleLayerValidate")]
    public partial class TableStyleLayerValidateController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ITableStyleRepository _tableStyleRepository;

        public TableStyleLayerValidateController(IAuthManager authManager, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var style = await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities);

            var options = TranslateUtils.GetEnums<ValidateType>().Select(validateType =>
                new Select<string>(validateType.GetValue(), validateType.GetDisplayName()));

            return new GetResult
            {
                Options = options,
                Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var style =
                await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities);
            style.RuleValues = TranslateUtils.JsonSerialize(request.Rules);

            //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
            if (style.Id == 0 && style.RelatedIdentity == 0 || style.RelatedIdentity != request.RelatedIdentities[0])
            {
                await _tableStyleRepository.InsertAsync(request.RelatedIdentities, style);
                await _authManager.AddAdminLogAsync("添加表单显示样式", $"字段名:{style.AttributeName}");
            }
            //数据库中有此项的表样式
            else
            {
                await _tableStyleRepository.UpdateAsync(style);
                await _authManager.AddAdminLogAsync("修改表单显示样式", $"字段名:{style.AttributeName}");
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
