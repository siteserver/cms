using System.Text;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using SiteServer.CMS.Context;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [StlElement(Title = "当前位置", Description = "通过 stl:location 标签在模板中插入页面的当前位置")]
    public class StlLocation
    {
        private StlLocation() { }
        public const string ElementName = "stl:location";

        [StlAttribute(Title = "当前位置分隔符")]
        private const string Separator = nameof(Separator);

        [StlAttribute(Title = "打开窗口的目标")]
        private const string Target = nameof(Target);

        [StlAttribute(Title = "链接CSS样式")]
        private const string LinkClass = nameof(LinkClass);

        [StlAttribute(Title = "链接字数")]
        private const string WordNum = nameof(WordNum);

        [StlAttribute(Title = "是否包含当前栏目")]
        private const string IsContainSelf = nameof(IsContainSelf);

        //对“当前位置”（stl:location）元素进行解析
        public static async Task<object> ParseAsync(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var separator = " - ";
            var target = string.Empty;
            var linkClass = string.Empty;
            var wordNum = 0;
            var isContainSelf = true;

            foreach (var name in contextInfo.Attributes.AllKeys)
            {
                var value = contextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Separator))
                {
                    separator = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Target))
                {
                    target = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, LinkClass))
                {
                    linkClass = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, WordNum))
                {
                    wordNum = TranslateUtils.ToInt(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, IsContainSelf))
                {
                    isContainSelf = TranslateUtils.ToBool(value);
                }
            }

            return await ParseImplAsync(pageInfo, contextInfo, separator, target, linkClass, wordNum,isContainSelf);
        }

        private static async Task<string> ParseImplAsync(PageInfo pageInfo, ContextInfo contextInfo, string separator, string target, string linkClass, int wordNum, bool isContainSelf)
        {
            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                separator = contextInfo.InnerHtml;
            }

            var nodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, contextInfo.ChannelId);

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
                    var currentNodeInfo = await ChannelManager.GetChannelAsync(pageInfo.SiteId, currentId);
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
                        var url = await PageUtility.GetIndexPageUrlAsync(pageInfo.Site, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = WebUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
                        var url = await PageUtility.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = WebUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
                        var url = await PageUtility.GetChannelUrlAsync(pageInfo.Site, currentNodeInfo, pageInfo.IsLocal);
                        if (url.Equals(PageUtils.UnclickedUrl))
                        {
                            stlAnchor.Target = string.Empty;
                        }
                        stlAnchor.HRef = url;
                        stlAnchor.InnerHtml = WebUtils.MaxLengthText(currentNodeInfo.ChannelName, wordNum);

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
