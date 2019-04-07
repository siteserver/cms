using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Shared
{
    [RoutePrefix("pages/shared/tableStyle")]
    public class PagesTableStyleController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin) return Unauthorized();

                var tableName = Request.GetQueryString("tableName");
                var attributeName = Request.GetQueryString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(Request.GetQueryString("relatedIdentities"));

                var styleInfo = TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities) ?? new TableStyleInfo
                {
                    Type = InputType.Text
                };
                if (styleInfo.StyleItems == null)
                {
                    styleInfo.StyleItems = new List<TableStyleItemInfo>();
                }

                var isRapid = true;
                var rapidValues = string.Empty;
                if (styleInfo.StyleItems.Count == 0)
                {
                    styleInfo.StyleItems.Add(new TableStyleItemInfo
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
                    foreach (var item in styleInfo.StyleItems)
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
                    rapidValues = string.Join(",", list);
                }

                return Ok(new
                {
                    Value = styleInfo,
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
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();
                if (!rest.IsAdminLoggin) return Unauthorized();

                var tableName = Request.GetPostString("tableName");
                var attributeName = Request.GetPostString("attributeName");
                var relatedIdentities = TranslateUtils.StringCollectionToIntList(Request.GetPostString("relatedIdentities"));
                var isRapid = Request.GetPostBool("isRapid");
                var rapidValues = TranslateUtils.StringCollectionToStringList(Request.GetPostString("rapidValues"));
                var body = Request.GetPostObject<TableStyleInfo>("styleInfo");

                var styleInfoDatabase =
                    TableStyleManager.GetTableStyleInfo(tableName, attributeName, relatedIdentities) ??
                    new TableStyleInfo();

                bool isSuccess;
                string errorMessage;

                //数据库中没有此项及父项的表样式 or 数据库中没有此项的表样式，但是有父项的表样式
                if (styleInfoDatabase.Id == 0 && styleInfoDatabase.RelatedIdentity == 0 || styleInfoDatabase.RelatedIdentity != relatedIdentities[0])
                {
                    isSuccess = InsertTableStyleInfo(tableName, relatedIdentities, body, isRapid, rapidValues, out errorMessage);
                    LogUtils.AddAdminLog(rest.AdminName, "添加表单显示样式", $"字段名:{body.AttributeName}");
                }
                //数据库中有此项的表样式
                else
                {
                    isSuccess = UpdateTableStyleInfo(styleInfoDatabase, body, isRapid, rapidValues, out errorMessage);
                    LogUtils.AddAdminLog(rest.AdminName, "修改表单显示样式", $"字段名:{body.AttributeName}");
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

        private bool InsertTableStyleInfo(string tableName, List<int> relatedIdentities, TableStyleInfo body, bool isRapid, List<string> rapidValues, out string errorMessage)
        {
            errorMessage = string.Empty;

            var relatedIdentity = relatedIdentities[0];

            if (string.IsNullOrEmpty(body.AttributeName))
            {
                errorMessage = "操作失败，字段名不能为空！";
                return false;
            }

            if (TableStyleManager.IsExists(relatedIdentity, tableName, body.AttributeName))
            {
                errorMessage = $@"显示样式添加失败：字段名""{body.AttributeName}""已存在";
                return false;
            }

            var styleInfo = TableColumnManager.IsAttributeNameExists(tableName, body.AttributeName) ? TableStyleManager.GetTableStyleInfo(tableName, body.AttributeName, relatedIdentities) : new TableStyleInfo();

            styleInfo.RelatedIdentity = relatedIdentity;
            styleInfo.TableName = tableName;
            styleInfo.AttributeName = body.AttributeName;
            styleInfo.DisplayName = AttackUtils.FilterXss(body.DisplayName);
            styleInfo.HelpText = body.HelpText;
            styleInfo.Taxis = body.Taxis;
            styleInfo.Type = body.Type;
            styleInfo.DefaultValue = body.DefaultValue;
            styleInfo.Horizontal = body.Horizontal;
            styleInfo.ExtendValues = body.ExtendValues;
            styleInfo.StyleItems = new List<TableStyleItemInfo>();

            if (body.Type == InputType.CheckBox || body.Type == InputType.Radio || body.Type == InputType.SelectMultiple || body.Type == InputType.SelectOne)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItemInfo
                        {
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.StyleItems)
                    {
                        if (body.Type != InputType.SelectMultiple && body.Type != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            errorMessage = "操作失败，只能有一个初始化时选定项！";
                            return false;
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItemInfo
                        {
                            ItemTitle = styleItem.ItemTitle,
                            ItemValue = styleItem.ItemValue,
                            Selected = styleItem.Selected
                        };
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                }
            }

            DataProvider.TableStyle.Insert(styleInfo);

            return true;
        }

        private bool UpdateTableStyleInfo(TableStyleInfo styleInfo, TableStyleInfo body, bool isRapid, List<string> rapidValues, out string errorMessage)
        {
            errorMessage = string.Empty;

            styleInfo.AttributeName = body.AttributeName;
            styleInfo.DisplayName = AttackUtils.FilterXss(body.DisplayName);
            styleInfo.HelpText = body.HelpText;
            styleInfo.Taxis = body.Taxis;
            styleInfo.Type = body.Type;
            styleInfo.DefaultValue = body.DefaultValue;
            styleInfo.Horizontal = body.Horizontal;
            styleInfo.ExtendValues = body.ExtendValues;
            styleInfo.StyleItems = new List<TableStyleItemInfo>();

            if (body.Type == InputType.CheckBox || body.Type == InputType.Radio || body.Type == InputType.SelectMultiple || body.Type == InputType.SelectOne)
            {
                if (isRapid)
                {
                    foreach (var rapidValue in rapidValues)
                    {
                        var itemInfo = new TableStyleItemInfo
                        {
                            TableStyleId = styleInfo.Id,
                            ItemTitle = rapidValue,
                            ItemValue = rapidValue,
                            Selected = false
                        };
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                }
                else
                {
                    var isHasSelected = false;
                    foreach (var styleItem in body.StyleItems)
                    {
                        if (body.Type != InputType.SelectMultiple && body.Type != InputType.CheckBox && isHasSelected && styleItem.Selected)
                        {
                            errorMessage = "操作失败，只能有一个初始化时选定项！";
                            return false;
                        }
                        if (styleItem.Selected) isHasSelected = true;

                        var itemInfo = new TableStyleItemInfo
                        {
                            TableStyleId = styleInfo.Id,
                            ItemTitle = styleItem.ItemTitle,
                            ItemValue = styleItem.ItemValue,
                            Selected = styleItem.Selected
                        };
                        styleInfo.StyleItems.Add(itemInfo);
                    }
                }
            }

            DataProvider.TableStyle.Update(styleInfo);
            
            return true;
        }
    }
}
