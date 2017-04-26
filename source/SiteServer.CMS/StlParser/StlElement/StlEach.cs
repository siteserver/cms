using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlEach
    {
        public const string ElementName = "stl:each";                       //循环

        public const string Attribute_Type = "type";
        public const string Attribute_Value = "value";
        public const string Attribute_TotalNum = "totalnum";				//显示内容数目
        public const string Attribute_StartNum = "startnum";				//从第几条信息开始显示
        public const string Attribute_Order = "order";						//排序
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public const string Attribute_OnPreLoad = "onpreload";              //加载数据前，执行函数
        public const string Attribute_OnLoaded = "onloaded";                //加载数据后，执行函数

        public const string Attribute_Columns = "columns";
        public const string Attribute_Direction = "direction";
        public const string Attribute_Height = "height";
        public const string Attribute_Width = "width";
        public const string Attribute_Align = "align";
        public const string Attribute_ItemHeight = "itemheight";
        public const string Attribute_ItemWidth = "itemwidth";
        public const string Attribute_ItemAlign = "itemalign";
        public const string Attribute_ItemVerticalAlign = "itemverticalalign";
        public const string Attribute_ItemClass = "itemclass";
        public const string Attribute_Layout = "layout";

        public const string Type_Photo = "photo";
        public const string Type_Template = "template";

        public const string Type_Property = "property";                     //属性（内容，栏目）

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();

                attributes.Add(Attribute_Type, "循环类型");
                attributes.Add(Attribute_Value, "循环值");
                attributes.Add(Attribute_TotalNum, "显示内容数目");
                attributes.Add(Attribute_StartNum, "从第几条信息开始显示");
                attributes.Add(Attribute_Order, "排序");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                attributes.Add(Attribute_OnPreLoad, "加载数据前，执行函数");
                attributes.Add(Attribute_OnLoaded, "加载数据后，执行函数");

                attributes.Add("cellpadding", "填充");
                attributes.Add("cellspacing", "间距");
                attributes.Add("class", "Css类");
                attributes.Add(Attribute_Columns, "列数");
                attributes.Add(Attribute_Direction, "方向");
                attributes.Add(Attribute_Layout, "指定列表布局方式");

                attributes.Add(Attribute_Height, "整体高度");
                attributes.Add(Attribute_Width, "整体宽度");
                attributes.Add(Attribute_Align, "整体对齐");
                attributes.Add(Attribute_ItemHeight, "项高度");
                attributes.Add(Attribute_ItemWidth, "项宽度");
                attributes.Add(Attribute_ItemAlign, "项水平对齐");
                attributes.Add(Attribute_ItemVerticalAlign, "项垂直对齐");
                attributes.Add(Attribute_ItemClass, "项Css类");

                return attributes;
            }
        }

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var displayInfo = ContentsDisplayInfo.GetContentsDisplayInfoByXmlNode(node, pageInfo, contextInfo, EContextType.Content);

                if (displayInfo.IsDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(pageInfo, node, contextInfo, displayInfo);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, XmlNode node, ContextInfo contextInfo, ContentsDisplayInfo displayInfo)
        {
            var parsedContent = string.Empty;

            var type = displayInfo.OtherAttributes[Attribute_Type];
            if (string.IsNullOrEmpty(type))
            {
                type = BackgroundContentAttribute.ImageUrl;
            }
            var value = displayInfo.OtherAttributes[Attribute_Value];
            var onPreLoad = displayInfo.OtherAttributes[Attribute_OnPreLoad];
            var onLoaded = displayInfo.OtherAttributes[Attribute_OnLoaded];

            var contextType = EContextType.Each;
            IEnumerable dataSource = null;
            if (StringUtils.EqualsIgnoreCase(type, Type_Photo))
            {
                contextType = EContextType.Photo;
                dataSource = StlDataUtility.GetPhotosDataSource(pageInfo.PublishmentSystemInfo, contextInfo.ContentID, displayInfo.StartNum, displayInfo.TotalNum);
            }
            else if (StringUtils.EqualsIgnoreCase(type, Type_Property))
            {
                if (contextInfo.ContextType == EContextType.Undefined)
                    contextInfo.ContextType = EContextType.Content;
                contextType = contextInfo.ContextType;
                dataSource = StlDataUtility.GetPropertysDataSource(pageInfo.PublishmentSystemInfo, contextInfo.ContentInfo, contextType, value, displayInfo.StartNum, displayInfo.TotalNum);
            }
            else if (StringUtils.EqualsIgnoreCase(type, Type_Template))
            {
                var builder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(builder, pageInfo, contextInfo);

                return $@"
<%for(i = 0; i < {value}.length; i ++) {{
    item = {value}[i]; %>
    {builder}
<%}}%>
";
            }
            else
            {
                var contentInfo = contextInfo.ContentInfo;
                if (contentInfo != null)
                {
                    var eachList = new List<string>();

                    if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(type)))
                    {
                        eachList.Add(contentInfo.GetExtendedAttribute(type));
                    }

                    var extendAttributeName = ContentAttribute.GetExtendAttributeName(type);
                    var extendValues = contentInfo.GetExtendedAttribute(extendAttributeName);
                    if (!string.IsNullOrEmpty(extendValues))
                    {
                        foreach (string extendValue in TranslateUtils.StringCollectionToStringList(extendValues))
                        {
                            eachList.Add(extendValue);
                        }
                    }

                    if (displayInfo.StartNum > 1 || displayInfo.TotalNum > 0)
                    {
                        if (displayInfo.StartNum > 1)
                        {
                            var count = displayInfo.StartNum - 1;
                            if (count > eachList.Count)
                            {
                                count = eachList.Count;
                            }
                            eachList.RemoveRange(0, count);
                        }

                        if (displayInfo.TotalNum > 0)
                        {
                            if (displayInfo.TotalNum < eachList.Count)
                            {
                                eachList.RemoveRange(displayInfo.TotalNum, eachList.Count - displayInfo.TotalNum);
                            }
                        }
                    }

                    dataSource = eachList;
                }
            }

            if (displayInfo.Layout == ELayout.None)
            {
                var rptContents = new Repeater();

                rptContents.ItemTemplate = new RepeaterTemplate(displayInfo.ItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                if (!string.IsNullOrEmpty(displayInfo.HeaderTemplate))
                {
                    rptContents.HeaderTemplate = new SeparatorTemplate(displayInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.FooterTemplate))
                {
                    rptContents.FooterTemplate = new SeparatorTemplate(displayInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.SeparatorTemplate))
                {
                    rptContents.SeparatorTemplate = new SeparatorTemplate(displayInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.AlternatingItemTemplate))
                {
                    rptContents.AlternatingItemTemplate = new RepeaterTemplate(displayInfo.AlternatingItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                rptContents.DataSource = dataSource;

                rptContents.DataBind();

                if (rptContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(rptContents);
                }
            }
            else
            {
                var pdlContents = new ParsedDataList();

                TemplateUtility.PutContentsDisplayInfoToMyDataList(pdlContents, displayInfo);

                pdlContents.ItemTemplate = new DataListTemplate(displayInfo.ItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                if (!string.IsNullOrEmpty(displayInfo.HeaderTemplate))
                {
                    pdlContents.HeaderTemplate = new SeparatorTemplate(displayInfo.HeaderTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.FooterTemplate))
                {
                    pdlContents.FooterTemplate = new SeparatorTemplate(displayInfo.FooterTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.SeparatorTemplate))
                {
                    pdlContents.SeparatorTemplate = new SeparatorTemplate(displayInfo.SeparatorTemplate);
                }
                if (!string.IsNullOrEmpty(displayInfo.AlternatingItemTemplate))
                {
                    pdlContents.AlternatingItemTemplate = new DataListTemplate(displayInfo.AlternatingItemTemplate, displayInfo.SelectedItems, displayInfo.SelectedValues, displayInfo.SeparatorRepeatTemplate, displayInfo.SeparatorRepeat, pageInfo, contextType, contextInfo);
                }

                pdlContents.DataSource = dataSource;

                pdlContents.DataBind();

                if (pdlContents.Items.Count > 0)
                {
                    parsedContent = ControlUtils.GetControlRenderHtml(pdlContents);
                }
            }

            return parsedContent;
        }
    }
}
