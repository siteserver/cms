using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/tableStyleLayerAddMultiple")]
    public partial class TableStyleLayerAddMultipleController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public TableStyleLayerAddMultipleController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin) return Unauthorized();

            foreach (var style in request.Styles)
            {
                var styleDatabase =
                    await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, request.RelatedIdentities) ??
                    new TableStyle();

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if ((styleDatabase.Id != 0 || styleDatabase.RelatedIdentity != 0) &&
                    styleDatabase.RelatedIdentity == request.RelatedIdentities[0]) continue;
                var relatedIdentity = request.RelatedIdentities[0];

                if (string.IsNullOrEmpty(style.AttributeName)) continue;

                if (await DataProvider.TableStyleRepository.IsExistsAsync(relatedIdentity, request.TableName, style.AttributeName))
                    continue;

                var tableStyle = await TableColumnManager.IsAttributeNameExistsAsync(request.TableName, style.AttributeName) ? await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, request.RelatedIdentities) : new TableStyle();

                tableStyle.RelatedIdentity = relatedIdentity;
                tableStyle.TableName = request.TableName;
                tableStyle.AttributeName = style.AttributeName;
                tableStyle.DisplayName = style.DisplayName;
                tableStyle.InputType = style.InputType;

                await DataProvider.TableStyleRepository.InsertAsync(request.RelatedIdentities, tableStyle);
            }

            await auth.AddAdminLogAsync("批量添加表单显示样式");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
