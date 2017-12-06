using System;
using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
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

                var isAllSites = body.GetPostBool(StlSearch.AttributeIsAllSites.ToLower());
                var siteName = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeSiteName.ToLower()));
                var siteDir = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeSiteDir.ToLower()));
                var siteIds = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeSiteIds.ToLower()));
                var channelIndex = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeChannelIndex.ToLower()));
                var channelName = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeChannelName.ToLower()));
                var channelIds = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeChannelIds.ToLower()));
                var type = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeType.ToLower()));
                var word = PageUtils.FilterSql(body.GetPostString(StlSearch.AttributeWord.ToLower()));
                var dateAttribute = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeDateAttribute.ToLower()));
                var dateFrom = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeDateFrom.ToLower()));
                var dateTo = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeDateTo.ToLower()));
                var since = PageUtils.FilterSqlAndXss(body.GetPostString(StlSearch.AttributeSince.ToLower()));
                var pageNum = body.GetPostInt(StlSearch.AttributePageNum.ToLower());
                var isHighlight = body.GetPostBool(StlSearch.AttributeIsHighlight.ToLower());
                var isDefaultDisplay = body.GetPostBool(StlSearch.AttributeIsDefaultDisplay.ToLower());
                var publishmentSystemId = body.GetPostInt("publishmentsystemid");
                var ajaxDivId = PageUtils.FilterSqlAndXss(body.GetPostString("ajaxdivid"));
                var template = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("template"));
                var pageIndex = body.GetPostInt("page", 1) - 1;

                var templateInfo = new TemplateInfo(0, publishmentSystemId, string.Empty, ETemplateType.FileTemplate, string.Empty, string.Empty, string.Empty, ECharset.utf_8, false);
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var pageInfo = new PageInfo(publishmentSystemId, 0, publishmentSystemInfo, templateInfo, body.UserInfo);
                var contextInfo = new ContextInfo(pageInfo);
                var contentBuilder = new StringBuilder(StlRequestEntities.ParseRequestEntities(form, template));

                var stlLabelList = StlParserUtility.GetStlLabelList(contentBuilder.ToString());

                if (StlParserUtility.IsStlElementExists(StlPageContents.ElementName, stlLabelList))
                {
                    var stlElement = StlParserUtility.GetStlElement(StlPageContents.ElementName, stlLabelList);
                    var stlPageContentsElement = stlElement;
                    var stlPageContentsElementReplaceString = stlElement;

                    bool isDefaultCondition;
                    var whereString = DataProvider.ContentDao.GetWhereStringByStlSearch(isAllSites, siteName, siteDir, siteIds, channelIndex, channelName, channelIds, type, word, dateAttribute, dateFrom, dateTo, since, publishmentSystemId, ActionsSearch.ExlcudeAttributeNames, form, out isDefaultCondition);

                    //没搜索条件时不显示搜索结果
                    if (isDefaultCondition && !isDefaultDisplay)
                    {
                        return NotFound();
                    }

                    var stlPageContents = new StlPageContents(stlPageContentsElement, pageInfo, contextInfo, pageNum, publishmentSystemInfo.AuxiliaryTableForContent, whereString);

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

                StlUtility.ParseStl(publishmentSystemInfo, pageInfo, contextInfo, contentBuilder, string.Empty, false);
                return Ok(contentBuilder.ToString());
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
