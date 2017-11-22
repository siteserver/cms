using System;
using System.Collections;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;
using System.Collections.Generic;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "下拉菜单", Description = "通过 stl:menu 标签在模板中显示栏目下拉菜单")]
    public class StlMenu
    {
        private StlMenu() { }
        public const string ElementName = "stl:menu";

        public const string AttributeStyleName = "styleName";
        public const string AttributeChannelIndex = "channelIndex";
        public const string AttributeChannelName = "channelName";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeIsShowChildren = "isShowChildren";
        public const string AttributeMenuWidth = "menuWidth";
        public const string AttributeMenuHeight = "menuHeight";
        public const string AttributeXPosition = "xPosition";
        public const string AttributeYPosition = "yPosition";
        public const string AttributeChildMenuDisplay = "childMenuDisplay";
        public const string AttributeChildMenuWidth = "childMenuWidth";
        public const string AttributeChildMenuHeight = "childMenuHeight";
        public const string AttributeTarget = "target";
        public const string AttributeIsDynamic = "isDynamic";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeStyleName, "样式名称"},
            {AttributeChannelIndex, "栏目索引"},
            {AttributeChannelName, "栏目名称"},
            {AttributeGroupChannel, "指定显示的栏目组"},
            {AttributeGroupChannelNot, "指定不显示的栏目组"},
            {AttributeIsShowChildren, "是否显示二级菜单"},
            {AttributeMenuWidth, "菜单宽度"},
            {AttributeMenuHeight, "菜单高度"},
            {AttributeXPosition, "菜单水平位置"},
            {AttributeYPosition, "菜单垂直位置"},
            {AttributeChildMenuDisplay, "二级菜单显示方式"},
            {AttributeChildMenuWidth, "二级菜单宽度"},
            {AttributeChildMenuHeight, "二级菜单高度"},
            {AttributeTarget, "打开窗口目标"},
            {AttributeIsDynamic, "是否动态显示"}
        };


        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            string parsedContent;
            try
            {
                var channelIndex = string.Empty;
                var channelName = string.Empty;
                var groupChannel = string.Empty;
                var groupChannelNot = string.Empty;
                var isShowChildren = false;
                var styleName = string.Empty;
                var menuWidth = string.Empty;
                var menuHeight = string.Empty;
                var xPosition = string.Empty;
                var yPosition = string.Empty;
                var childMenuDisplay = string.Empty;
                var childMenuWidth = string.Empty;
                var childMenuHeight = string.Empty;
                var target = string.Empty;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeStyleName))
                        {
                            styleName = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
                        {
                            channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
                        {
                            channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannel))
                        {
                            groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannelNot))
                        {
                            groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsShowChildren))
                        {
                            isShowChildren = TranslateUtils.ToBool(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeMenuWidth))
                        {
                            menuWidth = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeMenuHeight))
                        {
                            menuHeight = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeXPosition))
                        {
                            xPosition = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeYPosition))
                        {
                            yPosition = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChildMenuDisplay))
                        {
                            childMenuDisplay = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChildMenuWidth))
                        {
                            childMenuWidth = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChildMenuHeight))
                        {
                            childMenuHeight = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTarget))
                        {
                            target = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, channelIndex, channelName, groupChannel, groupChannelNot, isShowChildren, styleName, menuWidth, menuHeight, xPosition, yPosition, childMenuDisplay, childMenuWidth, childMenuHeight, target);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string channelIndex, string channelName, string groupChannel, string groupChannelNot, bool isShowChildren, string styleName, string menuWidth, string menuHeight, string xPosition, string yPosition, string childMenuDisplay, string childMenuWidth, string childMenuHeight, string target)
        {
            string parsedContent;

            var channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelId, channelIndex, channelName);
            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

            var innerHtml = nodeInfo.NodeName.Trim();
            if (!string.IsNullOrEmpty(node.InnerXml))
            {
                var innerBuilder = new StringBuilder(node.InnerXml);
                StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
                innerHtml = innerBuilder.ToString();
            }

            var nodeInfoArrayList = new ArrayList();//需要显示的栏目列表

            var childNodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(nodeInfo, EScopeType.Children, groupChannel, groupChannelNot);
            if (childNodeIdList != null && childNodeIdList.Count > 0)
            {
                foreach (int childNodeId in childNodeIdList)
                {
                    var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, childNodeId);
                    nodeInfoArrayList.Add(theNodeInfo);
                }
            }

            if (nodeInfoArrayList.Count == 0)
            {
                parsedContent = innerHtml;
            }
            else
            {
                var menuDisplayId = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(pageInfo.PublishmentSystemId, styleName);
                var menuDisplayInfo = menuDisplayId == 0 ? DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(pageInfo.PublishmentSystemId) : DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayId);
                var level2MenuDisplayInfo = menuDisplayInfo;
                if (isShowChildren && !string.IsNullOrEmpty(childMenuDisplay))
                {
                    var childMenuDisplayId = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(pageInfo.PublishmentSystemId, childMenuDisplay);
                    level2MenuDisplayInfo = childMenuDisplayId == 0 ? DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(pageInfo.PublishmentSystemId) : DataProvider.MenuDisplayDao.GetMenuDisplayInfo(childMenuDisplayId);
                }

                if (string.IsNullOrEmpty(menuWidth)) menuWidth = menuDisplayInfo.MenuWidth.ToString();
                if (string.IsNullOrEmpty(menuHeight)) menuHeight = menuDisplayInfo.MenuItemHeight.ToString();
                if (string.IsNullOrEmpty(xPosition)) xPosition = menuDisplayInfo.XPosition;
                if (string.IsNullOrEmpty(yPosition)) yPosition = menuDisplayInfo.YPosition;
                if (string.IsNullOrEmpty(childMenuWidth)) childMenuWidth = level2MenuDisplayInfo.MenuWidth.ToString();
                if (string.IsNullOrEmpty(childMenuHeight)) childMenuHeight = level2MenuDisplayInfo.MenuItemHeight.ToString();

                var createMenuString = string.Empty;

                var menuId = pageInfo.UniqueId;

                parsedContent =
                    $@"<span name=""mm_link_{menuId}"" id=""mm_link_{menuId}"" onmouseover=""MM_showMenu(window.mm_menu_{menuId},{xPosition},{yPosition},null,'mm_link_{menuId}');"" onmouseout=""MM_startTimeout();"">{innerHtml}</span>";

                var menuBuilder = new StringBuilder();
                var level2MenuBuilder = new StringBuilder();


                foreach (NodeInfo theNodeInfo in nodeInfoArrayList)
                {
                    var isLevel2Exist = false;
                    var level2MenuId = 0;

                    if (isShowChildren)
                    {
                        var level2NodeInfoArrayList = new ArrayList();

                        var level2NodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(theNodeInfo, EScopeType.Children, groupChannel, groupChannelNot);
                        if (level2NodeIdList != null && level2NodeIdList.Count > 0)
                        {
                            foreach (int level2NodeId in level2NodeIdList)
                            {
                                var level2NodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, level2NodeId);
                                level2NodeInfoArrayList.Add(level2NodeInfo);
                            }

                            if (level2NodeInfoArrayList.Count > 0)
                            {
                                isLevel2Exist = true;
                                var level2ChildMenuBuilder = new StringBuilder();
                                level2MenuId = pageInfo.UniqueId;

                                foreach (NodeInfo level2NodeInfo in level2NodeInfoArrayList)
                                {
                                    var level2NodeUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, level2NodeInfo);
                                    if (PageUtils.UnclickedUrl.Equals(level2NodeUrl))
                                    {
                                        level2ChildMenuBuilder.Append(
                                            $@"  mm_menu_{level2MenuId}.addMenuItem(""{level2NodeInfo.NodeName.Trim()}"", ""location='{level2NodeUrl}'"");");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(target))
                                        {
                                            if (target.ToLower().Equals("_blank"))
                                            {
                                                level2ChildMenuBuilder.Append(
                                                    $@"  mm_menu_{level2MenuId}.addMenuItem(""{level2NodeInfo.NodeName
                                                        .Trim()}"", ""window.open('{level2NodeUrl}', '_blank');"");");
                                            }
                                            else
                                            {
                                                level2ChildMenuBuilder.Append(
                                                    $@"  mm_menu_{level2MenuId}.addMenuItem(""{level2NodeInfo.NodeName
                                                        .Trim()}"", ""location='{level2NodeUrl}', '{target}'"");");
                                            }
                                        }
                                        else
                                        {
                                            level2ChildMenuBuilder.Append(
                                                $@"  mm_menu_{level2MenuId}.addMenuItem(""{level2NodeInfo.NodeName.Trim()}"", ""location='{level2NodeUrl}'"");");
                                        }
                                    }
                                }

                                string level2CreateMenuString = $@"
  window.mm_menu_{level2MenuId} = new Menu('{theNodeInfo.NodeName.Trim()}',{childMenuWidth},{childMenuHeight},'{level2MenuDisplayInfo
                                    .FontFamily}',{level2MenuDisplayInfo.FontSize},'{level2MenuDisplayInfo.FontColor}','{level2MenuDisplayInfo
                                    .FontColorHilite}','{level2MenuDisplayInfo.MenuItemBgColor}','{level2MenuDisplayInfo
                                    .MenuHiliteBgColor}','{level2MenuDisplayInfo.MenuItemHAlign}','{level2MenuDisplayInfo
                                    .MenuItemVAlign}',{level2MenuDisplayInfo.MenuItemPadding},{level2MenuDisplayInfo
                                    .MenuItemSpacing},{level2MenuDisplayInfo.HideTimeout},-5,7,true,{level2MenuDisplayInfo
                                    .MenuBgOpaque},{level2MenuDisplayInfo.Vertical},{level2MenuDisplayInfo
                                    .MenuItemIndent},true,true);
  {level2ChildMenuBuilder}
  mm_menu_{level2MenuId}.fontWeight='{level2MenuDisplayInfo.FontWeight}';
  mm_menu_{level2MenuId}.fontStyle='{level2MenuDisplayInfo.FontStyle}';
  mm_menu_{level2MenuId}.hideOnMouseOut={level2MenuDisplayInfo.HideOnMouseOut};
  mm_menu_{level2MenuId}.bgColor='{level2MenuDisplayInfo.BgColor}';
  mm_menu_{level2MenuId}.menuBorder={level2MenuDisplayInfo.MenuBorder};
  mm_menu_{level2MenuId}.menuLiteBgColor='{level2MenuDisplayInfo.MenuLiteBgColor}';
  mm_menu_{level2MenuId}.menuBorderBgColor='{level2MenuDisplayInfo.MenuBorderBgColor}';
