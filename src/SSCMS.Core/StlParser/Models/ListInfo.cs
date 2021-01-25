using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Enums;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Enums;
using SSCMS.Parse;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Models
{
    public class ListInfo
    {
        private ParseType _contextType = ParseType.Content;

        private bool _isScopeExists;
        private ScopeType _scope = ScopeType.Self;

        private bool _isTop;
        private bool _isRecommend;
        private bool _isHot;
        private bool _isColor;

        private bool _isImage;
        private bool _isVideo;
        private bool _isFile;

        public static async Task<ListInfo> GetListInfoAsync(IParseManager parseManager, ParseType contextType, Query query = null)
        {
            var contextInfo = parseManager.ContextInfo;

            var listInfo = new ListInfo
            {
                _contextType = contextType,
                DatabaseType = parseManager.SettingsManager.DatabaseType,
                ConnectionString = parseManager.SettingsManager.DatabaseConnectionString,
                Query = query
            };

            var innerHtml = contextInfo.InnerHtml;
            var itemTemplate = string.Empty;

            if (!string.IsNullOrEmpty(innerHtml))
            {
                var stlElementList = ParseUtils.GetStlElements(innerHtml);
                if (stlElementList.Count > 0)
                {
                    foreach (var theStlElement in stlElementList)
                    {
                        if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlItemTemplate.ElementName))
                        {
                            var (templateString, attributes) = ParseUtils.GetInnerHtmlAndAttributes(theStlElement);
                            if (!string.IsNullOrEmpty(templateString))
                            {
                                foreach (var key in attributes.AllKeys)
                                {
                                    if (!StringUtils.EqualsIgnoreCase(key, StlItemTemplate.Type)) continue;

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
                                        if (!string.IsNullOrEmpty(attributes[StlItemTemplate.Selected]))
                                        {
                                            var selected = attributes[StlItemTemplate.Selected];
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
                                            if (!string.IsNullOrEmpty(attributes[StlItemTemplate.SelectedValue]))
                                            {
                                                var selectedValue = attributes[StlItemTemplate.SelectedValue];
                                                listInfo.SelectedValues.Set(selectedValue, templateString);
                                            }
                                        }
                                    }
                                    else if (StringUtils.EqualsIgnoreCase(type, StlItemTemplate.TypeSeparator))
                                    {
                                        var selectedValue = TranslateUtils.ToInt(attributes[StlItemTemplate.SelectedValue], 1);
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
                        else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                        {
                            var innerBuilder = new StringBuilder(ParseUtils.GetInnerHtml(theStlElement));
                            await parseManager.ParseInnerContentAsync(innerBuilder);
                            listInfo.LoadingTemplate = innerBuilder.ToString();
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                        else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlQuery.ElementName))
                        {
                            if (listInfo.Query == null)
                            {
                                listInfo.Query = Q.NewQuery();
                            }
                            listInfo.Query.AddQuery(theStlElement);
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                        else if (contextType == ParseType.SqlContent && ParseUtils.IsSpecifiedStlElement(theStlElement, StlQueryString.ElementName))
                        {
                            var innerBuilder = new StringBuilder(ParseUtils.GetInnerHtml(theStlElement));
                            await parseManager.ParseInnerContentAsync(innerBuilder);
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

                if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelIndex) || StringUtils.EqualsIgnoreCase(name, StlListBase.Index))
                {
                    listInfo.ChannelIndex = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelName))
                {
                    listInfo.ChannelName = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Parent))
                {
                    listInfo.UpLevel = 1;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.UpLevel))
                {
                    listInfo.UpLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.TopLevel))
                {
                    listInfo.TopLevel = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Scope))
                {
                    listInfo.Scope = TranslateUtils.ToEnum(value, ScopeType.Self);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsTop))
                {
                    listInfo.IsTop = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsRecommend))
                {
                    listInfo.IsRecommend = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsHot))
                {
                    listInfo.IsHot = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsColor))
                {
                    listInfo.IsColor = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.TotalNum))
                {
                    listInfo.TotalNum = TranslateUtils.ToInt(await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.PageNum))
                {
                    listInfo.PageNum = TranslateUtils.ToInt(await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value), Constants.PageSize);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.MaxPage))
                {
                    listInfo.MaxPage = TranslateUtils.ToInt(await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.StartNum))
                {
                    listInfo.StartNum = TranslateUtils.ToInt(await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Order))
                {
                    listInfo.Order = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Where))
                {
                    listInfo.Where = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Group))
                {
                    if (contextType == ParseType.Channel)
                    {
                        listInfo.GroupChannel = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                        if (string.IsNullOrEmpty(listInfo.GroupChannel))
                        {
                            listInfo.GroupChannel = "__Empty__";
                        }
                    }
                    else if (contextType == ParseType.Content)
                    {
                        listInfo.GroupContent = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                        if (string.IsNullOrEmpty(listInfo.GroupContent))
                        {
                            listInfo.GroupContent = "__Empty__";
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupNot))
                {
                    if (contextType == ParseType.Channel)
                    {
                        listInfo.GroupChannelNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                        if (string.IsNullOrEmpty(listInfo.GroupChannelNot))
                        {
                            listInfo.GroupChannelNot = "__Empty__";
                        }
                    }
                    else if (contextType == ParseType.Content)
                    {
                        listInfo.GroupContentNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                        if (string.IsNullOrEmpty(listInfo.GroupContentNot))
                        {
                            listInfo.GroupContentNot = "__Empty__";
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannel))
                {
                    listInfo.GroupChannel = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (string.IsNullOrEmpty(listInfo.GroupChannel))
                    {
                        listInfo.GroupChannel = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannelNot))
                {
                    listInfo.GroupChannelNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (string.IsNullOrEmpty(listInfo.GroupChannelNot))
                    {
                        listInfo.GroupChannelNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContent))
                {
                    listInfo.GroupContent = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (string.IsNullOrEmpty(listInfo.GroupContent))
                    {
                        listInfo.GroupContent = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContentNot))
                {
                    listInfo.GroupContentNot = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (string.IsNullOrEmpty(listInfo.GroupContentNot))
                    {
                        listInfo.GroupContentNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Tags))
                {
                    listInfo.Tags = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Columns))
                {
                    listInfo.Columns = TranslateUtils.ToInt(value);
                    listInfo.Layout = ListLayout.Table;
                    if (listInfo.Columns > 1 && isSetDirection == false)
                    {
                        listInfo.Direction = "horizontal";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Direction))
                {
                    listInfo.Layout = ListLayout.Table;
                    listInfo.Direction = value;
                    isSetDirection = true;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Align))
                {
                    listInfo.Align = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemAlign))
                {
                    listInfo.ItemAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemVerticalAlign))
                {
                    listInfo.ItemVerticalAlign = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ItemClass))
                {
                    listInfo.ItemClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsImage))
                {
                    listInfo.IsImage = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsVideo))
                {
                    listInfo.IsVideo = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.IsFile))
                {
                    listInfo.IsFile = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlContents.IsRelatedContents))
                {
                    listInfo.IsRelatedContents = TranslateUtils.ToBool(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Layout))
                {
                    listInfo.Layout = TranslateUtils.ToEnum(value, ListLayout.None);
                }
                else if (contextType == ParseType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.DatabaseTypeName))
                {
                    var databaseType = parseManager.SettingsManager.Configuration[value];
                    if (!string.IsNullOrEmpty(databaseType))
                    {
                        listInfo.DatabaseType = TranslateUtils.ToEnum(databaseType, DatabaseType.MySql);
                    }
                }
                else if (contextType == ParseType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.DatabaseType))
                {
                    listInfo.DatabaseType = TranslateUtils.ToEnum(value, DatabaseType.MySql);
                }
                else if (contextType == ParseType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConnectionStringName))
                {
                    var connectionString = parseManager.SettingsManager.Configuration[value];
                    if (!string.IsNullOrEmpty(connectionString))
                    {
                        listInfo.ConnectionString = connectionString;
                    }
                }
                else if (contextType == ParseType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConnectionString))
                {
                    listInfo.ConnectionString = value;
                }
                else if (contextType == ParseType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.QueryString))
                {
                    listInfo.QueryString = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
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

        public NameValueCollection SelectedItems { get; } = TranslateUtils.NewIgnoreCaseNameValueCollection();

        public NameValueCollection SelectedValues { get; } = TranslateUtils.NewIgnoreCaseNameValueCollection();

        public string SeparatorRepeatTemplate { get; private set; } = string.Empty;

        public int SeparatorRepeat { get; private set; }

        public int TotalNum { get; private set; }

        public int PageNum { get; set; } = Constants.PageSize;

        public int MaxPage { get; private set; }

        public int StartNum { get; private set; } = 1;

        public string Order { get; set; }

        public string Where { get; set; }

        public Query Query { get; set; }

        public string GroupChannel { get; private set; } = string.Empty;

        public string GroupChannelNot { get; private set; } = string.Empty;

        public string GroupContent { get; private set; } = string.Empty;

        public string GroupContentNot { get; private set; } = string.Empty;

        public string Tags { get; private set; } = string.Empty;

        public int Columns { get; private set; }

        public string Direction { get; set; } = "vertical";

        public string Align { get; private set; } = string.Empty;

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

        public ListLayout Layout { get; set; } = ListLayout.None;

        public DatabaseType DatabaseType { get; set; } = DatabaseType.MySql;

        public string ConnectionString { get; set; } = string.Empty;

        public string QueryString { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string ChannelIndex { get; set; } = string.Empty;

        public int UpLevel { get; set; }

        public int TopLevel { get; set; } = -1;

        public ScopeType Scope
        {
            get
            {
                if (_isScopeExists)
                {
                    return _scope;
                }
                if (_contextType == ParseType.Channel || _contextType == ParseType.Site)
                {
                    return ScopeType.Children;
                }
                return ScopeType.Self;
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

        public NameValueCollection Others { get; } = TranslateUtils.NewIgnoreCaseNameValueCollection();

        public NameValueCollection GetTableAttributes()
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var key in Others.AllKeys)
            {
                if (!StringUtils.StartsWithIgnoreCase(key, "item"))
                {
                    nameValueCollection[key] = Others[key];
                }
            }
            if (string.IsNullOrEmpty(nameValueCollection["width"]))
            {
                nameValueCollection["width"] = "100%";
            }
            return nameValueCollection;
        }

        public NameValueCollection GetCellAttributes()
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var key in Others.AllKeys)
            {
                if (StringUtils.StartsWithIgnoreCase(key, "item"))
                {
                    var attributeName = StringUtils.ReplaceStartsWithIgnoreCase(key, "item", string.Empty);
                    if (StringUtils.EqualsIgnoreCase(attributeName, "VerticalAlign"))
                    {
                        nameValueCollection["valign"] = Others[key];
                    }
                    else
                    {
                        nameValueCollection[attributeName] = Others[key];
                    }
                }
            }

            return nameValueCollection;
        }
    }
}
