using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.Model
{
    public class ListInfo
    {
        private EContextType _contextType = EContextType.Content;

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

        public static ListInfo GetListInfoByXmlNode(PageInfo pageInfo, ContextInfo contextInfo, EContextType contextType)
        {
            var listInfo = new ListInfo
            {
                _contextType = contextType
            };

            var innerXml = contextInfo.InnerXml;
            var itemTemplate = string.Empty;

            if (!string.IsNullOrEmpty(innerXml))
            {
                var stlElementList = StlParserUtility.GetStlElementList(innerXml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlItemTemplate.ElementName))
                        {
                            var attributes = new LowerNameValueCollection();
                            var templateString = StlParserUtility.GetInnerXml(theStlElement, true, attributes);
                            if (!string.IsNullOrEmpty(templateString))
                            {
                                foreach (var key in attributes.Keys)
                                {
                                    if (!StringUtils.EqualsIgnoreCase(key, StlItemTemplate.AttributeType)) continue;

                                    var type = attributes.Get(key);
                                    if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeItem))
                                    {
                                        itemTemplate = templateString;
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeHeader))
                                    {
                                        listInfo.HeaderTemplate = templateString;
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeFooter))
                                    {
                                        listInfo.FooterTemplate = templateString;
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeAlternatingItem))
                                    {
                                        listInfo.AlternatingItemTemplate = templateString;
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeSelectedItem))
                                    {
                                        if (!string.IsNullOrEmpty(attributes.Get(StlItemTemplate.AttributeSelected)))
                                        {
                                            var selected = attributes.Get(StlItemTemplate.AttributeSelected);
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
                                                listInfo.SelectedItems.Set(val, templateString);
                                            }
                                            if (!string.IsNullOrEmpty(attributes.Get(StlItemTemplate.AttributeSelectedValue)))
                                            {
                                                var selectedValue = attributes.Get(StlItemTemplate.AttributeSelectedValue);
                                                listInfo.SelectedValues.Set(selectedValue, templateString);
                                            }
                                        }
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeSeparator))
                                    {
                                        var selectedValue = TranslateUtils.ToInt(attributes.Get(StlItemTemplate.AttributeSelectedValue), 1);
                                        if (selectedValue <= 1)
                                        {
                                            listInfo.SeparatorTemplate = templateString;
                                        }
                                        else
                                        {
                                            listInfo.SeparatorRepeatTemplate = templateString;
                                            listInfo.SeparatorRepeat = selectedValue;
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
                            listInfo.LoadingTemplate = innerBuilder.ToString();
                            innerXml = innerXml.Replace(theStlElement, string.Empty);
                        }
                        else if (contextType == EContextType.SqlContent && StlParserUtility.IsSpecifiedStlElement(theStlElement, StlQueryString.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerXml(theStlElement, true));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            StlParserUtility.XmlToHtml(innerBuilder);
                            listInfo.QueryString = innerBuilder.ToString();
                            innerXml = innerXml.Replace(theStlElement, string.Empty);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(itemTemplate))
            {
                listInfo.ItemTemplate = !string.IsNullOrEmpty(innerXml) ? innerXml : "<stl:a target=\"_blank\"></stl:a>";
            }
            else
            {
                listInfo.ItemTemplate = itemTemplate;
            }

            var isSetDirection = false;//是否设置了direction属性

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeChannelIndex))
                {
                    listInfo.ChannelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeChannelName))
                {
                    listInfo.ChannelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeUpLevel))
                {
                    listInfo.UpLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeTopLevel))
                {
                    listInfo.TopLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeScope))
                {
                    listInfo.Scope = EScopeTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsTop))
                {
                    listInfo.IsTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsRecommend))
                {
                    listInfo.IsRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsHot))
                {
                    listInfo.IsHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsColor))
                {
                    listInfo.IsColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeWhere))
                {
                    listInfo.Where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeTotalNum))
                {
                    listInfo.TotalNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.AttributePageNum))
                {
                    listInfo.PageNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.AttributeMaxPage))
                {
                    listInfo.MaxPage = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeStartNum))
                {
                    listInfo.StartNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeOrder))
                {
                    if (contextType == EContextType.Content)
                    {
                        listInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, value, ETableStyle.BackgroundContent, ETaxisType.OrderByTaxisDesc);
                    }
                    else if (contextType == EContextType.Channel)
                    {
                        listInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, value, ETableStyle.Channel, ETaxisType.OrderByTaxis);
                    }
                    else if (contextType == EContextType.InputContent)
                    {
                        listInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.PublishmentSystemId, value, ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc);
                    }
                    else
                    {
                        listInfo.OrderByString = value;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroupChannel))
                {
                    listInfo.GroupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupChannel))
                    {
                        listInfo.GroupChannel = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroupChannelNot))
                {
                    listInfo.GroupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupChannelNot))
                    {
                        listInfo.GroupChannelNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroupContent) || StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroup))
                {
                    listInfo.GroupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupContent))
                    {
                        listInfo.GroupContent = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroupContentNot) || StringUtils.EqualsIgnoreCase(name, StlContents.AttributeGroupNot))
                {
                    listInfo.GroupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupContentNot))
                    {
                        listInfo.GroupContentNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeTags))
                {
                    listInfo.Tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeColumns))
                {
                    listInfo.Columns = TranslateUtils.ToInt(value);
                    listInfo.Layout = ELayout.Table;
                    if (listInfo.Columns > 1 && isSetDirection == false)
                    {
                        listInfo.Direction = RepeatDirection.Horizontal;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeDirection))
                {
                    listInfo.Layout = ELayout.Table;
                    listInfo.Direction = Converter.ToRepeatDirection(value);
                    isSetDirection = true;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeHeight))
                {
                    try
                    {
                        listInfo.Height = Unit.Parse(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeWidth))
                {
                    try
                    {
                        listInfo.Width = Unit.Parse(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeAlign))
                {
                    listInfo.Align = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeItemHeight))
                {
                    try
                    {
                        listInfo.ItemHeight = Unit.Parse(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeItemWidth))
                {
                    try
                    {
                        listInfo.ItemWidth = Unit.Parse(value);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeItemAlign))
                {
                    listInfo.ItemAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeItemVerticalAlign))
                {
                    listInfo.ItemVerticalAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeItemClass))
                {
                    listInfo.ItemClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsImage))
                {
                    listInfo.IsImage = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsVideo))
                {
                    listInfo.IsVideo = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsFile))
                {
                    listInfo.IsFile = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsNoDup))
                {
                    listInfo.IsNoDup = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeIsRelatedContents))
                {
                    listInfo.IsRelatedContents = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.AttributeLayout))
                {
                    listInfo.Layout = ELayoutUtils.GetEnumType(value);
                }
                else if (contextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.AttributeConnectionString))
                {
                    listInfo.ConnectionString = value;
                }
                else if (contextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.AttributeConnectionStringName))
                {
                    listInfo.ConnectionString = WebConfigUtils.GetConnectionStringByName(value);
                    if (string.IsNullOrEmpty(listInfo.ConnectionString))
                    {
                        listInfo.ConnectionString = WebConfigUtils.ConnectionString;
                    }
                }
                else
                {
                    listInfo.Others.Set(name, value);
                }
            }

            return listInfo;

        }

        public string ItemTemplate { get; set; } = string.Empty;

        public string HeaderTemplate { get; set; } = string.Empty;

        public string FooterTemplate { get; set; } = string.Empty;

        public string LoadingTemplate { get; set; } = string.Empty;

        public string SeparatorTemplate { get; set; } = string.Empty;

        public string AlternatingItemTemplate { get; set; } = string.Empty;

        public LowerNameValueCollection SelectedItems { get; set; } = new LowerNameValueCollection();

        public LowerNameValueCollection SelectedValues { get; set; } = new LowerNameValueCollection();

        public string SeparatorRepeatTemplate { get; set; } = string.Empty;

        public int SeparatorRepeat { get; set; }

        public int TotalNum { get; set; }

        public int PageNum { get; set; }

        public int MaxPage { get; set; }

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
                        return ETaxisTypeUtils.GetOrderByString(ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc);
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

        public LowerNameValueCollection Others { get; set; } = new LowerNameValueCollection();
    }
}
