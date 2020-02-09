using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/tableStyleLayerEditor")]
    public partial class PagesTableStyleLayerEditorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<GetResult>();

            var style = await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities) ?? new TableStyle
            {
                InputType = InputType.Text
            };
            if (style.Items == null)
            {
                style.Items = new List<TableStyleItem>();
            }

            var isRapid = true;
            var rapidValues = string.Empty;
            if (style.Items.Count == 0)
            {
                style.Items.Add(new TableStyleItem
                {
                    Label = string.Empty,
                    Value = string.Empty,
                    Selected = false
                });
            }
            else
            {
                var isSelected = false;
                var isNotEquals = false;
                var list = new List<string>();
                foreach (var item in style.Items)
                {
                    list.Add(item.Value);
                    if (item.Selected)
                    {
                        isSelected = true;
                    }
                    if (item.Value != item.Label)
                    {
                        isNotEquals = true;
                    }
                }

                isRapid = !isSelected && !isNotEquals;
                rapidValues = Utilities.ToString(list);
            }

            var form = new SubmitRequest
            {
                TableName = style.TableName,
                AttributeName = style.AttributeName,
                RelatedIdentities = request.RelatedIdentities,
                IsRapid = isRapid,
                RapidValues = rapidValues,
                Taxis = style.Taxis,
                DisplayName = style.DisplayName,
                HelpText = style.HelpText,
                InputType = style.InputType,
                DefaultValue = style.DefaultValue,
                Horizontal = style.Horizontal,
                Items = style.Items,
                Height = style.Height,
                CustomizeLeft = style.CustomizeLeft,
                CustomizeRight = style.CustomizeRight
            };

            return new GetResult
            {
                InputTypes = InputTypeUtils.GetInputTypes(),
                Form = form
            };
        }

        [HttpPost, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin) return Request.Unauthorized<BoolResult>();

            var styleDatabase =
                await DataProvider.TableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities) ??
                new TableStyle();

            bool isSuccess;
            string errorMessage;

            //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
            if (styleDatabase.Id == 0 && styleDatabase.RelatedIdentity == 0 || styleDatabase.RelatedIdentity != request.RelatedIdentities[0])
            {
                (isSuccess, errorMessage) = await InsertTableStyleAsync(request);
                await auth.AddAdminLogAsync("添加表单显示样式", $"字段名:{request.AttributeName}");
            }
            //数据库中有此项的表样式
            else
            {
                (isSuccess, errorMessage) = await UpdateTableStyleAsync(styleDatabase, request);
                await auth.AddAdminLogAsync("修改表单显示样式", $"字段名:{request.AttributeName}");
            }

            if (!isSuccess)
            {
                return Request.BadRequest<BoolResult>(errorMessage);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
