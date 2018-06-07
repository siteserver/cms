using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Parsers;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;
using SiteServer.Utils.Enumerations;
using System.Collections.Generic;

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

            var innerHtml = contextInfo.InnerHtml;
            var itemTemplate = string.Empty;

            if (!string.IsNullOrEmpty(innerHtml))
            {
                var stlElementList = StlParserUtility.GetStlElementList(innerHtml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlItemTemplate.ElementName))
                        {
                            var attributes = TranslateUtils.NewIgnoreCaseNameValueCollection();
                            var templateString = StlParserUtility.GetInnerHtml(theStlElement, attributes);
                            if (!string.IsNullOrEmpty(templateString))
                            {
                                foreach (var key in attributes.AllKeys)
                                {
                                    if (!StringUtils.EqualsIgnoreCase(key, StlItemTemplate.Type.Name)) continue;

                                    var type = attributes[key];
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
                                        if (!string.IsNullOrEmpty(attributes[StlItemTemplate.Selected.Name]))
                                        {
                                            var selected = attributes[StlItemTemplate.Selected.Name];
                                            var list = new List<string>();
                                            if (selected.IndexOf(',') != -1)
                                            {
                                                list.AddRange(selected.Split(','));
                                            }
                                            else
                                            {
                                                if (selected.IndexOf('-') != -1)
                                                {
                                                    var first = TranslateUtils.ToInt(selected.Split('-')[0]);
                                                    var second = TranslateUtils.ToInt(selected.Split('-')[1]);
                                                    for (var i = first; i <= second; i++)
                                                    {
                                                        list.Add(i.ToString());
                                                    }
                                                }
                                                else
                                                {
                                                    list.Add(selected);
                                                }
                                            }
                                            foreach (string val in list)
                                            {
                                                listInfo.SelectedItems.Set(val, templateString);
                                            }
                                            if (!string.IsNullOrEmpty(attributes[StlItemTemplate.SelectedValue.Name]))
                                            {
                                                var selectedValue = attributes[StlItemTemplate.SelectedValue.Name];
                                                listInfo.SelectedValues.Set(selectedValue, templateString);
                                            }
                                        }
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeSeparator))
                                    {
                                        var selectedValue = TranslateUtils.ToInt(attributes[StlItemTemplate.SelectedValue.Name], 1);
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
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                        else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerHtml(theStlElement));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            listInfo.LoadingTemplate = innerBuilder.ToString();
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                        else if (contextType == EContextType.SqlContent && StlParserUtility.IsSpecifiedStlElement(theStlElement, StlQueryString.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerHtml(theStlElement));
                            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                            listInfo.QueryString = innerBuilder.ToString();
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(itemTemplate))
            {
                listInfo.ItemTemplate = !string.IsNullOrEmpty(innerHtml) ? innerHtml : "<stl:a target=\"_blank\"></stl:a>";
            }
            else
            {
                listInfo.ItemTemplate = itemTemplate;
            }

            var isSetDirection = false;//是否设置了direction属性

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelIndex.Name))
                {
                    listInfo.ChannelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelName.Name))
                {
                    listInfo.ChannelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.UpLevel.Name))
                {
                    listInfo.UpLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.TopLevel.Name))
                {
                    listInfo.TopLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Scope.Name))
                {
                    listInfo.Scope = EScopeTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsTop.Name))
                {
                    listInfo.IsTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsRecommend.Name))
                {
                    listInfo.IsRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsHot.Name))
                {
                    listInfo.IsHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsColor.Name))
                {
                    listInfo.IsColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Where.Name))
                {
                    listInfo.Where = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.TotalNum.Name))
                {
                    listInfo.TotalNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.PageNum.Name))
                {
                    listInfo.PageNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo), StringUtils.Constants.PageSize);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.MaxPage.Name))
                {
                    listInfo.MaxPage = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.StartNum.Name))
                {
                    listInfo.StartNum = TranslateUtils.ToInt(StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Order.Name))
                {
                    if (contextType == EContextType.Content)
                    {
                        listInfo.OrderByString = StlDataUtility.GetContentOrderByString(pageInfo.SiteId, value, ETaxisType.OrderByTaxisDesc);
                    }
                    else if (contextType == EContextType.Channel)
                    {
                        listInfo.OrderByString = StlDataUtility.GetChannelOrderByString(pageInfo.SiteId, value, ETaxisType.OrderByTaxis);
                    }
                    //else if (contextType == EContextType.InputContent)
                    //{
                    //    listInfo.OrderByString = StlDataUtility.GetOrderByString(pageInfo.SiteId, value, ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc);
                    //}
                    else
                    {
                        listInfo.OrderByString = value;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannel.Name))
                {
                    listInfo.GroupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupChannel))
                    {
                        listInfo.GroupChannel = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannelNot.Name))
                {
                    listInfo.GroupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupChannelNot))
                    {
                        listInfo.GroupChannelNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContent.Name) || StringUtils.EqualsIgnoreCase(name, StlListBase.Group.Name))
                {
                    listInfo.GroupContent = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupContent))
                    {
                        listInfo.GroupContent = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContentNot.Name) || StringUtils.EqualsIgnoreCase(name, StlListBase.GroupNot.Name))
                {
                    listInfo.GroupContentNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                    if (string.IsNullOrEmpty(listInfo.GroupContentNot))
                    {
                        listInfo.GroupContentNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Tags.Name))
                {
                    listInfo.Tags = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Columns.Name))
                {
                    listInfo.Columns = TranslateUtils.ToInt(value);
                    listInfo.Layout = ELayout.Table;
                    if (listInfo.Columns > 1 && isSetDirection == false)
                    {
                        listInfo.Direction = RepeatDirection.Horizontal;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Direction.Name))
                {
                    listInfo.Layout = ELayout.Table;
                    listInfo.Direction = TranslateUtils.ToRepeatDirection(value);
                    isSetDirection = true;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Height.Name))
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
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Width.Name))
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
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Align.Name))
                {
                    listInfo.Align = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemHeight.Name))
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
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemWidth.Name))
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
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemAlign.Name))
                {
                    listInfo.ItemAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemVerticalAlign.Name))
                {
                    listInfo.ItemVerticalAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemClass.Name))
                {
                    listInfo.ItemClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsImage.Name))
                {
                    listInfo.IsImage = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsVideo.Name))
                {
                    listInfo.IsVideo = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsFile.Name))
                {
                    listInfo.IsFile = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsRelatedContents.Name))
                {
                    listInfo.IsRelatedContents = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Layout.Name))
                {
                    listInfo.Layout = ELayoutUtils.GetEnumType(value);
                }
                else if (contextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConnectionString.Name))
                {
                    listInfo.ConnectionString = value;
                }
                else if (contextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConnectionStringName.Name))
                {
                    listInfo.ConnectionString = WebConfigUtils.GetConnectionStringByName(value);
                    if (string.IsNullOrEmpty(listInfo.ConnectionString))
                    {
                        listInfo.ConnectionString = WebConfigUtils.ConnectionString;
                    }
                }
                else if (contextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.QueryString.Name))
                {
                    listInfo.QueryString = StlEntityParser.ReplaceStlEntitiesForAttributeValue(value, pageInfo, contextInfo);
                }
                else
                {
                    listInfo.Others.Set(name, value);
                }
            }

            return listInfo;

        }

        public string ItemTemplate { get; private set; } = string.Empty;

        public string HeaderTemplate { get; private set; } = string.Empty;

        public string FooterTemplate { get; private set; } = string.Empty;

        public string LoadingTemplate { get; private set; } = string.Empty;

        public string SeparatorTemplate { get; private set; } = string.Empty;

        public string AlternatingItemTemplate { get; private set; } = string.Empty;

        public LowerNameValueCollection SelectedItems { get; } = new LowerNameValueCollection();

        public LowerNameValueCollection SelectedValues { get; } = new LowerNameValueCollection();

        public string SeparatorRepeatTemplate { get; private set; } = string.Empty;

        public int SeparatorRepeat { get; private set; }

        public int TotalNum { get; private set; }

        public int PageNum { get; set; } = StringUtils.Constants.PageSize;

        public int MaxPage { get; private set; }

        public int StartNum { get; private set; } = 1;

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
                    //if (_contextType == EContextType.InputContent)
                    //{
                    //    return ETaxisTypeUtils.GetOrderByString(ETableStyle.InputContent, ETaxisType.OrderByTaxisDesc);
                    //}
                }
                return _orderByString;
            }
            set { _orderByString = value; }
        }

        public string GroupChannel { get; private set; } = string.Empty;

        public string GroupChannelNot { get; private set; } = string.Empty;

        public string GroupContent { get; private set; } = string.Empty;

        public string GroupContentNot { get; private set; } = string.Empty;

        public string Tags { get; private set; } = string.Empty;

        public int Columns { get; private set; }

        public RepeatDirection Direction { get; private set; } = RepeatDirection.Vertical;

        public Unit Height { get; private set; } = Unit.Empty;

        public Unit Width { get; private set; } = Unit.Percentage(100);

        public string Align { get; private set; } = string.Empty;

        public Unit ItemHeight { get; private set; } = Unit.Empty;

        public Unit ItemWidth { get; private set; } = Unit.Empty;

        public string ItemAlign { get; private set; } = string.Empty;

        public string ItemVerticalAlign { get; private set; } = string.Empty;

        public string ItemClass { get; private set; } = string.Empty;

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