";
                                level2MenuBuilder.Append(level2CreateMenuString);
                            }
                        }
                    }

                    string menuName;
                    if (isLevel2Exist)
                    {
                        menuName = "mm_menu_" + level2MenuId;
                    }
                    else
                    {
                        menuName = "\"" + theNodeInfo.NodeName.Trim() + "\"";
                    }

                    var nodeUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, theNodeInfo);
                    if (PageUtils.UnclickedUrl.Equals(nodeUrl))
                    {
                        menuBuilder.Append($@"  mm_menu_{menuId}.addMenuItem({menuName}, ""location='{nodeUrl}'"");");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(target))
                        {
                            menuBuilder.Append(
                                target.ToLower().Equals("_blank")
                                    ? $@"  mm_menu_{menuId}.addMenuItem({menuName}, ""window.open('{nodeUrl}', '_blank');"");"
                                    : $@"  mm_menu_{menuId}.addMenuItem({menuName}, ""location='{nodeUrl}', '{target}'"");");
                        }
                        else
                        {
                            menuBuilder.Append($@"  mm_menu_{menuId}.addMenuItem({menuName}, ""location='{nodeUrl}'"");");
                        }
                    }

                }

                var childMenuIcon = string.Empty;
                if (!string.IsNullOrEmpty(menuDisplayInfo.ChildMenuIcon))
                {
                    childMenuIcon = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, menuDisplayInfo.ChildMenuIcon);
                }
                createMenuString += $@"
  if (window.mm_menu_{menuId}) return;
  {level2MenuBuilder}
  window.mm_menu_{menuId} = new Menu('root',{menuWidth},{menuHeight},'{menuDisplayInfo.FontFamily}',{menuDisplayInfo
                    .FontSize},'{menuDisplayInfo.FontColor}','{menuDisplayInfo.FontColorHilite}','{menuDisplayInfo
                    .MenuItemBgColor}','{menuDisplayInfo.MenuHiliteBgColor}','{menuDisplayInfo.MenuItemHAlign}','{menuDisplayInfo
                    .MenuItemVAlign}',{menuDisplayInfo.MenuItemPadding},{menuDisplayInfo.MenuItemSpacing},{menuDisplayInfo
                    .HideTimeout},-5,7,true,{menuDisplayInfo.MenuBgOpaque},{menuDisplayInfo.Vertical},{menuDisplayInfo
                    .MenuItemIndent},true,true);
  {menuBuilder}
  mm_menu_{menuId}.fontWeight='{menuDisplayInfo.FontWeight}';
  mm_menu_{menuId}.fontStyle='{menuDisplayInfo.FontStyle}';
  mm_menu_{menuId}.hideOnMouseOut={menuDisplayInfo.HideOnMouseOut};
  mm_menu_{menuId}.bgColor='{menuDisplayInfo.BgColor}';
  mm_menu_{menuId}.menuBorder={menuDisplayInfo.MenuBorder};
  mm_menu_{menuId}.menuLiteBgColor='{menuDisplayInfo.MenuLiteBgColor}';
  mm_menu_{menuId}.menuBorderBgColor='{menuDisplayInfo.MenuBorderBgColor}';
  mm_menu_{menuId}.childMenuIcon = ""{childMenuIcon}"";

