using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.StlParser.StlEntity;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    [OpenApiIgnore]
    [Route(Constants.ApiPrefix + Constants.ApiStlPrefix)]
    public class ActionsSearchController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IParseManager _parseManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;

        public ActionsSearchController(ISettingsManager settingsManager, IAuthManager authManager, IParseManager parseManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _parseManager = parseManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
        }

        [HttpPost, Route(Constants.RouteStlActionsSearch)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] StlSearch.SearchRequest request)
        {
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
                    DefaultTemplate = false
                };
                var site = await _siteRepository.GetAsync(request.SiteId);

                await _parseManager.InitAsync(site, request.SiteId, 0, templateInfo);
                _parseManager.PageInfo.User = await _authManager.GetUserAsync();

                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var whereString = await _contentRepository.GetWhereStringByStlSearchAsync(_databaseManager, request.IsAllSites, request.SiteName, request.SiteDir, request.SiteIds, request.ChannelIndex, request.ChannelName, request.ChannelIds, request.Type, request.Word, request.DateAttribute, request.DateFrom, request.DateTo, request.Since, request.SiteId, StlSearch.GetSearchExcludeAttributeNames, form);

                    var stlPageContents = await StlPageContents.GetAsync(stlPageContentsElement, _parseManager, request.PageNum, whereString);
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

                        await _parseManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, stlLabelList, request.AjaxDivId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await _parseManager.ParseAsync(pagedBuilder, string.Empty, false);
                        return new StringResult
                        {
                            Value = pagedBuilder.ToString()
                        };
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

                        await _parseManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, stlLabelList, request.AjaxDivId, currentPageIndex, pageCount, totalNum);

                        if (request.IsHighlight && !string.IsNullOrEmpty(request.Word))
                        {
                            var pagedContents = pagedBuilder.ToString();
                            pagedBuilder = new StringBuilder();
                            pagedBuilder.Append(RegexUtils.Replace(
                                $"({request.Word.Replace(" ", "\\s")})(?!</a>)(?![^><]*>)", pagedContents,
                                $"<span style='color:#cc0000'>{request.Word}</span>"));
                        }

                        await _parseManager.ParseAsync(pagedBuilder, string.Empty, false);
                        return new StringResult
                        {
                            Value = pagedBuilder.ToString()
                        };
                    }
                }

                await _parseManager.ParseAsync(contentBuilder, string.Empty, false);
                return new StringResult
                {
                    Value = contentBuilder.ToString()
                };
            }
            catch (Exception ex)
            {
                var message = await _parseManager.AddStlErrorLogAsync(StlSearch.ElementName, template, ex);
                return this.Error(message);
            }
        }

        private NameValueCollection GetPostCollection(StlSearch.SearchRequest request)
        {
            var formCollection = new NameValueCollection();
            foreach (var key in request.GetKeys())
            {
                var value = request.Get(key);
                if (value != null)
                {
                    formCollection[key] = request.Get(key).ToString();
                }
            }

            return formCollection;
        }
    }
}
