using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsSearchController
    {
        [HttpPost, Route(Constants.RouteStlActionsSearch)]
        public async Task<ActionResult<StringResult>> Submit([FromBody] StlSearchRequest request)
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

                await _parseManager.InitAsync(EditMode.Default, site, request.SiteId, 0, templateInfo);
                _parseManager.PageInfo.User = await _authManager.GetUserAsync();

                var contentBuilder = new StringBuilder(StlRequest.ParseRequestEntities(form, template));

                var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

                if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var query = await _contentRepository.GetQueryByStlSearchAsync(_databaseManager, request.IsAllSites, request.SiteName, request.SiteDir, request.SiteIds, request.ChannelIndex, request.ChannelName, request.ChannelIds, request.Type, request.Word, request.DateAttribute, request.DateFrom, request.DateTo, request.Since, request.SiteId, StlSearch.GetSearchExcludeAttributeNames, form);

                    var stlPageContents = await StlPageContents.GetByStlSearchAsync(stlPageContentsElement, _parseManager, request.PageNum, query);
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
                else if (ParseUtils.IsStlElementExists(StlPageSqlContents.ElementName, stlLabelList))
                {
                    var stlElement = ParseUtils.GetStlElement(StlPageSqlContents.ElementName, stlLabelList);

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
    }
}
