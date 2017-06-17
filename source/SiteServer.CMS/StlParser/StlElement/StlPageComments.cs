using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "翻页评论列表", Description = "通过 stl:pageComments 标签在模板中显示翻页评论列表")]
    public class StlPageComments : StlComments
    {
        public new const string ElementName = "stl:pageComments";//可翻页评论列表

        private readonly XmlNode _node;
        private readonly PageInfo _pageInfo;
        private readonly ContextInfo _contextInfo;
        private readonly DataSet _dataSet;

        public const string AttributePageNum = "pageNum";					//每页显示的栏目数目

        public new static SortedList<string, string> AttributeList
        {
            get
            {
                var attributes = StlComments.AttributeList;
                attributes.Add(AttributePageNum, "每页显示的栏目数目");
                return attributes;
            }
        }

        public StlPageComments(string stlPageCommentsElement, PageInfo pageInfo, ContextInfo contextInfo, bool isXmlContent)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageCommentsElement, isXmlContent);
            _node = xmlDocument.DocumentElement;
            _node = _node?.FirstChild;

            ListInfo = ListInfo.GetListInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Comment);

            _dataSet = StlDataUtility.GetPageCommentsDataSet(pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId, null, ListInfo.StartNum, ListInfo.TotalNum, ListInfo.IsRecommend, ListInfo.OrderByString, ListInfo.Where);
        }


        public int GetPageCount(out int totalNum)
        {
            var pageCount = 1;
            totalNum = 0;//数据库中实际的内容数目
            if (_dataSet == null) return pageCount;

            totalNum = _dataSet.Tables[0].DefaultView.Count;
            if (ListInfo.PageNum != 0 && ListInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(ListInfo.PageNum)));//需要生成的总页数
            }
            return pageCount;
        }

        public ListInfo ListInfo { get; }

        public string Parse(int currentPageIndex, int pageCount)
        {
            var parsedContent = string.Empty;

            _contextInfo.PageItemIndex = currentPageIndex * ListInfo.PageNum;

            try
            {
                if (_node != null)
                {
                    if (_dataSet != null)
                    {
                        var objPage = new PagedDataSource {DataSource = _dataSet.Tables[0].DefaultView}; //分页类

                        if (pageCount > 1)
                        {
                            objPage.AllowPaging = true;
                            objPage.PageSize = ListInfo.PageNum;//每页显示的项数
                        }
                        else
                        {
                            objPage.AllowPaging = false;
                        }

                        objPage.CurrentPageIndex = currentPageIndex;//当前页的索引


                        if (ListInfo.Layout == ELayout.None)
                        {
                            var rptContents = new Repeater
                            {
                                ItemTemplate =
                                    new RepeaterTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems,
                                        ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate,
                                        ListInfo.SeparatorRepeat, _pageInfo, EContextType.Comment, _contextInfo)
                            };

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
                                rptContents.AlternatingItemTemplate = new RepeaterTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Comment, _contextInfo);
                            }

                            rptContents.DataSource = objPage;
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

                            //设置列表模板
                            pdlContents.ItemTemplate = new DataListTemplate(ListInfo.ItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Comment, _contextInfo);
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
                                pdlContents.AlternatingItemTemplate = new DataListTemplate(ListInfo.AlternatingItemTemplate, ListInfo.SelectedItems, ListInfo.SelectedValues, ListInfo.SeparatorRepeatTemplate, ListInfo.SeparatorRepeat, _pageInfo, EContextType.Comment, _contextInfo);
                            }

                            pdlContents.DataSource = objPage;
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

            //还原翻页为0，使得其他列表能够正确解析ItemIndex
            _contextInfo.PageItemIndex = 0;

            return parsedContent;
        }
    }

}
