using System;
using System.Collections.Generic;
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
    [Stl(Usage = "翻页内容列表", Description = "通过 stl:pageContents 标签在模板中显示翻页内容列表")]
    public class StlPageContents : StlContents
    {
        public new const string ElementName = "stl:pageContents";

        public const string AttributePageNum = "pageNum";
        public const string AttributeMaxPage = "maxPage";

        private readonly string _stlPageContentsElement;
        private readonly XmlNode _node;
        private readonly PageInfo _pageInfo;
        private readonly ContextInfo _contextInfo;

        public new static SortedList<string, string> AttributeList
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

                ListInfo = ListInfo.GetListInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Content);
            }

            _contextInfo.TitleWordNum = ListInfo.TitleWordNum;

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, _contextInfo.ChannelId, ListInfo.UpLevel, ListInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, ListInfo.ChannelIndex, ListInfo.ChannelName);

            SqlString = StlDataUtility.GetPageContentsSqlString(_pageInfo.PublishmentSystemInfo, channelId, ListInfo.GroupContent, ListInfo.GroupContentNot, ListInfo.Tags, ListInfo.IsImageExists, ListInfo.IsImage, ListInfo.IsVideoExists, ListInfo.IsVideo, ListInfo.IsFileExists, ListInfo.IsFile, ListInfo.IsNoDup, ListInfo.StartNum, ListInfo.TotalNum, ListInfo.OrderByString, ListInfo.IsTopExists, ListInfo.IsTop, ListInfo.IsRecommendExists, ListInfo.IsRecommend, ListInfo.IsHotExists, ListInfo.IsHot, ListInfo.IsColorExists, ListInfo.IsColor, ListInfo.Where, ListInfo.Scope, ListInfo.GroupChannel, ListInfo.GroupChannelNot);
        }

        //API StlActionsSearchController调用
        public StlPageContents(string stlPageContentsElement, PageInfo pageInfo, ContextInfo contextInfo, int pageNum, string tableName, string whereString)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageContentsElement, false);
            _node = xmlDocument.DocumentElement;
            if (_node != null)
            {
                _node = _node.FirstChild;

                ListInfo = ListInfo.GetListInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Content);
            }
            ListInfo.Scope = EScopeType.All;

            _contextInfo.TitleWordNum = ListInfo.TitleWordNum;

            ListInfo.Where += whereString;
            if (pageNum > 0)
            {
                ListInfo.PageNum = pageNum;
            }

            SqlString = StlDataUtility.GetPageContentsSqlStringBySearch(tableName, ListInfo.GroupContent, ListInfo.GroupContentNot, ListInfo.Tags, ListInfo.IsImageExists, ListInfo.IsImage, ListInfo.IsVideoExists, ListInfo.IsVideo, ListInfo.IsFileExists, ListInfo.IsFile, ListInfo.IsNoDup, ListInfo.StartNum, ListInfo.TotalNum, ListInfo.OrderByString, ListInfo.IsTopExists, ListInfo.IsTop, ListInfo.IsRecommendExists, ListInfo.IsRecommend, ListInfo.IsHotExists, ListInfo.IsHot, ListInfo.IsColorExists, ListInfo.IsColor, ListInfo.Where, ListInfo.Scope, ListInfo.GroupChannel, ListInfo.GroupChannelNot);
        }

        public int GetPageCount(out int totalNum)
        {
            totalNum = 0;
            var pageCount = 1;
            try
            {
                totalNum = BaiRongDataProvider.DatabaseDao.GetPageTotalCount(SqlString);
                if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
                {
                    pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
                }
            }
            catch
            {
                // ignored
            }
            return pageCount;
        }

        public string SqlString { get; }

        public ListInfo ListInfo { get; }

        public string Parse(int totalNum, int currentPageIndex, int pageCount, bool isStatic)
        {
            if (isStatic)
            {
                var maxPage = ListInfo.MaxPage;
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

            _contextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

            try
            {
                if (_node != null)
                {
                    if (!string.IsNullOrEmpty(SqlString))
                    {
                        var pageSqlString = BaiRongDataProvider.DatabaseDao.GetPageSqlString(SqlString, ListInfo.OrderByString, totalNum, ListInfo.PageNum, currentPageIndex);

                        var datasource = BaiRongDataProvider.DatabaseDao.GetDataSource(pageSqlString);

                        if (ListInfo.Layout == ELayout.None)
                        {
                            var rptContents = new Repeater();

                            if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
                            {
                                rptContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
                            {
                                rptContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
                            {
                                rptContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
                            {
                                rptContents.AlternatingItemTemplate = new RepeaterTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
                            }

                            rptContents.ItemTemplate = new RepeaterTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);

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
                            TemplateUtility.PutListInfoToMyDataList(pdlContents, ListInfo);

                            pdlContents.ItemTemplate = new DataListTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
                            if (!string.IsNullOrEmpty(ListInfo.HeaderTemplate))
                            {
                                pdlContents.HeaderTemplate = new SeparatorTemplate(ListInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.FooterTemplate))
                            {
                                pdlContents.FooterTemplate = new SeparatorTemplate(ListInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.SeparatorTemplate))
                            {
                                pdlContents.SeparatorTemplate = new SeparatorTemplate(ListInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(ListInfo.AlternatingItemTemplate))
                            {
                                pdlContents.AlternatingItemTemplate = new DataListTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Content, _contextInfo);
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
            var loading = ListInfo.LoadingTemplate;
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