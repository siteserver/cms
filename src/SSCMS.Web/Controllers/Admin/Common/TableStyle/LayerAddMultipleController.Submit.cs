using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerAddMultipleController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var relatedIdentities = ListUtils.GetIntList(request.RelatedIdentities);

            foreach (var style in request.Styles)
            {
                var styleDatabase =
                    await _tableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, relatedIdentities) ??
                    new Models.TableStyle();

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if ((styleDatabase.Id != 0 || styleDatabase.RelatedIdentity != 0) &&
                    styleDatabase.RelatedIdentity == relatedIdentities[0]) continue;
                var relatedIdentity = relatedIdentities[0];

                if (string.IsNullOrEmpty(style.AttributeName)) continue;

                if (await _tableStyleRepository.IsExistsAsync(relatedIdentity, request.TableName, style.AttributeName))
                    continue;

                var tableStyle = await _databaseManager.IsAttributeNameExistsAsync(request.TableName, style.AttributeName) ? await _tableStyleRepository.GetTableStyleAsync(request.TableName, style.AttributeName, relatedIdentities) : new Models.TableStyle();

                tableStyle.RelatedIdentity = relatedIdentity;
                tableStyle.TableName = request.TableName;
                tableStyle.AttributeName = style.AttributeName;
                tableStyle.DisplayName = style.DisplayName;
                tableStyle.InputType = style.InputType;

                await _tableStyleRepository.InsertAsync(relatedIdentities, tableStyle);
            }

            await _authManager.AddAdminLogAsync("批量添加表单显示样式");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
