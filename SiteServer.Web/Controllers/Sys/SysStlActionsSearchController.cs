using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsSearchController : ApiController
    {
        public NameValueCollection GetPostCollection(Rest rest)
        {
            var formCollection = new NameValueCollection();
            foreach (var item in rest.PostDict)
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
                var rest = new Rest(Request);
                var form = GetPostCollection(rest);

                var isAllSites = rest.GetPostBool(StlSearch.IsAllSites.ToLower());
                var siteName = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.SiteName.ToLower()));
                var siteDir = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.SiteDir.ToLower()));
                var siteIds = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.SiteIds.ToLower()));
                var channelIndex = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.ChannelIndex.ToLower()));
                var channelName = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.ChannelName.ToLower()));
                var channelIds = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.ChannelIds.ToLower()));
                var type = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.Type.ToLower()));
                var word = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.Word.ToLower()));
                var dateAttribute = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.DateAttribute.ToLower()));
                var dateFrom = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.DateFrom.ToLower()));
                var dateTo = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.DateTo.ToLower()));
                var since = AttackUtils.FilterSqlAndXss(rest.GetPostString(StlSearch.Since.ToLower()));
                var pageNum = rest.GetPostInt(StlSearch.PageNum.ToLower());
                var isHighlight = rest.GetPostBool(StlSearch.IsHighlight.ToLower());
                var siteId = rest.GetPostInt("siteid");
                var ajaxDivId = AttackUtils.FilterSqlAndXss(rest.GetPostString("ajaxdivid"));
                template = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("template"));
                var pageIndex = rest.GetPostInt("page", 1) - 1;

                var templateInfo = new TemplateInfo
                {
                    SiteId = siteId,
                    TemplateName = string.Empty,
                    Type = TemplateType.FileTemplate,
                    RelatedFileName = string.Empty,
                    CreatedFileFullName = string.Empty,
                    CreatedFileExtName = string.Empty,
                    Default = false
                };
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>())
                {
                    UserInfo = rest.UserInfo
                };
                var contextInfo = new ContextInfo(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = DataProvider.ContentRepository.GetWhereStringByStlSearch(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, siteId, ApiRouteActionsSearch.ExlcudeAttributeNames, form);

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
