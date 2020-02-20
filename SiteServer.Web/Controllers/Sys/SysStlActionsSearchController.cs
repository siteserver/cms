using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.API.Context;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlEntity;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysStlActionsSearchController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsSearch.Route)]
        public async Task<IHttpActionResult> Main()
        {
            PageInfo pageInfo = null;
            var template = string.Empty;
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();
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
                template = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("template"), WebConfigUtils.SecretKey);
                var pageIndex = request.GetPostInt("page", 1) - 1;

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = siteId,
                    TemplateName = string.Empty,
                    TemplateType = TemplateType.FileTemplate,
                    RelatedFileName = string.Empty,
                    CreatedFileFullName = string.Empty,
                    CreatedFileExtName = string.Empty,
                    Default = false
                };
                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                pageInfo = await PageInfo.GetPageInfoAsync(siteId, 0, site, templateInfo, new Dictionary<string, object>());
                pageInfo.User = request.User;

                var contextInfo = new ContextInfo(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = await DataProvider.ContentRepository.GetWhereStringByStlSearchAsync(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, siteId, ApiRouteActionsSearch.ExlcudeAttributeNames, form);

                    var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, pageInfo, contextInfo, pageNum, site.TableName, whereString);
                    var (pageCount, totalNum) = stlPageContents.GetPageCount();
                    if (totalNum == 0)
                    {
                        return NotFound();
                    }

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex != pageIndex) continue;

                        var pageHtml = await stlPageContents.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                        await StlParserManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (isHighlight && !string.IsNullOrEmpty(word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{word}</span>"));
                        }

                        await Parser.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return Ok(pagedBuilder.ToString());
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);

                    var stlPageSqlContents = await StlPageSqlContents.GetAsync(stlElement, pageInfo, contextInfo);
                    
                    var pageCount = stlPageSqlContents.GetPageCount(out var totalNum);
                    if (totalNum == 0)
                    {
                        return NotFound();
                    }

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex != pageIndex) continue;

                        var pageHtml = await stlPageSqlContents.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlElement, pageHtml));

                        await StlParserManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, ajaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (isHighlight && !string.IsNullOrEmpty(word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{word}</span>"));
                        }

                        await Parser.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return Ok(pagedBuilder.ToString());
                    }
                }

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return Ok(contentBuilder.ToString());
            }
            catch (Exception ex)
            {
                var message = await LogUtils.AddStlErrorLogAsync(pageInfo, StlSearch.ElementName, template, ex);
                return BadRequest(message);
            }
        }

        private NameValueCollection GetPostCollection(AuthenticatedRequest request)
        {
            var formCollection = new NameValueCollection();
            foreach (var item in request.PostData)
            {
                formCollection[item.Key] = item.Value.ToString();
            }

            return formCollection;
        }
    }
}
