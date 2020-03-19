using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto.Result;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/tableStyleLayerAddMultiple")]
    public partial class TableStyleLayerAddMultipleController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ITableStyleRepository _tableStyleRepository;

        public TableStyleLayerAddMultipleController(IAuthManager authManager, IDatabaseManager databaseManager, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var styles = new List<Style>
            {
                new Style {
                    InputType = InputType.Text
                }
            };

            return new GetResult
            {
                InputTypes = InputTypeUtils.GetInputTypes(),
                Styles = styles
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            foreach (var style in request.Styles)
            {
                var styleDatabase =
                    await _tableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, request.RelatedIdentities) ??
                    new TableStyle();

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if ((styleDatabase.Id != 0 || styleDatabase.RelatedIdentity != 0) &&
                    styleDatabase.RelatedIdentity == request.RelatedIdentities[0]) continue;
                var relatedIdentity = request.RelatedIdentities[0];

                if (string.IsNullOrEmpty(style.AttributeName)) continue;

                if (await _tableStyleRepository.IsExistsAsync(relatedIdentity, request.TableName, style.AttributeName))
                    continue;

                var tableStyle = await _databaseManager.IsAttributeNameExistsAsync(request.TableName, style.AttributeName) ? await _tableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, request.RelatedIdentities) : new TableStyle();

                tableStyle.RelatedIdentity = relatedIdentity;
                tableStyle.TableName = request.TableName;
                tableStyle.AttributeName = style.AttributeName;
                tableStyle.DisplayName = style.DisplayName;
                tableStyle.InputType = style.InputType;

                await _tableStyleRepository.InsertAsync(request.RelatedIdentities, tableStyle);
            }

            await _authManager.AddAdminLogAsync("批量添加表单显示样式");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
