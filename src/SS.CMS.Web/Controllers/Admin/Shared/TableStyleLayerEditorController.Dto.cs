using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;

namespace SS.CMS.Web.Controllers.Admin.Shared
{
    public partial class TableStyleLayerEditorController
    {
        public class GetRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public List<int> RelatedIdentities { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<KeyValuePair<InputType, string>> InputTypes { get; set; }
            public SubmitRequest Form { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public List<int> RelatedIdentities { get; set; }
            public bool IsRapid { get; set; }
            public string RapidValues { get; set; }

            public int Taxis { get; set; }

            public string DisplayName { get; set; }

            public string HelpText { get; set; }

            public InputType InputType { get; set; }

            public string DefaultValue { get; set; }

            public bool Horizontal { get; set; }

            public List<TableStyleItem> Items { get; set; }

            public int Height { get; set; }

            public string CustomizeLeft { get; set; }

            public string CustomizeRight { get; set; }
        }

        private async Task<(bool Success, string ErrorMessage)> InsertTableStyleAsync(SubmitRequest request)
        {
            var relatedIdentity = request.RelatedIdentities[0];

            if (string.IsNullOrEmpty(request.AttributeName))
            {
                return (false, "操作失败，字段名不能为空！");
            }

            if (await _tableStyleRepository.IsExistsAsync(relatedIdentity, request.TableName, request.AttributeName))
            {
                return (false, $@"显示样式添加失败：字段名""{request.AttributeName}""已存在");
            }

            var style = await _databaseManager.IsAttributeNameExistsAsync(request.TableName, request.AttributeName) ? await _tableStyleRepository.GetTableStyleAsync(request.TableName, request.AttributeName, request.RelatedIdentities) : new TableStyle();

            style.RelatedIdentity = relatedIdentity;
            style.TableName = request.TableName;
            style.AttributeName = request.AttributeName;
            style.DisplayName = request.DisplayName;
            style.HelpText = request.HelpText;
            style.Taxis = request.Taxis;
            style.InputType = request.InputType;
            style.DefaultValue = request.DefaultValue;
            style.Horizontal = request.Horizontal;
            style.Items = new List<TableStyleItem>();

            if (request.InputType == InputType.CheckBox || request.InputType == InputType.Radio || request.InputType == InputType.SelectMultiple || request.InputType == InputType.SelectOne)
            {
                if (request.IsRapid)
                {
                    foreach (var rapidValue in Utilities.GetStringList(request.RapidValues))
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Label = rapidValue,
                            Value = rapidValue,
                            Selected = false
                        };
                        style.Items.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in request.Items)
                    {
                        if (request.InputType != InputType.SelectMultiple && request.InputType != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            return (false, "操作失败，只能有一个初始化时选定项！");
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Label = styleItem.Label,
                            Value = styleItem.Value,
                            Selected = styleItem.Selected
                        };
                        style.Items.Add(itemInfo);
                    }
                }
            }
            else if (request.InputType == InputType.TextArea || request.InputType == InputType.TextEditor)
            {
                style.Height = request.Height;
            }
            else if (request.InputType == InputType.Customize)
            {
                style.CustomizeLeft = request.CustomizeLeft;
                style.CustomizeRight = request.CustomizeRight;
            }

            await _tableStyleRepository.InsertAsync(request.RelatedIdentities, style);

            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage)> UpdateTableStyleAsync(TableStyle style, SubmitRequest request)
        {
            style.AttributeName = request.AttributeName;
            style.DisplayName = request.DisplayName;
            style.HelpText = request.HelpText;
            style.Taxis = request.Taxis;
            style.InputType = request.InputType;
            style.DefaultValue = request.DefaultValue;
            style.Horizontal = request.Horizontal;
            style.Items = new List<TableStyleItem>();

            if (request.InputType == InputType.CheckBox || request.InputType == InputType.Radio || request.InputType == InputType.SelectMultiple || request.InputType == InputType.SelectOne)
            {
                if (request.IsRapid)
                {
                    foreach (var rapidValue in Utilities.GetStringList(request.RapidValues))
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Label = rapidValue,
                            Value = rapidValue,
                            Selected = false
                        };
                        style.Items.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in request.Items)
                    {
                        if (request.InputType != InputType.SelectMultiple && request.InputType != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            return (false, "操作失败，只能有一个初始化时选定项！");
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Label = styleItem.Label,
                            Value = styleItem.Value,
                            Selected = styleItem.Selected
                        };
                        style.Items.Add(itemInfo);
                    }
                }
            }
            else if (request.InputType == InputType.TextArea || request.InputType == InputType.TextEditor)
            {
                style.Height = request.Height;
            }
            else if (request.InputType == InputType.Customize)
            {
                style.CustomizeLeft = request.CustomizeLeft;
                style.CustomizeRight = request.CustomizeRight;
            }

            await _tableStyleRepository.UpdateAsync(style);

            return (true, string.Empty);
        }
    }
}
