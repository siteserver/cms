using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    
    [RoutePrefix("pages/shared/tableStyleLayerValidate")]
    public partial class PagesTableStyleLayerValidateController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri]GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var style = await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities);

            var options = TranslateUtils.GetEnums<ValidateType>().Select(validateType =>
                new Select<string>(validateType.GetValue(), validateType.GetDisplayName()));

            return new GetResult
            {
                Options = options,
                Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues)
            };
        }

        [HttpPost, Route(Route)]
        public async Task<DefaultResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<DefaultResult>();

            var style =
                await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities);
            style.RuleValues = TranslateUtils.JsonSerialize(request.Rules);

            //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
            if (style.Id == 0 && style.RelatedIdentity == 0 || style.RelatedIdentity != request.RelatedIdentities[0])
            {
                await DataProvider.TableStyleRepository.InsertAsync(request.RelatedIdentities, style);
                await auth.AddAdminLogAsync("添加表单显示样式", $"字段名:{style.AttributeName}");
            }
            //数据库中有此项的表样式
            else
            {
                await DataProvider.TableStyleRepository.UpdateAsync(style);
                await auth.AddAdminLogAsync("修改表单显示样式", $"字段名:{style.AttributeName}");
            }

            return new DefaultResult
            {
                Value = true
            };
        }
    }
}
