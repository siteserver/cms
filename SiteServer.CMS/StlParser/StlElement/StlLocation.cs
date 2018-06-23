using System.Text;
using System.Web.UI.HtmlControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlClass(Usage = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public class StlLocation
    {
        private StlLocation() { }
        public const string ElementName = "stl:location";

        private static readonly Attr Separator = new Attr("separator", "当前位置分隔符");
        private static readonly Attr Target = new Attr("target", "打开窗口的目标");
        private static readonly Attr LinkClass = new Attr("linkClass", "链接CSS样式");
        private static readonly Attr WordNum = new Attr("wordNum", "链接字数");
        private static readonly Attr IsContainSelf = new Attr("isContainSelf", "是否包含当前栏目");

        //对“当前位置”（stl:location）元素进行解析
        public static string Parse(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Separator.Name))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Target.Name))
                {
                    target = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LinkClass.Name))
                {
                    linkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum.Name))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsContainSelf.Name))
                {
                    isContainSelf = TranslateUtils.ToBool(value);
                }
            }

            return ParseImpl(pageInfo, contextInfo, separator, target, linkClass, wordNum,isContainSelf);
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, string separator, string target, string linkClass, int wordNum, bool isContainSelf)
        {
            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                separator = contextInfo.InnerHtml;
            }

            var nodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, contextInfo.ChannelId);

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
                var channelIdArrayList = TranslateUtils.StringCollectionToStringList(nodePath);
                foreach (var channelIdStr in channelIdArrayList)
                {
                    var currentId = int.Parse(channelIdStr);
                    var currentNodeInfo = ChannelManager.GetChannelInfo(pageInfo.SiteId, currentId);
                    if (currentId == pageInfo.SiteId)
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
                        var url = PageUtility.GetIndexPageUrl(pageInfo.SiteInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
                        var url = PageUtility.GetChannelUrl(pageInfo.SiteInfo, currentNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
                        var url = PageUtility.GetChannelUrl(pageInfo.SiteInfo, currentNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = StringUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
