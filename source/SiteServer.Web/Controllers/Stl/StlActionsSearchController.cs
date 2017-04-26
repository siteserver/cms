using System;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsSearchController : ApiController
    {
        [HttpPost, Route(ActionsSearch.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                var form = HttpContext.Current.Request.Form;

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var ajaxDivId = PageUtils.FilterSqlAndXss(body.GetPostString("ajaxDivId"));
                var pageNum = body.GetPostInt("pageNum");
                var isHighlight = body.GetPostBool("isHighlight");
                var isRedirectSingle = body.GetPostBool("isRedirectSingle");
                var isDefaultDisplay = body.GetPostBool("isDefaultDisplay");
                var dateAttribute = PageUtils.FilterSqlAndXss(body.GetPostString("dateAttribute"));
                if (string.IsNullOrEmpty(dateAttribute))
                {
                    dateAttribute = ContentAttribute.AddDate;
                }
                var pageIndex = body.GetPostInt("page", 1) - 1;

                var template = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("template"));
                template = StlRequestEntities.ParseRequestEntities(form, template);
                var word = PageUtils.FilterSql(body.GetPostString("word"));
                var channelId = body.GetPostString("channelID");
                var dateFrom = PageUtils.FilterSqlAndXss(body.GetPostString("dateFrom"));
                var dateTo = PageUtils.FilterSqlAndXss(body.GetPostString("dateTo"));
                var date = PageUtils.FilterSqlAndXss(body.GetPostString("date"));
                var typeCollection = TranslateUtils.StringCollectionToStringCollection(PageUtils.UrlDecode(PageUtils.FilterSqlAndXss(body.GetPostString("type"))));

                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, TranslateUtils.ToInt(channelId, publishmentSystemId));
                if (nodeInfo == null)
                {
                    nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, publishmentSystemId);
                }
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);

                var excludeAttributes = "ajaxdivid,pagenum,pageindex,iscrosssite,ishighlight,isredirectsingle,isdefaultdisplay,charset,template,word,click,channelid,datefrom,dateto,date,type,dateattribute";

                var templateInfo = new TemplateInfo(0, publishmentSystemId, string.Empty, ETemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharsetUtils.GetEnumType(publishmentSystemInfo.Additional.Charset), false);

                var pageInfo = new PageInfo(nodeInfo.NodeId, 0, publishmentSystemInfo, templateInfo, body.UserInfo);
                var contextInfo = new ContextInfo(pageInfo);

                var contentBuilder = new StringBuilder(template);

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = DataProvider.ContentDao.GetWhereStringBySearchOutput(publishmentSystemInfo, nodeInfo.NodeId, tableStyle, word, typeCollection, channelId, dateFrom, dateTo, date, dateAttribute, excludeAttributes, form);

                    //没搜索条件时不显示搜索结果
                    if (string.IsNullOrEmpty(whereString) && !isDefaultDisplay)
                    {
                        return Ok(string.Empty);
                    }

                    var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, pageNum, whereString);

                    int totalNum;
                    var pageCount = stlPageContents.GetPageCount(out totalNum);

                    if (totalNum == 0)
                    {
                        return NotFound();
                    }
                    var isRedirect = false;
                    if (isRedirectSingle && totalNum == 1)
                    {
                        var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, stlPageContents.SqlString);
                        if (contentInfo != null)
                        {
                            isRedirect = true;
                            contentBuilder = new StringBuilder($@"
<script>
location.href = '{PageUtility.GetContentUrl(publishmentSystemInfo, contentInfo)}';
</script>
");
                        }
                    }
                    if (!isRedirect)
                    {
                        for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                        {
                            if (currentPageIndex == pageIndex)
                            {
                                var pageHtml = stlPageContents.Parse(totalNum, currentPageIndex, pageCount, false);
                                var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                                StlParserManager.ReplacePageElementsInSearchPage(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                                if (isHighlight && !string.IsNullOrEmpty(word))
                                {
                                    var pagedContents = pagedBuilder.ToString();
                                    pagedBuilder = new StringBuilder();
                                    pagedBuilder.Append(RegexUtils.Replace(
                                        $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                        $"<span style='color:#cc0000'>{word}</span>"));
                                }

                                StlUtility.ParseStl(publishmentSystemInfo, pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                                return Ok(pagedBuilder.ToString());
                            }
                        }
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
                {
                    var siteId = TranslateUtils.ToInt(body.GetPostString("siteID"), 0);
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);
                    var stlPageSqlContentsElement = stlElement;
                    var stlPageSqlContentsElementReplaceString = stlElement;

                    var whereBuilder = new StringBuilder();
                    if (!string.IsNullOrEmpty(word))
                    {
                        whereBuilder.Append("(");
                        foreach (var type in typeCollection)
                        {
                            whereBuilder.Append($"[{type}] like '%{word}%' OR ");
                        }
                        whereBuilder.Length = whereBuilder.Length - 3;
                        whereBuilder.Append(")");
                    }
                    if (!string.IsNullOrEmpty(dateFrom))
                    {
                        if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                        whereBuilder.Append($" AddDate >= '{dateFrom}' ");
                    }
                    if (!string.IsNullOrEmpty(dateTo))
                    {
                        if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                        whereBuilder.Append($" AddDate <= '{dateTo}' ");
                    }
                    if (!string.IsNullOrEmpty(date))
                    {
                        var days = TranslateUtils.ToInt(date);
                        if (days > 0)
                        {
                            if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                            whereBuilder.Append(SqlUtils.GetDateDiffLessThanDays("AddDate", days.ToString()));
                        }
                    }
                    if (siteId > 0)
                    {
                        if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                        whereBuilder.Append($"(PublishmentSystemID = {siteId})");
                    }

                    if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                    whereBuilder.Append("(NodeID > 0) ");

                    var tableName = BaiRongDataProvider.TableCollectionDao.GetFirstTableNameByTableType(EAuxiliaryTableType.BackgroundContent);
                    var arraylist = TranslateUtils.StringCollectionToStringList("ajaxdivid,pagenum,pageindex,iscrosssite,ishighlight,isredirectsingle,isdefaultdisplay,charset,successtemplatestring,failuretemplatestring,word,click,channelid,datefrom,dateto,date,type,siteid");
                    foreach (string key in form.Keys)
                    {
                        if (arraylist.Contains(key.ToLower())) continue;
                        if (!string.IsNullOrEmpty(form[key]))
                        {
                            var value = StringUtils.Trim(form[key]);
                            if (!string.IsNullOrEmpty(value))
                            {
                                if (TableManager.IsAttributeNameExists(tableStyle, tableName, key))
                                {
                                    if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                                    whereBuilder.Append($"([{key}] like '%{value}%')");
                                }
                                else
                                {
                                    if (whereBuilder.Length > 0) { whereBuilder.Append(" AND "); }
                                    whereBuilder.Append($"({ContentAttribute.SettingsXml} like '%{key}={value}%')");
                                }
                            }
                        }
                    }

                    //没搜索条件时不显示搜索结果
                    if (whereBuilder.Length == 0 && isDefaultDisplay == false)
                    {
                        return Ok(string.Empty);
                    }

                    var stlPageSqlContents = new StlPageSqlContents(stlPageSqlContentsElement, pageInfo, contextInfo, false, false);
                    if (string.IsNullOrEmpty(stlPageSqlContents.DisplayInfo.QueryString))
                    {
                        stlPageSqlContents.DisplayInfo.QueryString =
                            $"SELECT * FROM {tableName} WHERE {whereBuilder}";
                    }
                    stlPageSqlContents.LoadData();

                    int totalNum;
                    var pageCount = stlPageSqlContents.GetPageCount(out totalNum);

                    if (totalNum == 0)
                    {
                        return NotFound();
                    }
                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex == pageIndex)
                        {
                            var pageHtml = stlPageSqlContents.Parse(currentPageIndex, pageCount);
                            var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageSqlContentsElementReplaceString, pageHtml));

                            StlParserManager.ReplacePageElementsInSearchPage(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageNodeId, currentPageIndex, pageCount, totalNum);

                            if (isHighlight && !string.IsNullOrEmpty(word))
                            {
                                var pagedContents = pagedBuilder.ToString();
                                pagedBuilder = new StringBuilder();
                                pagedBuilder.Append(RegexUtils.Replace(
                                    $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                    $"<span style='color:#cc0000'>{word}</span>"));
                            }

                            StlUtility.ParseStl(publishmentSystemInfo, pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                            return Ok(pagedBuilder.ToString());
                        }
                    }
                }

                StlUtility.ParseStl(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return Ok(contentBuilder.ToString());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
