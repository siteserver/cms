using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlMenu
    {
        private StlMenu() { }
        public const string ElementName = "stl:menu";//获取链接

        public const string Attribute_StyleName = "stylename";				    //样式名称
        public const string Attribute_ChannelIndex = "channelindex";			//栏目索引
        public const string Attribute_ChannelName = "channelname";				//栏目名称
        public const string Attribute_GroupChannel = "groupchannel";		    //指定显示的栏目组
        public const string Attribute_GroupChannelNot = "groupchannelnot";	    //指定不显示的栏目组
        public const string Attribute_IsShowChildren = "isshowchildren";		//是否显示二级菜单
        public const string Attribute_MenuWidth = "menuwidth";					//菜单宽度
        public const string Attribute_MenuHeight = "menuheight";				//菜单高度
        public const string Attribute_XPosition = "xposition";					//菜单水平位置
        public const string Attribute_YPosition = "yposition";					//菜单垂直位置
        public const string Attribute_ChildMenuDisplay = "childmenudisplay";	//二级菜单显示方式
        public const string Attribute_ChildMenuWidth = "childmenuwidth";		//二级菜单宽度
        public const string Attribute_ChildMenuHeight = "childmenuheight";		//二级菜单高度
        public const string Attribute_Target = "target";						//打开窗口目标
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();
                attributes.Add(Attribute_StyleName, "样式名称");
                attributes.Add(Attribute_ChannelIndex, "栏目索引");
                attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_GroupChannel, "指定显示的栏目组");
                attributes.Add(Attribute_GroupChannelNot, "指定不显示的栏目组");
                attributes.Add(Attribute_IsShowChildren, "是否显示二级菜单");
                attributes.Add(Attribute_MenuWidth, "菜单宽度");
                attributes.Add(Attribute_MenuHeight, "菜单高度");
                attributes.Add(Attribute_XPosition, "菜单水平位置");
                attributes.Add(Attribute_YPosition, "菜单垂直位置");
                attributes.Add(Attribute_ChildMenuDisplay, "二级菜单显示方式");
                attributes.Add(Attribute_ChildMenuWidth, "二级菜单宽度");
                attributes.Add(Attribute_ChildMenuHeight, "二级菜单高度");
                attributes.Add(Attribute_Target, "打开窗口目标");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
                return attributes;
            }
        }


        //对“栏目链接”（stl:menu）元素进行解析
        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
        {
            var parsedContent = string.Empty;
            try
            {
                var ie = node.Attributes.GetEnumerator();
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

                while (ie.MoveNext())
                {
                    var attr = (XmlAttribute)ie.Current;
                    var attributeName = attr.Name.ToLower();
                    if (attributeName.Equals(Attribute_StyleName))
                    {
                        styleName = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChannelIndex))
                    {
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_ChannelName))
                    {
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannel))
                    {
                        groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannelNot))
                    {
                        groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_IsShowChildren))
                    {
                        isShowChildren = TranslateUtils.ToBool(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_MenuWidth))
                    {
                        menuWidth = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_MenuHeight))
                    {
                        menuHeight = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_XPosition))
                    {
                        xPosition = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_YPosition))
                    {
                        yPosition = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChildMenuDisplay))
                    {
                        childMenuDisplay = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChildMenuWidth))
                    {
                        childMenuWidth = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_ChildMenuHeight))
                    {
                        childMenuHeight = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_Target))
                    {
                        target = attr.Value;
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
                }

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = ParseImpl(node, pageInfo, contextInfo, channelIndex, channelName, groupChannel, groupChannelNot, isShowChildren, styleName, menuWidth, menuHeight, xPosition, yPosition, childMenuDisplay, childMenuWidth, childMenuHeight, target);
                }
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string channelIndex, string channelName, string groupChannel, string groupChannelNot, bool isShowChildren, string styleName, string menuWidth, string menuHeight, string xPosition, string yPosition, string childMenuDisplay, string childMenuWidth, string childMenuHeight, string target)
        {
            var parsedContent = string.Empty;

            var channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, contextInfo.ChannelID, channelIndex, channelName);
            var nodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

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
                foreach (int childNodeID in childNodeIdList)
                {
                    var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, childNodeID);
                    nodeInfoArrayList.Add(theNodeInfo);
                }
            }

            if (nodeInfoArrayList.Count == 0)
            {
                parsedContent = innerHtml;
            }
            else
            {
                MenuDisplayInfo menuDisplayInfo = null;
                var menuDisplayID = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(pageInfo.PublishmentSystemId, styleName);
                if (menuDisplayID == 0)
                {
                    menuDisplayInfo = DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(pageInfo.PublishmentSystemId);
                }
                else
                {
                    menuDisplayInfo = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(menuDisplayID);
                }
                var level2MenuDisplayInfo = menuDisplayInfo;
                if (isShowChildren && !string.IsNullOrEmpty(childMenuDisplay))
                {
                    var childMenuDisplayID = DataProvider.MenuDisplayDao.GetMenuDisplayIdByName(pageInfo.PublishmentSystemId, childMenuDisplay);
                    if (childMenuDisplayID == 0)
                    {
                        level2MenuDisplayInfo = DataProvider.MenuDisplayDao.GetDefaultMenuDisplayInfo(pageInfo.PublishmentSystemId);
                    }
                    else
                    {
                        level2MenuDisplayInfo = DataProvider.MenuDisplayDao.GetMenuDisplayInfo(childMenuDisplayID);
                    }
                }

                if (string.IsNullOrEmpty(menuWidth)) menuWidth = menuDisplayInfo.MenuWidth.ToString();
                if (string.IsNullOrEmpty(menuHeight)) menuHeight = menuDisplayInfo.MenuItemHeight.ToString();
                if (string.IsNullOrEmpty(xPosition)) xPosition = menuDisplayInfo.XPosition;
                if (string.IsNullOrEmpty(yPosition)) yPosition = menuDisplayInfo.YPosition;
                if (string.IsNullOrEmpty(childMenuWidth)) childMenuWidth = level2MenuDisplayInfo.MenuWidth.ToString();
                if (string.IsNullOrEmpty(childMenuHeight)) childMenuHeight = level2MenuDisplayInfo.MenuItemHeight.ToString();

                var createMenuString = string.Empty;
                var writeMenuString = string.Empty;

                var menuID = pageInfo.UniqueId;

                parsedContent =
                    $@"<span name=""mm_link_{menuID}"" id=""mm_link_{menuID}"" onmouseover=""MM_showMenu(window.mm_menu_{menuID},{xPosition},{yPosition},null,'mm_link_{menuID}');"" onmouseout=""MM_startTimeout();"">{innerHtml}</span>";

                var menuBuilder = new StringBuilder();
                var level2MenuBuilder = new StringBuilder();


                foreach (NodeInfo theNodeInfo in nodeInfoArrayList)
                {
                    var isLevel2Exist = false;
                    var level2MenuID = 0;

                    if (isShowChildren)
                    {
                        var level2NodeInfoArrayList = new ArrayList();

                        var level2NodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(theNodeInfo, EScopeType.Children, groupChannel, groupChannelNot);
                        if (level2NodeIdList != null && level2NodeIdList.Count > 0)
                        {
                            foreach (int level2NodeID in level2NodeIdList)
                            {
                                var level2NodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, level2NodeID);
                                level2NodeInfoArrayList.Add(level2NodeInfo);
                            }

                            if (level2NodeInfoArrayList.Count > 0)
                            {
                                isLevel2Exist = true;
                                var level2ChildMenuBuilder = new StringBuilder();
                                level2MenuID = pageInfo.UniqueId;

                                foreach (NodeInfo level2NodeInfo in level2NodeInfoArrayList)
                                {
                                    var level2NodeUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, level2NodeInfo);
                                    if (PageUtils.UnclickedUrl.Equals(level2NodeUrl))
                                    {
                                        level2ChildMenuBuilder.Append(
                                            $@"  mm_menu_{level2MenuID}.addMenuItem(""{level2NodeInfo.NodeName.Trim()}"", ""location='{level2NodeUrl}'"");");
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(target))
                                        {
                                            if (target.ToLower().Equals("_blank"))
                                            {
                                                level2ChildMenuBuilder.Append(
                                                    $@"  mm_menu_{level2MenuID}.addMenuItem(""{level2NodeInfo.NodeName
                                                        .Trim()}"", ""window.open('{level2NodeUrl}', '_blank');"");");
                                            }
                                            else
                                            {
                                                level2ChildMenuBuilder.Append(
                                                    $@"  mm_menu_{level2MenuID}.addMenuItem(""{level2NodeInfo.NodeName
                                                        .Trim()}"", ""location='{level2NodeUrl}', '{target}'"");");
                                            }
                                        }
                                        else
                                        {
                                            level2ChildMenuBuilder.Append(
                                                $@"  mm_menu_{level2MenuID}.addMenuItem(""{level2NodeInfo.NodeName.Trim()}"", ""location='{level2NodeUrl}'"");");
                                        }
                                    }
                                }

                                string level2CreateMenuString = $@"
  window.mm_menu_{level2MenuID} = new Menu('{theNodeInfo.NodeName.Trim()}',{childMenuWidth},{childMenuHeight},'{level2MenuDisplayInfo
                                    .FontFamily}',{level2MenuDisplayInfo.FontSize},'{level2MenuDisplayInfo.FontColor}','{level2MenuDisplayInfo
                                    .FontColorHilite}','{level2MenuDisplayInfo.MenuItemBgColor}','{level2MenuDisplayInfo
                                    .MenuHiliteBgColor}','{level2MenuDisplayInfo.MenuItemHAlign}','{level2MenuDisplayInfo
                                    .MenuItemVAlign}',{level2MenuDisplayInfo.MenuItemPadding},{level2MenuDisplayInfo
                                    .MenuItemSpacing},{level2MenuDisplayInfo.HideTimeout},-5,7,true,{level2MenuDisplayInfo
                                    .MenuBgOpaque},{level2MenuDisplayInfo.Vertical},{level2MenuDisplayInfo
                                    .MenuItemIndent},true,true);
  {level2ChildMenuBuilder}
  mm_menu_{level2MenuID}.fontWeight='{level2MenuDisplayInfo.FontWeight}';
  mm_menu_{level2MenuID}.fontStyle='{level2MenuDisplayInfo.FontStyle}';
  mm_menu_{level2MenuID}.hideOnMouseOut={level2MenuDisplayInfo.HideOnMouseOut};
  mm_menu_{level2MenuID}.bgColor='{level2MenuDisplayInfo.BgColor}';
  mm_menu_{level2MenuID}.menuBorder={level2MenuDisplayInfo.MenuBorder};
  mm_menu_{level2MenuID}.menuLiteBgColor='{level2MenuDisplayInfo.MenuLiteBgColor}';
  mm_menu_{level2MenuID}.menuBorderBgColor='{level2MenuDisplayInfo.MenuBorderBgColor}';
