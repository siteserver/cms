using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsMoreController
    {
        [HttpPost, Route(Constants.RouteStlActionsMore)]
        public async Task<ActionResult<MoreResult>> Submit([FromBody] StlMoreRequest request)
        {
            var template = string.Empty;
            try
            {
                template = _settingsManager.Decrypt(request.Template);
                var pageIndex = request.Page - 1;
                if (pageIndex < 0) pageIndex = 0;

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = string.Empty,
                    TemplateType = request.TemplateType,
                    RelatedFileName = string.Empty,
                    CreatedFileFullName = string.Empty,
                    CreatedFileExtName = string.Empty,
                    DefaultTemplate = false
                };
                var site = await _siteRepository.GetAsync(request.SiteId);

                await _parseManager.InitAsync(EditMode.Default, site, request.PageChannelId, request.PageContentId, templateInfo);
                _parseManager.PageInfo.User = await _authManager.GetUserAsync();

                var contentBuilder = new StringBuilder(template);

                var stlLabelList = ParseUtils.GetStlLabels(contentBuilder.ToString());

                if (ParseUtils.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = ParseUtils.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    var stlPageContents = await StlPageContents.GetByStlMoreAsync(stlPageContentsElement, _parseManager);
                    var (pageCount, totalNum) = stlPageContents.GetPageCount();
                    if (totalNum == 0)
                    {
                        return new MoreResult
                        {
                            Value = false,
                            Html = string.Empty,
                        };
                    }

                    for (var currentPageIndex = 0; currentPageIndex < pageCount; currentPageIndex++)
                    {
                        if (currentPageIndex != pageIndex) continue;

                        var pageHtml = await stlPageContents.ParseAsync(totalNum, currentPageIndex, pageCount, false);
                        var pagedBuilder = new StringBuilder(contentBuilder.ToString().Replace(stlPageContentsElementReplaceString, pageHtml));

                        await _parseManager.ReplacePageElementsInSearchPageAsync(pagedBuilder, stlLabelList, string.Empty, currentPageIndex, pageCount, totalNum);

                        await _parseManager.ParseAsync(pagedBuilder, string.Empty, false);
                        return new MoreResult
                        {
                            Value = currentPageIndex + 1 != pageCount,
                            Html = pagedBuilder.ToString()
                        };
                    }
                }

                return new MoreResult
                {
                    Value = false,
                    Html = string.Empty,
                };
            }
            catch (Exception ex)
            {
                var message = await _parseManager.AddStlErrorLogAsync(StlMore.ElementName, template, ex);
                return this.Error(message);
            }
        }
    }
}
