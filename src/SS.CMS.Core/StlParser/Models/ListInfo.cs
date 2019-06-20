using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Core.StlParser.StlElement;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.Models
{
    public class ListInfo
    {
        private EContextType _contextType = EContextType.Content;

        public static ListInfo GetListInfo(ParseContext parseContext)
        {
            var listInfo = new ListInfo
            {
                _contextType = parseContext.ContextType
            };

            var innerHtml = parseContext.InnerHtml;
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
                        else if (StlParserUtility.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerHtml(theStlElement));
                            parseContext.ParseInnerContent(innerBuilder);
                            listInfo.LoadingTemplate = innerBuilder.ToString();
                            innerHtml = innerHtml.Replace(theStlElement, string.Empty);
                        }
                        else if (parseContext.ContextType == EContextType.SqlContent && StlParserUtility.IsSpecifiedStlElement(theStlElement, StlQueryString.ElementName))
                        {
                            var innerBuilder = new StringBuilder(StlParserUtility.GetInnerHtml(theStlElement));
                            parseContext.ParseInnerContent(innerBuilder);
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

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelIndex))
                {
                    listInfo.ChannelIndex = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.ChannelName))
                {
                    listInfo.ChannelName = parseContext.ReplaceStlEntitiesForAttributeValue(value);
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
                    listInfo.Scope = ScopeType.Parse(value);
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
                    listInfo.TotalNum = TranslateUtils.ToInt(parseContext.ReplaceStlEntitiesForAttributeValue(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.PageNum))
                {
                    listInfo.PageNum = TranslateUtils.ToInt(parseContext.ReplaceStlEntitiesForAttributeValue(value), Constants.PageSize);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlPageContents.MaxPage))
                {
                    listInfo.MaxPage = TranslateUtils.ToInt(parseContext.ReplaceStlEntitiesForAttributeValue(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.StartNum))
                {
                    listInfo.StartNum = TranslateUtils.ToInt(parseContext.ReplaceStlEntitiesForAttributeValue(value));
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Order))
                {
                    listInfo.Order = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannel))
                {
                    listInfo.GroupChannel = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (string.IsNullOrEmpty(listInfo.GroupChannel))
                    {
                        listInfo.GroupChannel = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupChannelNot))
                {
                    listInfo.GroupChannelNot = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (string.IsNullOrEmpty(listInfo.GroupChannelNot))
                    {
                        listInfo.GroupChannelNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContent) || StringUtils.EqualsIgnoreCase(name, "group"))
                {
                    listInfo.GroupContent = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (string.IsNullOrEmpty(listInfo.GroupContent))
                    {
                        listInfo.GroupContent = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.GroupContentNot) || StringUtils.EqualsIgnoreCase(name, "groupNot"))
                {
                    listInfo.GroupContentNot = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                    if (string.IsNullOrEmpty(listInfo.GroupContentNot))
                    {
                        listInfo.GroupContentNot = "__Empty__";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Tags))
                {
                    listInfo.Tags = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Columns))
                {
                    listInfo.Columns = TranslateUtils.ToInt(value);
                    listInfo.Layout = ELayout.Table;
                    if (listInfo.Columns > 1 && isSetDirection == false)
                    {
                        listInfo.Direction = "horizontal";
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, StlListBase.Direction))
                {
                    listInfo.Layout = ELayout.Table;
                    listInfo.Direction = value;
                    isSetDirection = true;
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
                    listInfo.Layout = ELayoutUtils.GetEnumType(value);
                }
                else if (parseContext.ContextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConfigName))
                {
                    listInfo.DatabaseType =
                        DatabaseType.Parse(parseContext.Configuration[$"{StlSqlContents.ConfigName}:{StlSqlContents.DatabaseType}"]);
                    listInfo.ConnectionString = parseContext.Configuration[$"{StlSqlContents.ConfigName}:{StlSqlContents.ConnectionString}"];
                    if (string.IsNullOrEmpty(listInfo.ConnectionString))
                    {
                        listInfo.DatabaseType = parseContext.SettingsManager.DatabaseType;
                        listInfo.ConnectionString = parseContext.SettingsManager.DatabaseConnectionString;
                    }
                }
                else if (parseContext.ContextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.DatabaseType))
                {
                    listInfo.DatabaseType = DatabaseType.Parse(value);
                }
                else if (parseContext.ContextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.ConnectionString))
                {
                    listInfo.ConnectionString = value;
                }
                else if (parseContext.ContextType == EContextType.SqlContent && StringUtils.EqualsIgnoreCase(name, StlSqlContents.QueryString))
                {
                    listInfo.QueryString = parseContext.ReplaceStlEntitiesForAttributeValue(value);
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

        public NameValueCollection SelectedItems { get; } = TranslateUtils.NewIgnoreCaseNameValueCollection();

        public NameValueCollection SelectedValues { get; } = TranslateUtils.NewIgnoreCaseNameValueCollection();

        public string SeparatorRepeatTemplate { get; set; } = string.Empty;

        public int SeparatorRepeat { get; set; }

        public int TotalNum { get; set; }

        public int PageNum { get; set; } = Constants.PageSize;

        public int MaxPage { get; set; }

        public int StartNum { get; set; } = 1;

        public string Order { get; set; }

        public string GroupChannel { get; set; } = string.Empty;

        public string GroupChannelNot { get; set; } = string.Empty;

        public string GroupContent { get; set; } = string.Empty;

        public string GroupContentNot { get; set; } = string.Empty;

        public string Tags { get; set; } = string.Empty;

        public int Columns { get; set; }

        public string Direction { get; set; } = "vertical";

        public bool? IsImage { get; set; }

        public bool? IsVideo { get; set; }

        public bool? IsFile { get; set; }

        public bool IsRelatedContents { get; set; }

        public ELayout Layout { get; set; } = ELayout.None;

        public DatabaseType DatabaseType { get; set; }

        public string ConnectionString { get; set; } = string.Empty;

        public string QueryString { get; set; } = string.Empty;

        public string ChannelName { get; set; } = string.Empty;

        public string ChannelIndex { get; set; } = string.Empty;

        public int UpLevel { get; set; }

        public int TopLevel { get; set; } = -1;

        public ScopeType Scope { get; set; }

        public bool? IsTop { get; set; }

        public bool? IsRecommend { get; set; }

        public bool? IsHot { get; set; }

        public bool? IsColor { get; set; }

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