";
                                level2MenuBuilder.Append(level2CreateMenuString);
                            }
                        }
                    }

                    var menuName = string.Empty;
                    if (isLevel2Exist)
                    {
                        menuName = "mm_menu_" + level2MenuID;
                    }
                    else
                    {
                        menuName = "\"" + theNodeInfo.NodeName.Trim() + "\"";
                    }

                    var nodeUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, theNodeInfo);
                    if (PageUtils.UnclickedUrl.Equals(nodeUrl))
                    {
                        menuBuilder.Append($@"  mm_menu_{menuID}.addMenuItem({menuName}, ""location='{nodeUrl}'"");");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(target))
                        {
                            if (target.ToLower().Equals("_blank"))
                            {
                                menuBuilder.Append(
                                    $@"  mm_menu_{menuID}.addMenuItem({menuName}, ""window.open('{nodeUrl}', '_blank');"");");
                            }
                            else
                            {
                                menuBuilder.Append(
                                    $@"  mm_menu_{menuID}.addMenuItem({menuName}, ""location='{nodeUrl}', '{target}'"");");
                            }
                        }
                        else
                        {
                            menuBuilder.Append($@"  mm_menu_{menuID}.addMenuItem({menuName}, ""location='{nodeUrl}'"");");
                        }
                    }

                }

                var childMenuIcon = string.Empty;
                if (!string.IsNullOrEmpty(menuDisplayInfo.ChildMenuIcon))
                {
                    childMenuIcon = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, menuDisplayInfo.ChildMenuIcon);
                }
                createMenuString += $@"
  if (window.mm_menu_{menuID}) return;
  {level2MenuBuilder}
  window.mm_menu_{menuID} = new Menu('root',{menuWidth},{menuHeight},'{menuDisplayInfo.FontFamily}',{menuDisplayInfo
                    .FontSize},'{menuDisplayInfo.FontColor}','{menuDisplayInfo.FontColorHilite}','{menuDisplayInfo
                    .MenuItemBgColor}','{menuDisplayInfo.MenuHiliteBgColor}','{menuDisplayInfo.MenuItemHAlign}','{menuDisplayInfo
                    .MenuItemVAlign}',{menuDisplayInfo.MenuItemPadding},{menuDisplayInfo.MenuItemSpacing},{menuDisplayInfo
                    .HideTimeout},-5,7,true,{menuDisplayInfo.MenuBgOpaque},{menuDisplayInfo.Vertical},{menuDisplayInfo
                    .MenuItemIndent},true,true);
  {menuBuilder}
  mm_menu_{menuID}.fontWeight='{menuDisplayInfo.FontWeight}';
  mm_menu_{menuID}.fontStyle='{menuDisplayInfo.FontStyle}';
  mm_menu_{menuID}.hideOnMouseOut={menuDisplayInfo.HideOnMouseOut};
  mm_menu_{menuID}.bgColor='{menuDisplayInfo.BgColor}';
  mm_menu_{menuID}.menuBorder={menuDisplayInfo.MenuBorder};
  mm_menu_{menuID}.menuLiteBgColor='{menuDisplayInfo.MenuLiteBgColor}';
  mm_menu_{menuID}.menuBorderBgColor='{menuDisplayInfo.MenuBorderBgColor}';
  mm_menu_{menuID}.childMenuIcon = ""{childMenuIcon}"";

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
}}
</script>
<script language=""JavaScript1.2"">siteserverLoadMenus();writeMenus();</script>";
                //取得已经保存的js
                var existScript = string.Empty;
                if (pageInfo.IsPageScriptsExists(PageInfo.JsAcMenuScripts, true))
                { 
                    existScript = pageInfo.GetPageScripts(PageInfo.JsAcMenuScripts, true);
                }
                if (string.IsNullOrEmpty(existScript) || existScript.IndexOf("//HEAD") < 0)
                { 
                    scriptBuilder.Append(functionHead);
                }
                scriptBuilder.Append(createMenuString);
                //scriptBuilder.Append(writeMenuString);
                if (string.IsNullOrEmpty(existScript) || existScript.IndexOf("//FOOT") < 0)
                { 
                    scriptBuilder.Append(functionFoot);
                }
                if (!string.IsNullOrEmpty(existScript) && existScript.IndexOf("//NEXT") >= 0)
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
