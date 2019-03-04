using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsSearchController : ApiController
    {
        public NameValueCollection GetPostCollection(RequestImpl request)
        {
            var formCollection = new NameValueCollection();
            foreach (var item in request.PostData)
            {
                formCollection[item.Key] = item.Value.ToString();
            }

            return formCollection;
        }

        [HttpPost, Route(ApiRouteActionsSearch.Route)]
        public IHttpActionResult Main()
        {
            PageInfo pageInfo = null;
            var template = string.Empty;
            try
            {
                var request = new RequestImpl();
                var form = GetPostCollection(request);

                var isAllSites = request.GetPostBool(StlSearch.IsAllSites.ToLower());
                var siteName = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteName.ToLower()));
                var siteDir = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteDir.ToLower()));
                var siteIds = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteIds.ToLower()));
                var channelIndex = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelIndex.ToLower()));
                var channelName = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelName.ToLower()));
                var channelIds = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelIds.ToLower()));
                var type = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.Type.ToLower()));
                var word = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.Word.ToLower()));
                var dateAttribute = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateAttribute.ToLower()));
                var dateFrom = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateFrom.ToLower()));
                var dateTo = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateTo.ToLower()));
                var since = AttackUtils.FilterSqlAndXss(request.GetPostString(StlSearch.Since.ToLower()));
                var pageNum = request.GetPostInt(StlSearch.PageNum.ToLower());
                var isHighlight = request.GetPostBool(StlSearch.IsHighlight.ToLower());
                var siteId = request.GetPostInt("siteid");
                var ajaxDivId = AttackUtils.FilterSqlAndXss(request.GetPostString("ajaxdivid"));
                template = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("template"));
                var pageIndex = request.GetPostInt("page", 1) - 1;

                var templateInfo = new TemplateInfo(0, siteId, string.Empty, TemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharset.utf_8, false);
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>())
                {
                    UserInfo = request.UserInfo
                };
                var contextInfo = new ContextInfo(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = DataProvider.ContentDao.GetWhereStringByStlSearch(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, siteId, ApiRouteActionsSearch.ExlcudeAttributeNames, form);

                    var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, pageNum, siteInfo.TableName, whereString);
                    var pageCount = stlPageContents.GetPageCount(out var totalNum);
                    if (totalNum == 0)
                    {
                        return NotFound();
                    }

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex != pageIndex) continue;

                        var pageHtml = stlPageContents.Parse(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                        StlParserManager.ReplacePageElementsInSearchPage(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (isHighlight && !string.IsNullOrEmpty(word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{word}</span>"));
                        }

                        Parser.Parse(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return Ok(pagedBuilder.ToString());
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);

                    var stlPageSqlContents = new StlPageSqlContents(stlElement, pageInfo, contextInfo);
                    
                    var pageCount = stlPageSqlContents.GetPageCount(out var totalNum);
                    if (totalNum == 0)
                    {
                        return NotFound();
                    }

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex != pageIndex) continue;

                        var pageHtml = stlPageSqlContents.Parse(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElement, pageHtml));

                        StlParserManager.ReplacePageElementsInSearchPage(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (isHighlight && !string.IsNullOrEmpty(word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{word}</span>"));
                        }

                        Parser.Parse(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return Ok(pagedBuilder.ToString());
                    }
                }

                Parser.Parse(pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return Ok(contentBuilder.ToString());
            }
            catch (Exception ex)
            {
                var message = LogUtils.AddStlErrorLog(pageInfo, StlSearch.ElementName, template, ex);
                return BadRequest(message);
            }
        }
    }
}