//NEXT
";

                var scriptBuilder = new StringBuilder();

                string functionHead =
                    $@"<script language=""JavaScript"" src=""{SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.MmMenu.Js)}""></script>
<script language=""JavaScript"">
//HEAD
function siteserverLoadMenus() {{";
                var functionFoot = @"
//FOOT
}
</script>
<script language=""JavaScript1.2"">siteserverLoadMenus();writeMenus();</script>";
                //取得已经保存的js
                var existScript = string.Empty;
                if (pageInfo.IsPageScriptsExists(PageInfo.JsAcMenuScripts, true))
                { 
                    existScript = pageInfo.GetPageScripts(PageInfo.JsAcMenuScripts, true);
                }
                if (string.IsNullOrEmpty(existScript) || existScript.IndexOf("//HEAD", StringComparison.Ordinal) < 0)
                { 
                    scriptBuilder.Append(functionHead);
                }
                scriptBuilder.Append(createMenuString);
                //scriptBuilder.Append(writeMenuString);
                if (string.IsNullOrEmpty(existScript) || existScript.IndexOf("//FOOT", StringComparison.Ordinal) < 0)
                { 
                    scriptBuilder.Append(functionFoot);
                }
                if (!string.IsNullOrEmpty(existScript) && existScript.IndexOf("//NEXT", StringComparison.Ordinal) >= 0)
                {
                    existScript = existScript.Replace("//NEXT", scriptBuilder.ToString());
                    pageInfo.SetPageScripts(PageInfo.JsAcMenuScripts, existScript, true);
                }
                else
                {
                    pageInfo.SetPageScripts(PageInfo.JsAcMenuScripts, scriptBuilder.ToString(), true);
                }
            }

            return parsedContent;
        }
    }
}
