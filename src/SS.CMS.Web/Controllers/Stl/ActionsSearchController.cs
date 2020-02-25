using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Api.Stl;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.StlParser;
using SS.CMS.StlParser.Model;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.StlEntity;
using SS.CMS.StlParser.Utility;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsSearchController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        public ActionsSearchController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost, Route(ApiRouteActionsSearch.Route)]
        public async Task<ActionResult<string>> Submit([FromBody] SubmitRequest request)
        {
            PageInfo pageInfo = null;
            var template = string.Empty;
            try
            {
                var form = GetPostCollection(request);

                template = TranslateUtils.DecryptStringBySecretKey(request.Template, WebConfigUtils.SecretKey);
                var pageIndex = request.Page - 1;
                if (pageIndex < 0) pageIndex = 0;

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = string.Empty,
                    TemplateType = TemplateType.FileTemplate,
                    RelatedFileName = string.Empty,
                    CreatedFileFullName = string.Empty,
                    CreatedFileExtName = string.Empty,
                    Default = false
                };
                var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
                pageInfo = await PageInfo.GetPageInfoAsync(request.SiteId, 0, site, templateInfo, new Dictionary<string, object>());
                pageInfo.User = _authManager.User;

                var contextInfo = new ContextInfo(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = await DataProvider.ContentRepository.GetWhereStringByStlSearchAsync(request.IsAllSites, request.SiteName, request.SiteDir, request.SiteIds, request.ChannelIndex, request.ChannelName, request.ChannelIds, request.Type, request.Word, request.DateAttribute, request.DateFrom, request.DateTo, request.Since, request.SiteId, ApiRouteActionsSearch.ExlcudeAttributeNames, form);

                    var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, pageInfo, contextInfo, request.PageNum, site.TableName, whereString);
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

                        await StlParserManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, request.AjaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await Parser.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return pagedBuilder.ToString();
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

                        await StlParserManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, request.AjaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await Parser.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return pagedBuilder.ToString();
                    }
                }

                await Parser.ParseAsync(pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return contentBuilder.ToString();
            }
            catch (Exception ex)
            {
                var message = await LogUtils.AddStlErrorLogAsync(pageInfo, StlSearch.ElementName, template, ex);
                return this.Error(message);
            }
        }

        private NameValueCollection GetPostCollection(Dictionary<string, object> request)
        {
            var formCollection = new NameValueCollection();
            foreach (var (key, value) in request)
            {
                formCollection[key] = value.ToString();
            }

            return formCollection;
        }
    }
}
