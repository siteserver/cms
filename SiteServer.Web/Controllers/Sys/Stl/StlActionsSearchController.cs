using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsSearchController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsSearch.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new AuthRequest();
                var form = request.GetPostCollection();

                var isAllSites = request.GetPostBool(StlSearch.IsAllSites.Name.ToLower());
                var siteName = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteName.Name.ToLower()));
                var siteDir = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteDir.Name.ToLower()));
                var siteIds = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.SiteIds.Name.ToLower()));
                var channelIndex = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelIndex.Name.ToLower()));
                var channelName = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelName.Name.ToLower()));
                var channelIds = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.ChannelIds.Name.ToLower()));
                var type = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.Type.Name.ToLower()));
                var word = PageUtils.FilterSql(request.GetPostString(StlSearch.Word.Name.ToLower()));
                var dateAttribute = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateAttribute.Name.ToLower()));
                var dateFrom = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateFrom.Name.ToLower()));
                var dateTo = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.DateTo.Name.ToLower()));
                var since = PageUtils.FilterSqlAndXss(request.GetPostString(StlSearch.Since.Name.ToLower()));
                var pageNum = request.GetPostInt(StlSearch.PageNum.Name.ToLower());
                var isHighlight = request.GetPostBool(StlSearch.IsHighlight.Name.ToLower());
                var siteId = request.GetPostInt("siteid");
                var ajaxDivId = PageUtils.FilterSqlAndXss(request.GetPostString("ajaxdivid"));
                var template = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("template"));
                var pageIndex = request.GetPostInt("page", 1) - 1;

                var templateInfo = new TemplateInfo(0, siteId, string.Empty, TemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharset.utf_8, false);
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var pageInfo = new PageInfo(siteId, 0, siteInfo, templateInfo, new Dictionary<string, object>())
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

                    int totalNum;
                    var pageCount = stlPageContents.GetPageCount(out totalNum);

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

                Parser.Parse(pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return Ok(contentBuilder.ToString());
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
