using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlPageContents : StlContents
    {
        public new const string ElementName = "stl:pagecontents";                       //可翻页内容列表

        public const string AttributePageNum = "pagenum";					            //每页显示的内容数目
        public const string AttributeMaxPage = "maxpage";					            //翻页中生成的静态页面最大数，剩余页面将动态获取

        private readonly string _stlPageContentsElement;
        private readonly XmlNode _node;
        private readonly PageInfo _pageInfo;
        private readonly ContextInfo _contextInfo;

        public new static ListDictionary AttributeList
        {
            get
            {
                var attributes = StlContents.AttributeList;
                attributes.Add(AttributePageNum, "每页显示的内容数目");
                attributes.Add(AttributeMaxPage, "翻页中生成的静态页面最大数，剩余页面将动态获取");
                return attributes;
            }
        }

        public static string Translate(string stlElement)
        {
            return TranslateUtils.EncryptStringBySecretKey(stlElement);
        }

        public StlPageContents(string stlPageContentsElement, PageInfo pageInfo, ContextInfo contextInfo, bool isXmlContent)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageContentsElement, isXmlContent);
            _node = xmlDocument.DocumentElement;
            if (_node != null)
            {
                _stlPageContentsElement = _node.InnerXml;
                _node = _node.FirstChild;

                DisplayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Content);
            }

            _contextInfo.TitleWordNum = DisplayInfo.TitleWordNum;

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, _contextInfo.ChannelID, DisplayInfo.UpLevel, DisplayInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, DisplayInfo.ChannelIndex, DisplayInfo.ChannelName);

            SqlString = StlDataUtility.GetPageContentsSqlString(_pageInfo.PublishmentSystemInfo, channelId, DisplayInfo.GroupContent, DisplayInfo.GroupContentNot, DisplayInfo.Tags, DisplayInfo.IsImageExists, DisplayInfo.IsImage, DisplayInfo.IsVideoExists, DisplayInfo.IsVideo, DisplayInfo.IsFileExists, DisplayInfo.IsFile, DisplayInfo.IsNoDup, DisplayInfo.StartNum, DisplayInfo.TotalNum, DisplayInfo.OrderByString, DisplayInfo.IsTopExists, DisplayInfo.IsTop, DisplayInfo.IsRecommendExists, DisplayInfo.IsRecommend, DisplayInfo.IsHotExists, DisplayInfo.IsHot, DisplayInfo.IsColorExists, DisplayInfo.IsColor, DisplayInfo.Where, DisplayInfo.Scope, DisplayInfo.GroupChannel, DisplayInfo.GroupChannelNot);
        }

        //API SearchOutput调用
        public StlPageContents(string stlPageContentsElement, PageInfo pageInfo, ContextInfo contextInfo, int pageNum, string whereString)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageContentsElement, false);
            _node = xmlDocument.DocumentElement;
            if (_node != null)
            {
                _node = _node.FirstChild;

                DisplayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Content);
            }
            DisplayInfo.Scope = EScopeType.All;

            _contextInfo.TitleWordNum = DisplayInfo.TitleWordNum;

            DisplayInfo.Where += whereString;
            if (pageNum > 0)
            {
                DisplayInfo.PageNum = pageNum;
            }

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, _contextInfo.ChannelID, DisplayInfo.UpLevel, DisplayInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, DisplayInfo.ChannelIndex, DisplayInfo.ChannelName);

            SqlString = StlDataUtility.GetPageContentsSqlString(_pageInfo.PublishmentSystemInfo, channelId, DisplayInfo.GroupContent, DisplayInfo.GroupContentNot, DisplayInfo.Tags, DisplayInfo.IsImageExists, DisplayInfo.IsImage, DisplayInfo.IsVideoExists, DisplayInfo.IsVideo, DisplayInfo.IsFileExists, DisplayInfo.IsFile, DisplayInfo.IsNoDup, DisplayInfo.StartNum, DisplayInfo.TotalNum, DisplayInfo.OrderByString, DisplayInfo.IsTopExists, DisplayInfo.IsTop, DisplayInfo.IsRecommendExists, DisplayInfo.IsRecommend, DisplayInfo.IsHotExists, DisplayInfo.IsHot, DisplayInfo.IsColorExists, DisplayInfo.IsColor, DisplayInfo.Where, DisplayInfo.Scope, DisplayInfo.GroupChannel, DisplayInfo.GroupChannelNot);
        }

        public int GetPageCount(out int totalNum)
        {
            totalNum = 0;
            var pageCount = 1;
            try
            {
                totalNum = BaiRongDataProvider.DatabaseDao.GetPageTotalCount(SqlString);
                if (DisplayInfo.PageNum != 0 && DisplayInfo.PageNum < totalNum)//需要翻页
                {
                    pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(DisplayInfo.PageNum)));//需要生成的总页数
                }
            }
            catch
            {
                // ignored
            }
            return pageCount;
        }

        public string SqlString { get; }

        public ContentsDisplayInfo DisplayInfo { get; }

        public string Parse(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            if (isStatic)
            {
                var maxPage = DisplayInfo.MaxPage;
                if (maxPage == 0)
                {
                    maxPage = _pageInfo.PublishmentSystemInfo.Additional.CreateStaticMaxPage;
                }
                if (maxPage > 0 && currentPageIndex + 1 > maxPage)
                {
                    return ParseDynamic(totalNum, currentPageIndex, pageCount);
                }
            }

            var parsedContent = string.Empty;

            _contextInfo.PageItemIndex = currentPageIndex * DisplayInfo.PageNum;

            try
            {
                if (_node != null)
                {
                    if (!string.IsNullOrEmpty(SqlString))
                    {
                        var pageSqlString = BaiRongDataProvider.DatabaseDao.GetPageSqlString(SqlString, DisplayInfo.OrderByString, totalNum, DisplayInfo.PageNum, currentPageIndex);

                        var datasource = BaiRongDataProvider.DatabaseDao.GetDataSource(pageSqlString);

                        if (DisplayInfo.Layout == ELayout.None)
                        {
                            var rptContents = new Repeater();

                            if (!string.IsNullOrEmpty(DisplayInfo.HeaderTemplate))
                            {
                                rptContents.HeaderTemplate = new SeparatorTemplate(DisplayInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.FooterTemplate))
                            {
                                rptContents.FooterTemplate = new SeparatorTemplate(DisplayInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.SeparatorTemplate))
                            {
                                rptContents.SeparatorTemplate = new SeparatorTemplate(DisplayInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.AlternatingItemTemplate))
                            {
                                rptContents.AlternatingItemTemplate = new RepeaterTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
                            }

                            rptContents.ItemTemplate = new RepeaterTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);

                            rptContents.DataSource = datasource;
                            rptContents.DataBind();

                            if (rptContents.Items.Count > 0)
                            {
                                parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                            }
                        }
                        else
                        {
                            var pdlContents = new ParsedDataList();

                            //设置显示属性
                            TemplateUtility.PutContentsDisplayInfoToMyDataList(pdlContents, DisplayInfo);

                            pdlContents.ItemTemplate = new DataListTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
                            if (!string.IsNullOrEmpty(DisplayInfo.HeaderTemplate))
                            {
                                pdlContents.HeaderTemplate = new SeparatorTemplate(DisplayInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.FooterTemplate))
                            {
                                pdlContents.FooterTemplate = new SeparatorTemplate(DisplayInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.SeparatorTemplate))
                            {
                                pdlContents.SeparatorTemplate = new SeparatorTemplate(DisplayInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(DisplayInfo.AlternatingItemTemplate))
                            {
                                pdlContents.AlternatingItemTemplate = new DataListTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
                            }

                            pdlContents.DataSource = datasource;
                            pdlContents.DataKeyField = ContentAttribute.Id;
                            pdlContents.DataBind();

                            if (pdlContents.Items.Count > 0)
                            {
                                parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            parsedContent = StlParserUtility.GetBackHtml(parsedContent, _pageInfo);

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            _contextInfo.PageItemIndex = 0;
            return parsedContent;
        }

        private string ParseDynamic(int totalNum, int currentPageIndex, int pageCount)
        {
            var loading = DisplayInfo.LoadingTemplate;
            if (string.IsNullOrEmpty(loading))
            {
                loading = @"<div style=""margin: 0 auto;
    padding: 40px 0;
    font-size: 14px;
    font-family: 'Microsoft YaHei';
    text-align: center;
    font-weight: 400;"">
        载入中，请稍后...
</div>";
            }

            _pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(_pageInfo.UniqueId);
            var apiUrl = ActionsPageContents.GetUrl(_pageInfo.PublishmentSystemInfo.Additional.ApiUrl);
            var apiParameters = ActionsPageContents.GetParameters(_pageInfo.PublishmentSystemId, _pageInfo.PageNodeId, _pageInfo.TemplateInfo.TemplateId, totalNum, pageCount, currentPageIndex, _stlPageContentsElement);

            var builder = new StringBuilder();
            builder.Append($@"<div id=""{ajaxDivId}"">");
            builder.Append($@"<div class=""loading"">{loading}</div>");
            builder.Append($@"<div class=""yes"">{string.Empty}</div>");
            builder.Append("</div>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
$(document).ready(function(){{
    $(""#{ajaxDivId} .loading"").show();
    $(""#{ajaxDivId} .yes"").hide();

    var url = '{apiUrl}';
    var parameters = {apiParameters};

    $.support.cors = true;
    $.ajax({{
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(parameters),
        dataType: 'json',
        success: function(res) {{
            $(""#{ajaxDivId} .loading"").hide();
            $(""#{ajaxDivId} .yes"").show();
            $(""#{ajaxDivId} .yes"").html(res);
        }},
        error: function(e) {{
            $(""#{ajaxDivId} .loading"").hide();
            $(""#{ajaxDivId} .yes"").hide();
        }}
    }});
}});
</script>
");

            return builder.ToString();
        }
    }
}