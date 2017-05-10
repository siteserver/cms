using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "翻页栏目列表", Description = "通过 stl:pageChannels 标签在模板中显示翻页栏目列表")]
    public class StlPageChannels : StlChannels
    {
        public new const string ElementName = "stl:pageChannels";

        public const string AttributePageNum = "pageNum";

        private readonly XmlNode _node;
        private readonly PageInfo _pageInfo;
        private readonly ContextInfo _contextInfo;
        private readonly DataSet _dataSet;

        public new static SortedList<string, string> AttributeList
        {
            get
            {
                var attributes = StlChannels.AttributeList;
                attributes.Add(AttributePageNum, "每页显示的栏目数目");
                return attributes;
            }
        }

        public static string Translate(string stlElement)
        {
            return TranslateUtils.EncryptStringBySecretKey(stlElement);
        }

        public StlPageChannels(string stlPageChannelsElement, PageInfo pageInfo, ContextInfo contextInfo, bool isXmlContent)
        {
            _pageInfo = pageInfo;
            _contextInfo = contextInfo;
            var xmlDocument = StlParserUtility.GetXmlDocument(stlPageChannelsElement, isXmlContent);
            _node = xmlDocument.DocumentElement;
            _node = _node?.FirstChild;

            DisplayInfo = ListInfo.GetListInfoByXmlNode(_node, pageInfo, _contextInfo, EContextType.Channel);

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, _contextInfo.ChannelId, DisplayInfo.UpLevel, DisplayInfo.TopLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, DisplayInfo.ChannelIndex, DisplayInfo.ChannelName);

            var isTotal = TranslateUtils.ToBool(DisplayInfo.Others.Get(AttributeIsTotal));

            if (TranslateUtils.ToBool(DisplayInfo.Others.Get(AttributeIsAllChildren)))
            {
                DisplayInfo.Scope = EScopeType.Descendant;
            }

            _dataSet = StlDataUtility.GetPageChannelsDataSet(pageInfo.PublishmentSystemId, channelId, DisplayInfo.GroupChannel, DisplayInfo.GroupChannelNot, DisplayInfo.IsImageExists, DisplayInfo.IsImage, DisplayInfo.StartNum, DisplayInfo.TotalNum, DisplayInfo.OrderByString, DisplayInfo.Scope, isTotal, DisplayInfo.Where);
        }


        public int GetPageCount(out int totalNum)
        {
            var pageCount = 1;
            totalNum = 0;//数据库中实际的内容数目
            if (_dataSet == null) return pageCount;

            totalNum = _dataSet.Tables[0].DefaultView.Count;
            if (DisplayInfo.PageNum != 0 && DisplayInfo.PageNum < totalNum)//需要翻页
            {
                pageCount = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(totalNum) / Convert.ToDouble(DisplayInfo.PageNum)));//需要生成的总页数
            }
            return pageCount;
        }

        public ListInfo DisplayInfo { get; }

        public string Parse(int currentPageIndex, int pageCount)
        {
            var parsedContent = string.Empty;

            _contextInfo.PageItemIndex = currentPageIndex * DisplayInfo.PageNum;

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
                            objPage.PageSize = DisplayInfo.PageNum;//每页显示的项数
                        }
                        else
                        {
                            objPage.AllowPaging = false;
                        }

                        objPage.CurrentPageIndex = currentPageIndex;//当前页的索引


                        if (DisplayInfo.Layout == ELayout.None)
                        {
                            var rptContents = new Repeater
                            {
                                ItemTemplate =
                                    new RepeaterTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems,
                                        DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate,
                                        DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Channel, _contextInfo)
                            };

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
                                rptContents.AlternatingItemTemplate = new RepeaterTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Channel, _contextInfo);
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
                            TemplateUtility.PutListInfoToMyDataList(pdlContents, DisplayInfo);

                            //设置列表模板
                            pdlContents.ItemTemplate = new DataListTemplate(DisplayInfo.ItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Channel, _contextInfo);
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
                                pdlContents.AlternatingItemTemplate = new DataListTemplate(DisplayInfo.AlternatingItemTemplate, DisplayInfo.SelectedItems, DisplayInfo.SelectedValues, DisplayInfo.SeparatorRepeatTemplate, DisplayInfo.SeparatorRepeat, _pageInfo, EContextType.Channel, _contextInfo);
                            }

                            pdlContents.DataSource = objPage;
                            pdlContents.DataKeyField = NodeAttribute.NodeId;
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

            return StlParserUtility.GetBackHtml(parsedContent, _pageInfo);
        }
    }

}
