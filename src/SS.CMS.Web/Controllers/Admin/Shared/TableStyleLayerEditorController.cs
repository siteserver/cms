using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    [Route("admin/shared/tableStyleLayerEditor")]
    public partial class TableStyleLayerEditorController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ITableStyleRepository _tableStyleRepository;

        public TableStyleLayerEditorController(IAuthManager authManager, IDatabaseManager databaseManager, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var style = await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities) ?? new TableStyle
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync()) return Unauthorized();

            var styleDatabase =
                await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities) ??
                new TableStyle();

            bool isSuccess;
            string errorMessage;

            //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
            if (styleDatabase.Id == 0 && styleDatabase.RelatedIdentity == 0 || styleDatabase.RelatedIdentity != request.RelatedIdentities[0])
            {
                (isSuccess, errorMessage) = await InsertTableStyleAsync(request);
                await _authManager.AddAdminLogAsync("添加表单显示样式", $"字段名:{request.AttributeName}");
            }
            //数据库中有此项的表样式
            else
            {
                (isSuccess, errorMessage) = await UpdateTableStyleAsync(styleDatabase, request);
                await _authManager.AddAdminLogAsync("修改表单显示样式", $"字段名:{request.AttributeName}");
            }

            if (!isSuccess)
            {
                return this.Error(errorMessage);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
