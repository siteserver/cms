using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Parse;
using SS.CMS.Api.Stl;
using SS.CMS.StlParser.StlElement;
using SS.CMS.StlParser.StlEntity;
using SS.CMS.StlParser.Utility;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Stl
{
    public partial class ActionsSearchController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public ActionsSearchController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IParseManager parseManager, IDatabaseManager databaseManager, IConfigRepository configRepository, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpPost, Route(ApiRouteActionsSearch.Route)]
        public async Task<ActionResult<string>> Submit([FromBody] SubmitRequest request)
        {
            ParsePage pageInfo = null;
            var template = string.Empty;
            try
            {
                var form = GetPostCollection(request);

                template = _settingsManager.Decrypt(request.Template);
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
                var site = await _siteRepository.GetAsync(request.SiteId);
                var config = await _configRepository.GetAsync();
                pageInfo = ParsePage.GetPageInfo(_pathManager, config, request.SiteId, 0, site, templateInfo, new Dictionary<string, object>());
                pageInfo.User = _authManager.User;

                var contextInfo = new ParseContext(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = await _contentRepository.GetWhereStringByStlSearchAsync(_databaseManager, request.IsAllSites, request.SiteName, request.SiteDir, request.SiteIds, request.ChannelIndex, request.ChannelName, request.ChannelIds, request.Type, request.Word, request.DateAttribute, request.DateFrom, request.DateTo, request.Since, request.SiteId, ApiRouteActionsSearch.ExlcudeAttributeNames, form);

                    var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, _parseManager, request.PageNum, site.TableName, whereString);
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

                        await _parseManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, request.AjaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await _parseManager.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return pagedBuilder.ToString();
                    }
                }
                else if (StlParserUtility.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);

                    var stlPageSqlContents = await StlPageSqlContents.GetAsync(stlElement, _parseManager);
                    
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

                        await _parseManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, pageInfo, stlLabelList, request.AjaxDivId, pageInfo.PageChannelId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await _parseManager.ParseAsync(pageInfo, contextInfo, pagedBuilder, string.Empty, false);
                        return pagedBuilder.ToString();
                    }
                }

                await _parseManager.ParseAsync(pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return contentBuilder.ToString();
            }
            catch (Exception ex)
            {
                var message = await _parseManager.AddStlErrorLogAsync(pageInfo, StlSearch.ElementName, template, ex);
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
