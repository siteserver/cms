using System.Collections.Generic;
using System.Text;
using System.Web.UI.HtmlControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public class StlLocation
    {
        private StlLocation() { }
        public const string ElementName = "stl:location";

        public const string AttributeSeparator = "separator";
        public const string AttributeTarget = "target";
        public const string AttributeLinkClass = "linkClass";
        public const string AttributeWordNum = "wordNum";
        public const string AttributeIsContainSelf = "isContainSelf";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeSeparator, "当前位置分隔符"},
            {AttributeTarget, "打开窗口的目标"},
            {AttributeLinkClass, "链接CSS样式"},
            {AttributeWordNum, "链接字数"},
            {AttributeIsContainSelf, "是否包含当前栏目"}
        };


        //对“当前位置”（stl:location）元素进行解析
        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;

            foreach (var name in contextInfo.Attributes.Keys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, AttributeSeparator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeTarget))
                {
                    target = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeLinkClass))
                {
                    linkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeWordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, AttributeIsContainSelf))
                {
                    isContainSelf = TranslateUtils.ToBool(value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, separator, target, linkClass, wordNum,isContainSelf);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string separator, string target, string linkClass, int wordNum, bool isContainSelf)
        {
            if (!string.IsNullOrEmpty(contextInfo.InnerXml))
            {
                separator = contextInfo.InnerXml;
            }

            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

            var builder = new StringBuilder();

            var parentsPath = nodeInfo.ParentsPath;
            var parentsCount = nodeInfo.ParentsCount;
            if (parentsPath.Length != 0)
            {
                var nodePath = parentsPath;
                if (isContainSelf)
                {
                    nodePath = nodePath + "," + contextInfo.ChannelId;
                }
                var nodeIdArrayList = TranslateUtils.StringCollectionToStringList(nodePath);
                foreach (var nodeIdStr in nodeIdArrayList)
                {
                    var currentId = int.Parse(nodeIdStr);
                    var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, currentId);
                    if (currentId == pageInfo.PublishmentSystemId)
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetIndexPageUrl(pageInfo.PublishmentSystemInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, contextInfo.Attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                    else if (currentId == contextInfo.ChannelId)
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, currentNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, contextInfo.Attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));
                    }
                    else
                    {
                        var stlAnchor = new HtmlAnchor();
                        if (!string.IsNullOrEmpty(target))
                        {
                            stlAnchor.Target = target;
                        }
                        if (!string.IsNullOrEmpty(linkClass))
                        {
                            stlAnchor.Attributes.Add("class", linkClass);
                        }
                        var url = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, currentNodeInfo);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.NodeName, wordNum);

                        ControlUtils.AddAttributesIfNotExists(stlAnchor, contextInfo.Attributes);

                        builder.Append(ControlUtils.GetControlRenderHtml(stlAnchor));

                        if (parentsCount > 0)
                        {
                            builder.Append(separator);
                        }
                    }
                }
            }

            return builder.ToString();
        }
    }
}
