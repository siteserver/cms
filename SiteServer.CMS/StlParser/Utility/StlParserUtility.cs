using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;

namespace SiteServer.CMS.StlParser.Utility
{
    public static class StlParserUtility
    {
        public const string OrderDefault = "Default";								//默认排序
        public const string OrderBack = "Back";								//默认排序的相反方向
        public const string OrderAddDate = "AddDate";							//添加时间
        public const string OrderAddDateBack = "AddDateBack";					//添加时间的相反方向
        public const string OrderLastEditDate = "LastEditDate";				//最后更改时间（只可用于内容列表）
        public const string OrderLastEditDateBack = "LastEditDateBack";	//最后更改时间的相反方向（只可用于内容列表）
        public const string OrderHits = "Hits";	            //点击量
        public const string OrderHitsByDay = "HitsByDay";	    //日点击量
        public const string OrderHitsByWeek = "HitsByWeek";	//周点击量
        public const string OrderHitsByMonth = "HitsByMonth";	//月点击量
        public const string OrderRandom = "Random";            //随机

        //http://weblogs.asp.net/whaggard/archive/2005/02/20/377025.aspx
        //        public static Regex REGEX_STL_ELEMENT = new Regex(@"
        //<stl:(\w+?)[^>]*>
        //  (?>
        //      <stl:\1[^>]*> (?<LEVEL>)
        //    | 
        //      </stl:\1[^>]*> (?<-LEVEL>)
        //    |
        //      (?! <stl:\1[^>]*> | </stl:\1[^>]*> ).
        //  )*
        //  (?(LEVEL)(?!))
        //</stl:\1[^>]*>
        //", ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace) | RegexOptions.Compiled);
        public static readonly Regex RegexStlElement = new Regex(@"
<stl:(\w+?)[^>]*>
  (?>
      <stl:\1[^>]*> (?<LEVEL>)
    | 
      </stl:\1[^>]*> (?<-LEVEL>)
    |
      (?! <stl:\1[^>]*> | </stl:\1[^>]*> ).
  )*
  (?(LEVEL)(?!))
</stl:\1[^>]*>|<stl:(\w+?)[^>]*/>
", ((RegexOptions.Singleline | RegexOptions.IgnoreCase) | RegexOptions.IgnorePatternWhitespace) | RegexOptions.Compiled);

        public static List<string> GetStlLabelList(string templateContent)
        {
            var stlElementList = GetStlElementList(templateContent);
            var stlEntityList = GetStlEntityList(templateContent);
            stlElementList.AddRange(stlEntityList);
            return stlElementList;
        }

        //需要修改
        public static bool IsStlElementExists(string stlElementName, List<string> list)
        {
            var exists = false;
            foreach (var label in list)
            {
                if (label.ToLower().StartsWith($"<{stlElementName.ToLower()} ") || label.ToLower().StartsWith(
                        $"<{stlElementName.ToLower()}>"))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public static bool IsStlContentElementWithTypePageContent(List<string> list)
        {
            foreach (var label in list)
            {
                if (!IsStlContentElement(label, ContentAttribute.PageContent)) continue;
                return true;
            }
            return false;
        }

        public static string GetStlElement(string stlElementName, List<string> labelList)
        {
            var stlElement = string.Empty;
            foreach (var labelWithDisplayModeEnNameAndChannelId in labelList)
            {
                if (labelWithDisplayModeEnNameAndChannelId.ToLower().StartsWith($"<{stlElementName.ToLower()} ") || labelWithDisplayModeEnNameAndChannelId.ToLower().StartsWith(
                        $"<{stlElementName.ToLower()}>"))
                {
                    stlElement = labelWithDisplayModeEnNameAndChannelId;
                    break;
                }
            }
            return stlElement;
        }

        public static string GetStlContentElementWithTypePageContent(List<string> labelList)
        {
            var stlPageContentElement = string.Empty;
            foreach (var label in labelList)
            {
                if (!IsStlContentElement(label, ContentAttribute.PageContent)) continue;
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
            content = content.ToLower();
            return StringUtils.Contains(content, "}") && (StringUtils.Contains(content, "{stl.") || StringUtils.Contains(content, "{content.") || StringUtils.Contains(content, "{channel."));
        }

        public static bool IsSpecifiedStlElement(string stlElement, string elementName)
        {
            if (stlElement == null) return false;
            return (StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName} ") ||
                    StringUtils.StartsWithIgnoreCase(stlElement, $"<{elementName}>")) &&
                   (StringUtils.EndsWithIgnoreCase(stlElement, $"</{elementName}>") ||
                    StringUtils.EndsWithIgnoreCase(stlElement, "/>"));
        }

        public static Regex GetStlEntityRegex(string entityName)
        {
            return new Regex($@"{{{entityName}.[^{{}}]*}}", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        /// <summary>
        /// 得到内容中的STL元素列表
        /// </summary>
        public static List<string> GetStlElementList(string templateContent)
        {
            var stlElementList = new List<string>();

            var mc = RegexStlElement.Matches(templateContent);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlElement = mc[i].Value;
                stlElementList.Add(stlElement);
            }

            return stlElementList;
        }

        /// <summary>
        /// 得到内容中的STL实体列表
        /// </summary>
        /// <param name="content">需要解析的内容</param>
        /// <returns></returns>
        public static List<string> GetStlEntityList(string content)
        {
            //首先需要去除<stl:项
            //content = RegexUtils.Replace(@"<stl:(\w+)[^>]*>.*?<\/stl:\1>", content, string.Empty);
            content = RegexStlElement.Replace(content, string.Empty);

            var stlEntityList = new List<string>();

            //Regex regex = new Regex(@"{[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            //Regex regex = new Regex(@"{stl\.[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{navigation\.[^{}]*}", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var regex = new Regex(EStlEntityTypeUtils.RegexStringAll, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            var mc = regex.Matches(content);
            for (var i = 0; i < mc.Count; i++)
            {
                var stlEntity = mc[i].Value;
                stlEntityList.Add(stlEntity);
            }

            return stlEntityList;
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

        public static string GetInnerHtml(string stlElement)
        {
            var retval = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    retval = stlNode.InnerHtml;
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        public static string GetInnerHtml(string stlElement, NameValueCollection attributes)
        {
            var retval = string.Empty;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    retval = stlNode.InnerHtml;

                    if (attributes != null && stlNode.Attributes != null)
                    {
                        foreach (var attribute in stlNode.Attributes)
                        {
                            attributes[attribute.Name] = attribute.Value;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        public static StlElementInfo ParseStlElement(string stlElement)
        {
            StlElementInfo retval = null;

            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(stlElement);
                var docNode = htmlDoc.DocumentNode;
                if (docNode?.FirstChild != null)
                {
                    var stlNode = docNode.FirstChild;

                    var name = stlNode.Name;
                    var outerHtml = stlNode.OuterHtml;
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
                        retval = new StlElementInfo(name.ToLower(), attributesIgnoreCase, outerHtml, innerHtml);
                    }
                }
            }
            catch
            {
                // ignored
            }

            return retval;
        }

        public const string ItemIndex = "ItemIndex";

        public static int ParseItemIndex(int dbItemIndex, string attributeName, ContextInfo contextInfo)
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

        public static int GetItemIndex(ContextInfo contextInfo)
        {
            var dbItemIndex = 0;
            if (contextInfo.ContextType == EContextType.Channel)
            {
                dbItemIndex = contextInfo.ItemContainer.ChannelItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Content)
            {
                dbItemIndex = contextInfo.ItemContainer.ContentItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.SqlContent)
            {
                dbItemIndex = contextInfo.ItemContainer.SqlItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Site)
            {
                dbItemIndex = contextInfo.ItemContainer.SiteItem.ItemIndex;
            }
            else if (contextInfo.ContextType == EContextType.Each)
            {
                dbItemIndex = contextInfo.ItemContainer.EachItem.ItemIndex;
            }

            return contextInfo.PageItemIndex + dbItemIndex + 1;
        }

        public static string GetAjaxDivId(int updaterId)
        {
            return "ajaxElement_" + updaterId + "_" + StringUtils.GetRandomInt(100, 1000);
        }

        public static string GetStlCurrentUrl(SiteInfo siteInfo, int channelId, int contentId, IContentInfo contentInfo, TemplateType templateType, int templateId, bool isLocal)
        {
            var currentUrl = string.Empty;
            if (templateType == TemplateType.IndexPageTemplate)
            {
                currentUrl = siteInfo.Additional.WebUrl;
            }
            else if (templateType == TemplateType.ContentTemplate)
            {
                if (contentInfo == null)
                {
                    var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                    currentUrl = PageUtility.GetContentUrl(siteInfo, nodeInfo, contentId, isLocal);
                }
                else
                {
                    currentUrl = PageUtility.GetContentUrl(siteInfo, contentInfo, isLocal);
                }
            }
            else if (templateType == TemplateType.ChannelTemplate)
            {
                currentUrl = PageUtility.GetChannelUrl(siteInfo, ChannelManager.GetChannelInfo(siteInfo.Id, channelId), isLocal);
            }
            else if (templateType == TemplateType.FileTemplate)
            {
                currentUrl = PageUtility.GetFileUrl(siteInfo, templateId, isLocal);
            }
            //currentUrl是当前页面的地址，前后台分离的时候，不允许带上protocol
            //return PageUtils.AddProtocolToUrl(currentUrl);
            return currentUrl;
        }

        public static void GetYesNo(string innerHtml, out string yes, out string no)
        {
            yes = string.Empty;
            no = string.Empty;
            if (string.IsNullOrEmpty(innerHtml)) return;

            var stlElementList = GetStlElementList(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = GetInnerHtml(theStlElement);
                    }
                    else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = GetInnerHtml(theStlElement);
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

            var stlElementList = GetStlElementList(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    if (IsSpecifiedStlElement(theStlElement, StlLoading.ElementName))
                    {
                        loading = GetInnerHtml(theStlElement);
                    }
                    else if (IsSpecifiedStlElement(theStlElement, StlYes.ElementName) || IsSpecifiedStlElement(theStlElement, StlYes.ElementName2))
                    {
                        yes = GetInnerHtml(theStlElement);
                    }
                    else if (IsSpecifiedStlElement(theStlElement, StlNo.ElementName) || IsSpecifiedStlElement(theStlElement, StlNo.ElementName2))
                    {
                        no = GetInnerHtml(theStlElement);
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

        public static Dictionary<string, string> GetStlElements(string innerHtml, List<string> stlElementNames)
        {
            var dic = new Dictionary<string, string>();
            foreach (var stlElementName in stlElementNames)
            {
                dic[stlElementName] = string.Empty;
            }
            if (string.IsNullOrEmpty(innerHtml)) return dic;

            var stlElementList = GetStlElementList(innerHtml);
            if (stlElementList.Count > 0)
            {
                foreach (var theStlElement in stlElementList)
                {
                    foreach (var stlElementName in stlElementNames)
                    {
                        if (IsSpecifiedStlElement(theStlElement, stlElementName))
                        {
                            var template = GetInnerHtml(theStlElement);
                            dic[stlElementName] = template;
                        }
                    }
                }
            }

            return dic;
        }
    }
}
