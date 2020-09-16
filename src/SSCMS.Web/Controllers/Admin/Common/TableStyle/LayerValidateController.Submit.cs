using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerValidateController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var relatedIdentities = ListUtils.GetIntList(request.RelatedIdentities);
            var style =
                await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, relatedIdentities);
            style.Rules = request.Rules;

            //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
            if (style.Id == 0 && style.RelatedIdentity == 0 || style.RelatedIdentity != relatedIdentities[0])
            {
                await _tableStyleRepository.InsertAsync(relatedIdentities, style);
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
