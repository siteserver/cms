using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    public partial class LayerEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var relatedIdentities = ListUtils.GetIntList(request.RelatedIdentities);
            var style = await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, relatedIdentities) ?? new Models.TableStyle
            {
                InputType = InputType.Text
            };
            if (style.Items == null)
            {
                style.Items = new List<InputStyleItem>();
            }

            var isRapid = true;
            var rapidValues = string.Empty;
            if (style.Items.Count == 0)
            {
                style.Items.Add(new InputStyleItem
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
                rapidValues = ListUtils.ToStringByReturnAndNewline(list);
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
    }
}
