using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "翻页提交表单列表", Description = "通过 stl:pageInputContents 标签在模板中显示翻页提交表单列表")]
    public class StlPageInputContents : StlInputContents
    {
        public new const string ElementName = "stl:pageInputContents";

        public const string AttributePageNum = "pageNum";

        private readonly XmlNode _node;
        private readonly ListInfo _listInfo;
        private readonly PageInfo _pageInfo;
        private readonly ContextInfo _contextInfo;
        private readonly DataSet _dataSet;

        public new static SortedList<string, string> AttributeList
        {
            get
            {
                var attributes = StlInputContents.AttributeList;
                attributes.Add(AttributePageNum, "每页显示的提交内容数目");
                return attributes;
            }
        }

        public static string Translate(string stlElement)
        {
            return TranslateUtils.EncryptStringBySecretKey(stlElement);
        }

        public StlPageInputContents(string stlPageInputContentsElement, PageInfo pageInfo, ContextInfo contextInfo, bool isXmlContent)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageInputContentsElement, isXmlContent);
            _node = xmlDocument.DocumentElement;
            _node = _node?.FirstChild;

            _listInfo = ListInfo.GetListInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.InputContent);

            var inputId = DataProvider.InputDao.GetInputIdAsPossible(_listInfo.Others.Get(AttributeInputName), pageInfo.PublishmentSystemId);

            _dataSet = StlDataUtility.GetPageInputContentsDataSet(pageInfo.PublishmentSystemId, inputId, _listInfo);
        }


        public int GetPageCount(out int totalNum)
        {
            var pageCount = 1;
            totalNum = 0;//数据库中实际的内容数目
            if (_dataSet == null) return pageCount;

            totalNum = _dataSet.Tables[0].DefaultView.Count;
            if (_listInfo.PageNum != 0 && _listInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(_listInfo.PageNum)));//需要生成的总页数
            }
            return pageCount;
        }

        public ListInfo DisplayInfo => _listInfo;

        public string Parse(int currentPageIndex, int pageCount)
        {
            var parsedContent = string.Empty;

            _contextInfo.PageItemIndex = currentPageIndex * _listInfo.PageNum;

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
                            objPage.PageSize = _listInfo.PageNum;//每页显示的项数
                        }
                        else
                        {
                            objPage.AllowPaging = false;
                        }

                        objPage.CurrentPageIndex = currentPageIndex;//当前页的索引


                        if (_listInfo.Layout == ELayout.None)
                        {
                            var rptContents = new Repeater
                            {
                                ItemTemplate =
                                    new RepeaterTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems,
                                        _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate,
                                        _listInfo.SeparatorRepeat, _pageInfo, EContextType.InputContent, _contextInfo)
                            };

                            if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
                            {
                                rptContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
                            {
                                rptContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
                            {
                                rptContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
                            {
                                rptContents.AlternatingItemTemplate = new RepeaterTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.InputContent, _contextInfo);
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
                            TemplateUtility.PutListInfoToMyDataList(pdlContents, _listInfo);

                            //设置列表模板
                            pdlContents.ItemTemplate = new DataListTemplate(_listInfo.ItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.InputContent, _contextInfo);
                            if (!string.IsNullOrEmpty(_listInfo.HeaderTemplate))
                            {
                                pdlContents.HeaderTemplate = new SeparatorTemplate(_listInfo.HeaderTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.FooterTemplate))
                            {
                                pdlContents.FooterTemplate = new SeparatorTemplate(_listInfo.FooterTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.SeparatorTemplate))
                            {
                                pdlContents.SeparatorTemplate = new SeparatorTemplate(_listInfo.SeparatorTemplate);
                            }
                            if (!string.IsNullOrEmpty(_listInfo.AlternatingItemTemplate))
                            {
                                pdlContents.AlternatingItemTemplate = new DataListTemplate(_listInfo.AlternatingItemTemplate, _listInfo.SelectedItems, _listInfo.SelectedValues, _listInfo.SeparatorRepeatTemplate, _listInfo.SeparatorRepeat, _pageInfo, EContextType.InputContent, _contextInfo);
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
