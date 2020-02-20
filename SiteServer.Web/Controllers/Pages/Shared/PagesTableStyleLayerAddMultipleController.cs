using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/tableStyleLayerAddMultiple")]
    public partial class PagesTableStyleLayerAddMultipleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

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
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<BoolResult>();

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
