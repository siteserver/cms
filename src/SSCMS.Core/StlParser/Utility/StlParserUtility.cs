using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.Utility
{
    public static class StlParserUtility
    {
        public const string PageContent = nameof(PageContent);
        public const string CountOfChannels = nameof(CountOfChannels);
        public const string CountOfContents = nameof(CountOfContents);
        public const string CountOfImageContents = nameof(CountOfImageContents);
        public const string ItemIndex = nameof(ItemIndex);
        public const string Title = nameof(Title);

        public const string OrderDefault = "Default";								//默认排序
        public const string OrderBack = "Back";								//默认排序的相反方向
        public const string OrderAddDate = "AddDate";							//添加时间
        public const string OrderAddDateBack = "AddDateBack";					//添加时间的相反方向
        public const string OrderLastModifiedDate = "LastModifiedDate";				//最后更改时间（只可用于内容列表）
        public const string OrderLastModifiedDateBack = "LastModifiedDateBack";	//最后更改时间的相反方向（只可用于内容列表）
        public const string OrderHits = "Hits";	            //点击量
        public const string OrderHitsByDay = "HitsByDay";	    //日点击量
        public const string OrderHitsByWeek = "HitsByWeek";	//周点击量
        public const string OrderHitsByMonth = "HitsByMonth";	//月点击量
        public const string OrderRandom = "Random";            //随机

        

        public static bool IsStlChannelElementWithTypePageContent(List<string> list)
        {
            foreach (var label in list)
            {
                if (!IsStlChannelElement(label, PageContent)) continue;
                return true;
            }
            return false;
        }

        public static string GetStlChannelElementWithTypePageContent(List<string> labelList)
        {
            var stlPageChannelElement = string.Empty;
            foreach (var label in labelList)
            {
                if (!IsStlChannelElement(label, PageContent)) continue;
                stlPageChannelElement = label;
                break;
            }
            return stlPageChannelElement;
        }

        public static bool IsStlContentElementWithTypePageContent(List<string> list)
        {
            foreach (var label in list)
            {
                if (!IsStlContentElement(label, ColumnsManager.PageContent)) continue;
                return true;
            }
            return false;
        }

        public static string GetStlContentElementWithTypePageContent(List<string> labelList)
        {
            var stlPageContentElement = string.Empty;
            foreach (var label in labelList)
            {
                if (!IsStlContentElement(label, ColumnsManager.PageContent)) continue;
                stlPageContentElement = label;
                break;
            }
            return stlPageContentElement;
        }

        

        public static string GetNameFromEntity(string stlEntity)
        {
            var name = stlEntity;
            if (stlEntity.IndexOf("_", StringComparison.Ordinal) != -1)
            {
                name = stlEntity.Substring(0, stlEntity.IndexOf("_", StringComparison.Ordinal));
                name = name + "}";
            }
            return name;
        }

        public static string GetValueFromEntity(string stlEntity)
        {
            if (stlEntity.IndexOf("_", StringComparison.Ordinal) == -1) return string.Empty;

            var value = string.Empty;
            try
            {
                var length = stlEntity.LastIndexOf("}", StringComparison.Ordinal) - stlEntity.LastIndexOf("_", StringComparison.Ordinal) - 1;
                value = stlEntity.Substring(stlEntity.LastIndexOf("_", StringComparison.Ordinal) + 1, length);
            }
            catch
            {
                // ignored
            }
            return value;
        }

        public static bool IsStlEntityInclude(string content)
        {
            if (content == null) return false;
            content = StringUtils.ToLower(content);
            return content.Contains('}') && (content.IndexOf("{stl:", StringComparison.Ordinal) != -1 || content.IndexOf("{stl.", StringComparison.Ordinal) != -1 || content.IndexOf("{content.", StringComparison.Ordinal) != -1 || content.IndexOf("{channel.", StringComparison.Ordinal) != -1);
        }

        public static Regex GetStlEntityRegex(string entityName)
        {
            return new Regex($@"{{{entityName}.[^{{}}]*}}", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        //判断属于某种类型（type）的<stl:content>元素是否存在
        public static bool IsStlContentElement(string labelString, string type)
        {
            return RegexUtils.IsMatch($@"<stl:content[^>]+type=""{type}""[^>]*>", labelString);
        }

        //判断属于某种类型（type）的<stl:channel>元素是否存在
        public static bool IsStlChannelElement(string labelString, string type)
        {
            return RegexUtils.IsMatch($@"<stl:channel[^>]+type=""{type}""[^>]*>", labelString);
        }

        public static StlElementInfo ParseStlElement(string stlElement, int startIndex)
        {
            StlElementInfo retVal = null;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    var name = stlNode.Name;
                    //var outerHtml = stlNode.OuterHtml;
                    var innerHtml = stlNode.InnerHtml;
                    var attributesIgnoreCase = TranslateUtils.NewIgnoreCaseNameValueCollection();

                    if (attributesIgnoreCase != null && stlNode.Attributes != null)
                    {
                        foreach (var attribute in stlNode.Attributes)
                        {
                            attributesIgnoreCase[attribute.Name] = attribute.Value;
                        }
                    }

                    if (!string.IsNullOrEmpty(name))
                    {
                        retVal = new StlElementInfo(StringUtils.ToLower(name), attributesIgnoreCase, stlElement, innerHtml, startIndex);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retVal;
        }

        public static int ParseItemIndex(int dbItemIndex, string attributeName, ParseContext contextInfo)
        {
            var itemIndex = contextInfo.PageItemIndex + dbItemIndex + 1;
            if (attributeName.IndexOf('+') == -1 && attributeName.IndexOf('-') == -1) return itemIndex;

            var array = attributeName.Split('+');
            if (array.Length == 2)
            {
                var addNum = TranslateUtils.ToInt(array[1].Trim(), 1);
                return itemIndex + addNum;
            }

            array = attributeName.Split('-');
            if (array.Length != 2) return itemIndex;

            var substractNum = TranslateUtils.ToInt(array[1].Trim(), 1);
            return itemIndex - substractNum;
        }

        public static int GetItemIndex(ParseContext contextInfo)
        {
            var dbItemIndex = 0;
            if (contextInfo.ContextType == ParseType.Channel)
            {
                dbItemIndex = contextInfo.ItemContainer.ChannelItem.Key;
            }
            else if (contextInfo.ContextType == ParseType.Content)
            {
                dbItemIndex = contextInfo.ItemContainer.ContentItem.Key;
            }
            else if (contextInfo.ContextType == ParseType.SqlContent)
            {
                dbItemIndex = contextInfo.ItemContainer.SqlItem.Key;
            }
            else if (contextInfo.ContextType == ParseType.Site)
            {
                dbItemIndex = contextInfo.ItemContainer.SiteItem.Key;
            }
            else if (contextInfo.ContextType == ParseType.Each)
            {
                dbItemIndex = contextInfo.ItemContainer.EachItem.Key;
            }

            return contextInfo.PageItemIndex + dbItemIndex + 1;
        }

        public static async Task<string> GetStlCurrentUrlAsync(IParseManager parseManager, Site site, int channelId, int contentId, Content content, TemplateType templateType, int templateId, bool isLocal)
        {
            var currentUrl = string.Empty;
            if (templateType == TemplateType.IndexPageTemplate)
            {
                currentUrl = await parseManager.PathManager.GetWebUrlAsync(site);
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                if (content == null)
                {
                    var nodeInfo = await parseManager.DatabaseManager.ChannelRepository.GetAsync(channelId);
                    currentUrl = await parseManager.PathManager.GetContentUrlAsync(site, nodeInfo, contentId, isLocal);
                }
                else
                {
                    currentUrl = await parseManager.PathManager.GetContentUrlAsync(site, content, isLocal);
                }
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                currentUrl = await parseManager.PathManager.GetChannelUrlAsync(site, await parseManager.DatabaseManager.ChannelRepository.GetAsync(channelId), isLocal);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                currentUrl = await parseManager.PathManager.GetFileUrlAsync(site, templateId, isLocal);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public static void GetLoading(string innerHtml, out string loading, out string template)
        {
            loading = string.Empty;
            template = string.Empty;
            if (string.IsNullOrEmpty(innerHtml)) return;

            var stlElementList = ParseUtils.GetStlElements(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var stlElement in stlElementList)
                {
                    if (ParseUtils.IsSpecifiedStlElement(stlElement, StlLoading.ElementName))
                    {
                        loading = ParseUtils.GetInnerHtml(stlElement);
                        template = innerHtml.Replace(stlElement, string.Empty);
                    }
                }
            }

            if (string.IsNullOrEmpty(loading) && string.IsNullOrEmpty(template))
            {
                template = innerHtml;
            }

            loading = StringUtils.Trim(loading);
            template = StringUtils.Trim(template);
        }

        public static void GetYesNo(string innerHtml, out string yes, out string no)
        {
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerHtml)) return;

            var stlElementList = ParseUtils.GetStlElements(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = ParseUtils.GetInnerHtml(theStlElement);
                    }
                    else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = ParseUtils.GetInnerHtml(theStlElement);
                    }
                }
            }

            if (string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                yes = innerHtml;
            }

            yes = StringUtils.Trim(yes);
            no = StringUtils.Trim(no);
        }

        public static void GetLoadingYesNo(string innerHtml, out string loading, out string yes, out string no)
        {
            loading = string.Empty;
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerHtml)) return;

            var stlElementList = ParseUtils.GetStlElements(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                    {
                        loading = ParseUtils.GetInnerHtml(theStlElement);
                    }
                    else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = ParseUtils.GetInnerHtml(theStlElement);
                    }
                    else if (ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || ParseUtils.IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = ParseUtils.GetInnerHtml(theStlElement);
                    }
                }
            }

            if (string.IsNullOrEmpty(loading) && string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                yes = innerHtml;
            }

            loading = StringUtils.Trim(loading);
            yes = StringUtils.Trim(yes);
            no = StringUtils.Trim(no);
        }
    }
}
