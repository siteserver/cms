using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.StlElement.Inner;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Model
{
    public class ContentsDisplayInfo
    {
        private EContextType _contextType = EContextType.Content;
        private string _headerTemplate = string.Empty;
        private string _footerTemplate = string.Empty;
        private string _loadingTemplate = string.Empty;
        private string _itemTemplate = string.Empty;
        private string _separatorTemplate = string.Empty;
        private string _alternatingItemTemplate = string.Empty;
        private LowerNameValueCollection _selectedItems = new LowerNameValueCollection();
        private LowerNameValueCollection _selectedValues = new LowerNameValueCollection();
        private string _separatorRepeatTemplate = string.Empty;
        private int _separatorRepeat;

        //sqlContents only

        private bool _isScopeExists;
        private EScopeType _scope = EScopeType.Self;

        private bool _isTop;
        private bool _isRecommend;
        private bool _isHot;
        private bool _isColor;

        private string _orderByString = string.Empty;
        private bool _isImage;
        private bool _isVideo;
        private bool _isFile;

        public static ContentsDisplayInfo GetContentsDisplayInfoByXmlNode(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, EContextType contextType)
        {
            var displayInfo = new ContentsDisplayInfo
            {
                _contextType = contextType
            };

            var innerXml = node.InnerXml;
            var itemTemplate = string.Empty;

            if (!string.IsNullOrEmpty(innerXml))
            {
                var stlElementList = StlParserUtility.GetStlElementList(innerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlItem.ElementName)
                         || StlParserUtility.IsSpecifiedStlElement(theStlElement, StlItem.ElementName2)
                            )
                        {
                            var attributes = new LowerNameValueCollection();
                            var templateString = StlParserUtility.GetInnerXml(theStlElement, true, attributes);
                            if (!string.IsNullOrEmpty(templateString))
                            {
                                foreach (string key in attributes.Keys)
                                {
                                    if (key == StlItem.Attribute_Type)
                                    {
                                        var type = attributes[key];
                                        if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_Item))
                                        {
                                            itemTemplate = templateString;
                                        }
                                        else if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_Header))
                                        {
                                            displayInfo._headerTemplate = templateString;
                                        }
                                        else if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_Footer))
                                        {
                                            displayInfo._footerTemplate = templateString;
                                        }
                                        else if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_AlternatingItem))
                                        {
                                            displayInfo._alternatingItemTemplate = templateString;
                                        }
                                        else if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_SelectedItem))
                                        {
                                            if (!string.IsNullOrEmpty(attributes[StlItem.Attribute_Selected]))
                                            {
                                                var selected = attributes[StlItem.Attribute_Selected];
                                                var arraylist = new ArrayList();
                                                if (selected.IndexOf(',') != -1)
                                                {
                                                    arraylist.AddRange(selected.Split(','));
                                                }
                                                else
                                                {
                                                    if (selected.IndexOf('-') != -1)
                                                    {
                                                        var first = TranslateUtils.ToInt(selected.Split('-')[0]);
                                                        var second = TranslateUtils.ToInt(selected.Split('-')[1]);
                                                        for (var i = first; i <= second; i++)
                                                        {
                                                            arraylist.Add(i.ToString());
                                                        }
                                                    }
                                                    else
                                                    {
                                                        arraylist.Add(selected);
                                                    }
                                                }
                                                foreach (string val in arraylist)
                                                {
                                                    displayInfo._selectedItems[val] = templateString;
                                                }
                                                if (!string.IsNullOrEmpty(attributes[StlItem.Attribute_SelectedValue]))
                                                {
                                                    var selectedValue = attributes[StlItem.Attribute_SelectedValue];
                                                    displayInfo._selectedValues[selectedValue] = templateString;
                                                }
                                            }
                                        }
                                        else if (StringUtils.EqualsIgnoreCase(type, StlItem.Type_Separator))
                                        {
                                            var selectedValue = TranslateUtils.ToInt(attributes[StlItem.Attribute_SelectedValue], 1);
                                            if (selectedValue <= 1)
                                            {
                                                displayInfo._separatorTemplate = templateString;
                                            }
                                            else
                                            {
                                                displayInfo._separatorRepeatTemplate = templateString;
                                                displayInfo._separatorRepeat = selectedValue;
                                            }
                                        }
                                    }
                                }
                            }
                            innerXml = innerXml.Replace(theStlElement, string.Empty);
                        }
                        else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            StlParserUtility.XmlToHtml(innerBuilder);
                            displayInfo._loadingTemplate = innerBuilder.ToString();
                            innerXml = innerXml.Replace(theStlElement, string.Empty);
                        }
                        else if (contextType == EContextType.SqlContent && StlParserUtility.IsSpecifiedStlElement(theStlElement, StlQueryString.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            StlParserUtility.XmlToHtml(innerBuilder);
                            displayInfo.QueryString = innerBuilder.ToString();
                            innerXml = innerXml.Replace(theStlElement, string.Empty);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(itemTemplate))
            {
                displayInfo.ItemTemplate = !string.IsNullOrEmpty(innerXml) ? innerXml : "<stl:a target=\"_blank\"></stl:a>";
            }
            else
            {
                displayInfo._itemTemplate = itemTemplate;
            }

            var ie = node.Attributes.GetEnumerator();
            var isSetDirection = false;//是否设置了direction属性

            while (ie.MoveNext())
            {
                var attr = (XmlAttribute)ie.Current;
                var attributeName = attr.Name.ToLower();

                if (attributeName.Equals(StlContents.Attribute_ChannelIndex))
                {
                    displayInfo.ChannelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                }
                else if (attributeName.Equals(StlContents.Attribute_ChannelName))
                {
                    displayInfo.ChannelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                }
                else if (attributeName.Equals(StlContents.Attribute_UpLevel))
                {
                    displayInfo.UpLevel = TranslateUtils.ToInt(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_TopLevel))
                {
                    displayInfo.TopLevel = TranslateUtils.ToInt(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_Scope))
                {
                    displayInfo.Scope = EScopeTypeUtils.GetEnumType(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsTop))
                {
                    displayInfo.IsTop = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsRecommend))
                {
                    displayInfo.IsRecommend = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsHot))
                {
                    displayInfo.IsHot = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsColor))
                {
                    displayInfo.IsColor = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_Where))
                {
                    displayInfo.Where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsDynamic))
                {
                    displayInfo.IsDynamic = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_TotalNum))
                {
                    displayInfo.TotalNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo));
                }
                else if (attributeName.Equals(StlPageContents.AttributePageNum))
                {
                    displayInfo.PageNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo));
                }
                else if (attributeName.Equals(StlPageContents.AttributeMaxPage))
                {
                    displayInfo.MaxPage = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo));
                }
                else if (attributeName.Equals(StlContents.Attribute_TitleWordNum))
                {
                    displayInfo.TitleWordNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo));
                }
                else if (attributeName.Equals(StlContents.Attribute_StartNum))
                {
                    displayInfo.StartNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo));
                }
                else if (attributeName.Equals(StlContents.Attribute_Order))
                {
                    if (contextType == EContextType.Content)
                    {
                        displayInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, attr.Value, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);
                    }
                    else if (contextType == EContextType.Channel)
                    {
                        displayInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, attr.Value, ETableStyle.Channel, ETaxisType.OrderByTaxis);
                    }
                    else if (contextType == EContextType.InputContent)
                    {
                        displayInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, attr.Value, ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc);
                    }
                    else
                    {
                        displayInfo.OrderByString = attr.Value;
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_GroupChannel))
                {
                    displayInfo.GroupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(displayInfo.GroupChannel))
                    {
                        displayInfo.GroupChannel = "__Empty__";
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_GroupChannelNot))
                {
                    displayInfo.GroupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(displayInfo.GroupChannelNot))
                    {
                        displayInfo.GroupChannelNot = "__Empty__";
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_GroupContent) || attributeName.Equals(StlContents.Attribute_Group))
                {
                    displayInfo.GroupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(displayInfo.GroupContent))
                    {
                        displayInfo.GroupContent = "__Empty__";
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_GroupContentNot) || attributeName.Equals(StlContents.Attribute_GroupNot))
                {
                    displayInfo.GroupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(displayInfo.GroupContentNot))
                    {
                        displayInfo.GroupContentNot = "__Empty__";
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_Tags))
                {
                    displayInfo.Tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                }
                else if (attributeName.Equals(StlContents.Attribute_Columns))
                {
                    displayInfo.Columns = TranslateUtils.ToInt(attr.Value);
                    displayInfo.Layout = ELayout.Table;
                    if (displayInfo.Columns > 1 && isSetDirection == false)
                    {
                        displayInfo.Direction = RepeatDirection.Horizontal;
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_Direction))
                {
                    displayInfo.Layout = ELayout.Table;
                    displayInfo.Direction = Converter.ToRepeatDirection(attr.Value);
                    isSetDirection = true;
                }
                else if (attributeName.Equals(StlContents.Attribute_Height))
                {
                    try
                    {
                        displayInfo.Height = Unit.Parse(attr.Value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_Width))
                {
                    try
                    {
                        displayInfo.Width = Unit.Parse(attr.Value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_Align))
                {
                    displayInfo.Align = attr.Value;
                }
                else if (attributeName.Equals(StlContents.Attribute_ItemHeight))
                {
                    try
                    {
                        displayInfo.ItemHeight = Unit.Parse(attr.Value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_ItemWidth))
                {
                    try
                    {
                        displayInfo.ItemWidth = Unit.Parse(attr.Value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (attributeName.Equals(StlContents.Attribute_ItemAlign))
                {
                    displayInfo.ItemAlign = attr.Value;
                }
                else if (attributeName.Equals(StlContents.Attribute_ItemVerticalAlign))
                {
                    displayInfo.ItemVerticalAlign = attr.Value;
                }
                else if (attributeName.Equals(StlContents.Attribute_ItemClass))
                {
                    displayInfo.ItemClass = attr.Value;
                }
                else if (attributeName.Equals(StlContents.Attribute_IsImage))
                {
                    displayInfo.IsImage = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsVideo))
                {
                    displayInfo.IsVideo = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsFile))
                {
                    displayInfo.IsFile = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsNoDup))
                {
                    displayInfo.IsNoDup = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_IsRelatedContents))
                {
                    displayInfo.IsRelatedContents = TranslateUtils.ToBool(attr.Value);
                }
                else if (attributeName.Equals(StlContents.Attribute_Layout))
                {
                    displayInfo.Layout = ELayoutUtils.GetEnumType(attr.Value);
                }
                else if (contextType == EContextType.SqlContent && attributeName.Equals(StlSqlContents.Attribute_ConnectionString))
                {
                    displayInfo.ConnectionString = attr.Value;
                }
                else if (contextType == EContextType.SqlContent && attributeName.Equals(StlSqlContents.Attribute_ConnectionStringName))
                {
                    if (string.IsNullOrEmpty(displayInfo.ConnectionString))
                    {
                        displayInfo.ConnectionString = WebConfigUtils.ConnectionString;
                    }
                }
                else
                {
                    displayInfo.OtherAttributes.Add(attributeName, attr.Value);
                }
            }

            return displayInfo;

        }

        public string ItemTemplate
        {
            get { return _itemTemplate; }
            set { _itemTemplate = value; }
        }

        public string HeaderTemplate
        {
            get { return _headerTemplate; }
            set { _headerTemplate = value; }
        }

        public string FooterTemplate
        {
            get { return _footerTemplate; }
            set { _footerTemplate = value; }
        }

        public string LoadingTemplate
        {
            get { return _loadingTemplate; }
            set { _loadingTemplate = value; }
        }

        public string SeparatorTemplate
        {
            get { return _separatorTemplate; }
            set { _separatorTemplate = value; }
        }

        public string AlternatingItemTemplate
        {
            get { return _alternatingItemTemplate; }
            set { _alternatingItemTemplate = value; }
        }

        public LowerNameValueCollection SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        public LowerNameValueCollection SelectedValues
        {
            get { return _selectedValues; }
            set { _selectedValues = value; }
        }

        public string SeparatorRepeatTemplate
        {
            get { return _separatorRepeatTemplate; }
            set { _separatorRepeatTemplate = value; }
        }

        public int SeparatorRepeat
        {
            get { return _separatorRepeat; }
            set { _separatorRepeat = value; }
        }

        public int TotalNum { get; set; }

        public int PageNum { get; set; }

        public int MaxPage { get; set; }

        public int TitleWordNum { get; set; }

        public int StartNum { get; set; } = 1;

        public string OrderByString
        {
            get
            {
                if (string.IsNullOrEmpty(_orderByString))
                {
                    if (_contextType == EContextType.Content)
                    {
                        return ETaxisTypeUtils.GetContentOrderByString(ETaxisType.OrderByTaxisDesc);
                    }
                    if (_contextType == EContextType.Channel)
                    {
                        return ETaxisTypeUtils.GetChannelOrderByString(ETaxisType.OrderByTaxis);
                    }
                    if (_contextType == EContextType.InputContent)
                    {
                        return ETaxisTypeUtils.GetOrderByString(ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc, string.Empty, null);
                    }
                }
                return _orderByString;
            }
            set { _orderByString = value; }
        }

        public string GroupChannel { get; set; } = string.Empty;

        public string GroupChannelNot { get; set; } = string.Empty;

        public string GroupContent { get; set; } = string.Empty;

        public string GroupContentNot { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        public int Columns { get; set; }

        public RepeatDirection Direction { get; set; } = RepeatDirection.Vertical;

        public Unit Height { get; set; } = Unit.Empty;

        public Unit Width { get; set; } = Unit.Percentage(100);

        public string Align { get; set; } = string.Empty;

        public Unit ItemHeight { get; set; } = Unit.Empty;

        public Unit ItemWidth { get; set; } = Unit.Empty;

        public string ItemAlign { get; set; } = string.Empty;

        public string ItemVerticalAlign { get; set; } = string.Empty;

        public string ItemClass { get; set; } = string.Empty;

        public bool IsImage
        {
            get
            {
                return _isImage;
            }
            set
            {
                IsImageExists = true;
                _isImage = value;
            }
        }

        public bool IsImageExists { get; private set; }

        public bool IsVideo
        {
            get
            {
                return _isVideo;
            }
            set
            {
                IsVideoExists = true;
                _isVideo = value;
            }
        }

        public bool IsVideoExists { get; private set; }

        public bool IsFile
        {
            get
            {
                return _isFile;
            }
            set
            {
                IsFileExists = true;
                _isFile = value;
            }
        }

        public bool IsFileExists { get; private set; }

        public bool IsNoDup { get; set; }

        public bool IsRelatedContents { get; set; }

        public ELayout Layout { get; set; } = ELayout.None;

        public string ConnectionString { get; set; } = string.Empty;

        public string QueryString { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string ChannelIndex { get; set; } = string.Empty;

        public int UpLevel { get; set; }

        public int TopLevel { get; set; } = -1;

        public EScopeType Scope
        {
            get
            {
                if (_isScopeExists)
                {
                    return _scope;
                }
                if (_contextType == EContextType.Channel || _contextType == EContextType.Site)
                {
                    return EScopeType.Children;
                }
                return EScopeType.Self;
            }
            set
            {
                _isScopeExists = true;
                _scope = value;
            }
        }

        public bool IsTop
        {
            get { return _isTop; }
            set
            {
                IsTopExists = true;
                _isTop = value;
            }
        }

        public bool IsTopExists { get; private set; }

        public bool IsRecommend
        {
            get { return _isRecommend; }
            set
            {
                IsRecommendExists = true;
                _isRecommend = value;
            }
        }

        public bool IsRecommendExists { get; private set; }

        public bool IsHot
        {
            get { return _isHot; }
            set
            {
                IsHotExists = true;
                _isHot = value;
            }
        }

        public bool IsHotExists { get; private set; }

        public bool IsColor
        {
            get { return _isColor; }
            set
            {
                IsColorExists = true;
                _isColor = value;
            }
        }

        public bool IsColorExists { get; private set; }

        public string Where { get; set; } = string.Empty;

        public bool IsDynamic { get; set; }

        public NameValueCollection OtherAttributes { get; set; } = new NameValueCollection();
    }
}
