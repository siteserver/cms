using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Shared
{
    
    [RoutePrefix("pages/shared/tableStyle")]
    public class PagesTableStyleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetQueryString("tableName");
                var attributeName = request.GetQueryString("attributeName");
                var relatedIdentities = StringUtils.GetIntList(request.GetQueryString("relatedIdentities"));

                var style = await TableStyleManager.GetTableStyleAsync(tableName, attributeName, relatedIdentities) ?? new TableStyle
                {
                    Type = InputType.Text
                };
                if (style.StyleItems == null)
                {
                    style.StyleItems = new List<TableStyleItem>();
                }

                var isRapid = true;
                var rapidValues = string.Empty;
                if (style.StyleItems.Count == 0)
                {
                    style.StyleItems.Add(new TableStyleItem
                    {
                        ItemTitle = string.Empty,
                        ItemValue = string.Empty,
                        Selected = false
                    });
                }
                else
                {
                    var isSelected = false;
                    var isNotEquals = false;
                    var list = new List<string>();
                    foreach (var item in style.StyleItems)
                    {
                        list.Add(item.ItemValue);
                        if (item.Selected)
                        {
                            isSelected = true;
                        }
                        if (item.ItemValue != item.ItemTitle)
                        {
                            isNotEquals = true;
                        }
                    }

                    isRapid = !isSelected && !isNotEquals;
                    rapidValues = StringUtils.Join(list);
                }

                return Ok(new
                {
                    Value = style,
                    InputTypes = InputTypeUtils.GetInputTypes(tableName),
                    IsRapid = isRapid,
                    RapidValues = rapidValues
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
                if (!request.IsAdminLoggin) return Unauthorized();

                var tableName = request.GetPostString("tableName");
                var attributeName = request.GetPostString("attributeName");
                var relatedIdentities = StringUtils.GetIntList(request.GetPostString("relatedIdentities"));
                var isRapid = request.GetPostBool("isRapid");
                var rapidValues = StringUtils.GetStringList(request.GetPostString("rapidValues"));
                var body = request.GetPostObject<TableStyle>("style");

                var styleDatabase =
                    await TableStyleManager.GetTableStyleAsync(tableName, attributeName, relatedIdentities) ??
                    new TableStyle();

                bool isSuccess;
                string errorMessage;

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if (styleDatabase.Id == 0 && styleDatabase.RelatedIdentity == 0 || styleDatabase.RelatedIdentity != relatedIdentities[0])
                {
                    (isSuccess, errorMessage) = await InsertTableStyleAsync(tableName, relatedIdentities, body, isRapid, rapidValues);
                    await request.AddAdminLogAsync("添加表单显示样式", $"字段名:{body.AttributeName}");
                }
                //数据库中有此项的表样式
                else
                {
                    (isSuccess, errorMessage) = await UpdateTableStyleAsync(styleDatabase, body, isRapid, rapidValues);
                    await request.AddAdminLogAsync("修改表单显示样式", $"字段名:{body.AttributeName}");
                }

                if (!isSuccess)
                {
                    return BadRequest(errorMessage);
                }

                return Ok(new{});
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<(bool Success, string ErrorMessage)> InsertTableStyleAsync(string tableName, List<int> relatedIdentities, TableStyle body, bool isRapid, List<string> rapidValues)
        {
            var relatedIdentity = relatedIdentities[0];

            if (string.IsNullOrEmpty(body.AttributeName))
            {
                return (false, "操作失败，字段名不能为空！");
            }

            if (await TableStyleManager.IsExistsAsync(relatedIdentity, tableName, body.AttributeName))
            {
                return (false, $@"显示样式添加失败：字段名""{body.AttributeName}""已存在");
            }

            var style = TableColumnManager.IsAttributeNameExists(tableName, body.AttributeName) ? await TableStyleManager.GetTableStyleAsync(tableName, body.AttributeName, relatedIdentities) : new TableStyle();

            style.RelatedIdentity = relatedIdentity;
            style.TableName = tableName;
            style.AttributeName = body.AttributeName;
            style.DisplayName = AttackUtils.FilterXss(body.DisplayName);
            style.HelpText = body.HelpText;
            style.Taxis = body.Taxis;
            style.Type = body.Type;
            style.DefaultValue = body.DefaultValue;
            style.Horizontal = body.Horizontal;
            style.StyleItems = new List<TableStyleItem>();

            if (body.Type == InputType.CheckBox || body.Type == InputType.Radio || body.Type == InputType.SelectMultiple || body.Type == InputType.SelectOne)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = 0,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        style.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.StyleItems)
                    {
                        if (body.Type != InputType.SelectMultiple && body.Type != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            return (false, "操作失败，只能有一个初始化时选定项！");
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = 0,
                            ItemTitle = styleItem.ItemTitle,
                            ItemValue = styleItem.ItemValue,
                            Selected = styleItem.Selected
                        };
                        style.StyleItems.Add(itemInfo);
                    }
                }
            }

            await DataProvider.TableStyleRepository.InsertAsync(style);

            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage)> UpdateTableStyleAsync(TableStyle style, TableStyle body, bool isRapid, List<string> rapidValues)
        {
            style.AttributeName = body.AttributeName;
            style.DisplayName = AttackUtils.FilterXss(body.DisplayName);
            style.HelpText = body.HelpText;
            style.Taxis = body.Taxis;
            style.Type = body.Type;
            style.DefaultValue = body.DefaultValue;
            style.Horizontal = body.Horizontal;
            style.StyleItems = new List<TableStyleItem>();

            if (body.Type == InputType.CheckBox || body.Type == InputType.Radio || body.Type == InputType.SelectMultiple || body.Type == InputType.SelectOne)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = style.Id,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        style.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.StyleItems)
                    {
                        if (body.Type != InputType.SelectMultiple && body.Type != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            return (false, "操作失败，只能有一个初始化时选定项！");
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItem
                        {
                            Id = 0,
                            TableStyleId = style.Id,
                            ItemTitle = styleItem.ItemTitle,
                            ItemValue = styleItem.ItemValue,
                            Selected = styleItem.Selected
                        };
                        style.StyleItems.Add(itemInfo);
                    }
                }
            }

            await DataProvider.TableStyleRepository.UpdateAsync(style);
            
            return (true, string.Empty);
        }
    }
}
