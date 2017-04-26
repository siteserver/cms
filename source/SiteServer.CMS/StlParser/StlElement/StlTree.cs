using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
	public class StlTree
	{
        private StlTree() { }
        public const string ElementName = "stl:tree";//树状导航

		public const string Attribute_ChannelIndex = "channelindex";			        //栏目索引
		public const string Attribute_ChannelName = "channelname";				        //栏目名称
        public const string Attribute_UpLevel = "uplevel";					            //上级栏目的级别
        public const string Attribute_TopLevel = "toplevel";				            //从首页向下的栏目级别

        public const string Attribute_GroupChannel = "groupchannel";		    //指定显示的栏目组
        public const string Attribute_GroupChannelNot = "groupchannelnot";	    //指定不显示的栏目组

        public const string Attribute_Title = "title";							        //根节点显示名称
        public const string Attribute_IsLoading = "isloading";				            //是否AJAX方式动态载入
        public const string Attribute_IsShowContentNum = "isshowcontentnum";            //是否显示栏目内容数
        public const string Attribute_IsShowTreeLine = "isshowtreeline";                //是否显示树状线
        public const string Attribute_CurrentFormatString = "currentformatstring";      //当前项格式化字符串
        public const string Attribute_Target = "target";							        //打开窗口目标
        public const string Attribute_IsDynamic = "isdynamic";              //是否动态显示

		public static ListDictionary AttributeList
		{
			get
			{
				var attributes = new ListDictionary();

				attributes.Add(Attribute_ChannelIndex, "栏目索引");
				attributes.Add(Attribute_ChannelName, "栏目名称");
                attributes.Add(Attribute_UpLevel, "上级栏目的级别");
                attributes.Add(Attribute_TopLevel, "从首页向下的栏目级别");
                attributes.Add(Attribute_GroupChannel, "指定显示的栏目组");
                attributes.Add(Attribute_GroupChannelNot, "指定不显示的栏目组");

                attributes.Add(Attribute_Title, "根节点显示名称");
                attributes.Add(Attribute_IsLoading, "是否AJAX方式即时载入");
                attributes.Add(Attribute_IsShowContentNum, "是否显示栏目内容数");
                attributes.Add(Attribute_IsShowTreeLine, "是否显示树状线");
                attributes.Add(Attribute_CurrentFormatString, "当前项格式化字符串");
                attributes.Add(Attribute_Target, "打开窗口目标");
                attributes.Add(Attribute_IsDynamic, "是否动态显示");
				return attributes;
			}
		}

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			var parsedContent = string.Empty;
			try
			{
                var attributes = new LowerNameValueCollection();
				var ie = node.Attributes.GetEnumerator();

				var channelIndex = string.Empty;
				var channelName = string.Empty;
                var upLevel = 0;
                var topLevel = -1;
                var groupChannel = string.Empty;
                var groupChannelNot = string.Empty;

                var title = string.Empty;
                var isLoading = false;
                var isShowContentNum = false;
                var isShowTreeLine = true;
                var currentFormatString = "<strong>{0}</strong>";
                var isDynamic = false;

				while (ie.MoveNext())
				{
					var attr = (XmlAttribute)ie.Current;
					var attributeName = attr.Name.ToLower();
                    
                    if (attributeName.Equals(Attribute_ChannelIndex))
					{
                        channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
					}
					else if (attributeName.Equals(Attribute_ChannelName))
					{
                        channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
					}
                    else if (attributeName.Equals(Attribute_UpLevel))
                    {
                        upLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_TopLevel))
                    {
                        topLevel = TranslateUtils.ToInt(attr.Value);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannel))
                    {
                        groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_GroupChannelNot))
                    {
                        groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_Title))
                    {
                        title = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                    }
                    else if (attributeName.Equals(Attribute_IsLoading))
                    {
                        isLoading = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else if (attributeName.Equals(Attribute_IsShowContentNum))
                    {
                        isShowContentNum = TranslateUtils.ToBool(attr.Value, false);
                    }
                    else if (attributeName.Equals(Attribute_IsShowTreeLine))
                    {
                        isShowTreeLine = TranslateUtils.ToBool(attr.Value, true);
                    }
                    else if (attributeName.Equals(Attribute_CurrentFormatString))
                    {
                        currentFormatString = attr.Value;
                        if (!StringUtils.Contains(currentFormatString, "{0}"))
                        {
                            currentFormatString += "{0}";
                        }
                    }
                    else if (attributeName.Equals(Attribute_IsDynamic))
                    {
                        isDynamic = TranslateUtils.ToBool(attr.Value);
                    }
					else
					{
                        attributes[attributeName] = attr.Value;
					}
				}

                if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    if (isLoading)
                    {
                        parsedContent = ParseImplAjax(pageInfo, contextInfo, channelIndex, channelName, upLevel, topLevel, groupChannel, groupChannelNot, title, isShowContentNum, isShowTreeLine, currentFormatString);
                    }
                    else
                    {
                        parsedContent = ParseImplNotAjax(pageInfo, contextInfo, channelIndex, channelName, upLevel, topLevel, groupChannel, groupChannelNot, title, isShowContentNum, isShowTreeLine, currentFormatString);
                    }
                }
			}
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

			return parsedContent;
		}

        private static string ParseImplNotAjax(PageInfo pageInfo, ContextInfo contextInfo, string channelIndex, string channelName, int upLevel, int topLevel, string groupChannel, string groupChannelNot, string title, bool isShowContentNum, bool isShowTreeLine, string currentFormatString)
        {
            var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);

            channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

            var target = "";

            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">");

            var theNodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(channel, EScopeType.All, groupChannel, groupChannelNot);
            var isLastNodeArray = new bool[theNodeIdList.Count];
            var nodeIDArrayList = new List<int>();

            var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PageNodeId);
            if (currentNodeInfo != null)
            {
                nodeIDArrayList = TranslateUtils.StringCollectionToIntList(currentNodeInfo.ParentsPath);
                nodeIDArrayList.Add(currentNodeInfo.NodeId);
            }

            foreach (int theNodeID in theNodeIdList)
            {
                var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, theNodeID);
                var nodeInfo = new NodeInfo(theNodeInfo);
                if (theNodeID == pageInfo.PublishmentSystemId && !string.IsNullOrEmpty(title))
                {
                    nodeInfo.NodeName = title;
                }
                var isDisplay = nodeIDArrayList.Contains(theNodeID);
                if (!isDisplay)
                {

                    isDisplay = (nodeInfo.ParentId == channelID || nodeIDArrayList.Contains(nodeInfo.ParentId));
                }

                var selected = (theNodeID == channelID);
                if (!selected && nodeIDArrayList.Contains(nodeInfo.NodeId))
                {
                    selected = true;
                }
                var hasChildren = (nodeInfo.ChildrenCount != 0);

                var linkUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, theNodeInfo);
                var level = theNodeInfo.ParentsCount - channel.ParentsCount;
                var item = new StlTreeItemNotAjax(isDisplay, selected, pageInfo, nodeInfo, hasChildren, linkUrl, target, isShowTreeLine, isShowContentNum, isLastNodeArray, currentFormatString, channelID, level);

                htmlBuilder.Append(item.GetTrHtml());
            }

            htmlBuilder.Append("</table>");

            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAgStlTreeNotAjax, StlTreeItemNotAjax.GetNodeTreeScript(pageInfo));

            return htmlBuilder.ToString();
        }

        private class StlTreeItemNotAjax
        {
            private string treeDirectoryUrl;
            private string iconFolderUrl;
            private string iconOpenedFolderUrl;
            private readonly string iconEmptyUrl;
            private readonly string iconMinusUrl;
            private readonly string iconPlusUrl;

            private bool isDisplay = false;
            private bool selected = false;
            private PageInfo pageInfo;
            private NodeInfo nodeInfo;
            private bool hasChildren = false;
            private string linkUrl = string.Empty;
            private string target = string.Empty;
            private bool isShowTreeLine = true;
            private bool isShowContentNum = false;
            bool[] isLastNodeArray;
            string currentFormatString;
            private int topNodeID = 0;
            private int level = 0;

            private StlTreeItemNotAjax() { }

            public StlTreeItemNotAjax(bool isDisplay, bool selected, PageInfo pageInfo, NodeInfo nodeInfo, bool hasChildren, string linkUrl, string target, bool isShowTreeLine, bool isShowContentNum, bool[] isLastNodeArray, string currentFormatString, int topNodeID, int level)
            {
                this.isDisplay = isDisplay;
                this.selected = selected;
                this.pageInfo = pageInfo;
                this.nodeInfo = nodeInfo;
                this.hasChildren = hasChildren;
                this.linkUrl = linkUrl;
                this.target = target;
                this.isShowTreeLine = isShowTreeLine;
                this.isShowContentNum = isShowContentNum;
                this.isLastNodeArray = isLastNodeArray;
                this.currentFormatString = currentFormatString;
                this.topNodeID = topNodeID;
                this.level = level;

                treeDirectoryUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, "tree");
                iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
                iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
                iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
                iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
                iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
            }

            public string GetTrHtml()
            {
                var displayHtml = (isDisplay) ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
                string trElementHtml = $@"
<tr style='{displayHtml}' treeItemLevel='{level + 1}'>
	<td nowrap>
		{GetItemHtml()}
	</td>
</tr>
";

                return trElementHtml;
            }

            public string GetItemHtml()
            {
                var htmlBuilder = new StringBuilder();
                if (isShowTreeLine)
                {
                    if (topNodeID == nodeInfo.NodeId)
                    {
                        nodeInfo.IsLastNode = true;
                    }
                    if (nodeInfo.IsLastNode == false)
                    {
                        isLastNodeArray[level] = false;
                    }
                    else
                    {
                        isLastNodeArray[level] = true;
                    }
                    for (var i = 0; i < level; i++)
                    {
                        if (isLastNodeArray[i])
                        {
                            htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                        }
                        else
                        {
                            htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_line.gif\"/>");
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < level; i++)
                    {
                        htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                    }
                }

                if (isDisplay)
                {
                    if (isShowTreeLine)
                    {
                        if (nodeInfo.IsLastNode)
                        {
                            if (hasChildren)
                            {
                                if (selected)
                                {
                                    htmlBuilder.Append(
                                        $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{treeDirectoryUrl}/tree_minusbottom.gif\"/>");
                                }
                                else
                                {
                                    htmlBuilder.Append(
                                        $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{treeDirectoryUrl}/tree_plusbottom.gif\"/>");
                                }
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_bottom.gif\"/>");
                            }
                        }
                        else
                        {
                            if (hasChildren)
                            {
                                if (selected)
                                {
                                    htmlBuilder.Append(
                                        $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{treeDirectoryUrl}/tree_minusmiddle.gif\"/>");
                                }
                                else
                                {
                                    htmlBuilder.Append(
                                        $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{treeDirectoryUrl}/tree_plusmiddle.gif\"/>");
                                }
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_middle.gif\"/>");
                            }
                        }
                    }
                    else
                    {
                        if (hasChildren)
                        {
                            if (selected)
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{iconMinusUrl}\"/>");
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{iconPlusUrl}\"/>");
                            }
                        }
                        else
                        {
                            htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                        }
                    }
                }
                else
                {
                    if (isShowTreeLine)
                    {
                        if (nodeInfo.IsLastNode)
                        {
                            if (hasChildren)
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{treeDirectoryUrl}/tree_plusbottom.gif\"/>");
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_bottom.gif\"/>");
                            }
                        }
                        else
                        {
                            if (hasChildren)
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{treeDirectoryUrl}/tree_plusmiddle.gif\"/>");
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{treeDirectoryUrl}/tree_middle.gif\"/>");
                            }
                        }
                    }
                    else
                    {
                        if (hasChildren)
                        {
                            htmlBuilder.Append(
                                $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{iconPlusUrl}\"/>");
                        }
                        else
                        {
                            htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(iconFolderUrl))
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconFolderUrl}\"/>");
                }

                htmlBuilder.Append("&nbsp;");

                var nodeName = nodeInfo.NodeName;
                if ((pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate || pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate) && pageInfo.PageNodeId == nodeInfo.NodeId)
                {
                    nodeName = string.Format(currentFormatString, nodeName);
                }

                if (!string.IsNullOrEmpty(linkUrl))
                {
                    var targetHtml = (string.IsNullOrEmpty(target)) ? string.Empty : $"target='{target}'";
                    var clickChangeHtml = "onclick='stltree_openFolderByA(this);'";

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' {targetHtml} {clickChangeHtml} isTreeLink='true'>{nodeName}</a>");
                }
                else
                {
                    htmlBuilder.Append(nodeName);
                }

                if (isShowContentNum && nodeInfo.ContentNum >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append($"<span style=\"font-size:8pt;font-family:arial\">({nodeInfo.ContentNum})</span>");
                }

                return htmlBuilder.ToString();
            }

            public static string GetNodeTreeScript(PageInfo pageInfo)
            {
                var script = @"
<script language=""JavaScript"">
function stltree_isNull(obj){
	if (typeof(obj) == 'undefined')
	  return true;
	  
	if (obj == undefined)
	  return true;
	  
	if (obj == null)
	  return true;
	 
	return false;
}

//取得Tree的级别，1为第一级
function stltree_getTreeLevel(e) {
	var length = 0;
	if (!stltree_isNull(e)){
		if (e.tagName == 'TR') {
			length = parseInt(e.getAttribute('treeItemLevel'));
		}
	}
	return length;
}

function stltree_closeAllFolder(){
	if (stltree_isNodeTree){
		var imgCol = document.getElementsByTagName('IMG');
		if (!stltree_isNull(imgCol)){
			for (x=0;x<imgCol.length;x++){
				if (!stltree_isNull(imgCol.item(x).getAttribute('src'))){
					if (imgCol.item(x).getAttribute('src') == '{iconOpenedFolderUrl}'){
						imgCol.item(x).setAttribute('src', '{iconFolderUrl}');
					}
				}
			}
		}
	}

	var aCol = document.getElementsByTagName('A');
	if (!stltree_isNull(aCol)){
		for (x=0;x<aCol.length;x++){
			if (aCol.item(x).getAttribute('isTreeLink') == 'true'){
				aCol.item(x).style.fontWeight = 'normal';
			}
		}
	}
}

function stltree_openFolderByA(element){
	stltree_closeAllFolder();
	if (stltree_isNull(element) || element.tagName != 'A') return;

	element.style.fontWeight = 'bold';

	if (stltree_isNodeTree){
		for (element = element.previousSibling;;){
			if (element != null && element.tagName == 'A'){
				element = element.firstChild;
			}
			if (element != null && element.tagName == 'IMG'){
				if (element.getAttribute('src') == '{iconFolderUrl}') break;
				break;
			}else{
				element = element.previousSibling;
			} 
		}
		if (!stltree_isNull(element)){
			element.setAttribute('src', '{iconOpenedFolderUrl}');
		}
	}
}

function stltree_getTrElement(element){
	if (stltree_isNull(element)) return;
	for (element = element.parentNode;;){
		if (element != null && element.tagName == 'TR'){
			break;
		}else{
			element = element.parentNode;
		} 
	}
	return element;
}

function stltree_getImgClickableElementByTr(element){
	if (stltree_isNull(element) || element.tagName != 'TR') return;
	var img = null;
	if (!stltree_isNull(element.childNodes)){
		var imgCol = element.getElementsByTagName('IMG');
		if (!stltree_isNull(imgCol)){
			for (x=0;x<imgCol.length;x++){
				if (!stltree_isNull(imgCol.item(x).getAttribute('isOpen'))){
					img = imgCol.item(x);
					break;
				}
			}
		}
	}
	return img;
}

//显示、隐藏下级目录
function stltree_displayChildren(element){
	if (stltree_isNull(element)) return;

	var tr = stltree_getTrElement(element);

	var img = stltree_getImgClickableElementByTr(tr);//需要变换的加减图标

	if (!stltree_isNull(img) && img.getAttribute('isOpen') != null){
		if (img.getAttribute('isOpen') == 'false'){
			img.setAttribute('isOpen', 'true');
            var iconMinusUrl = img.getAttribute('src').replace('plus','minus');
            img.setAttribute('src', iconMinusUrl);
		}else{
            img.setAttribute('isOpen', 'false');
            var iconPlusUrl = img.getAttribute('src').replace('minus','plus');
            img.setAttribute('src', iconPlusUrl);
		}
	}

	var level = stltree_getTreeLevel(tr);//点击项菜单的级别
	
	var collection = new Array();
	var index = 0;

	for ( var e = tr.nextSibling; !stltree_isNull(e) ; e = e.nextSibling) {
		if (!stltree_isNull(e) && !stltree_isNull(e.tagName) && e.tagName == 'TR'){
		    var currentLevel = stltree_getTreeLevel(e);
		    if (currentLevel <= level) break;
		    if(e.style.display == '') {
			    e.style.display = 'none';
		    }else{//展开
			    if (currentLevel != level + 1) continue;
			    e.style.display = '';
			    var imgClickable = stltree_getImgClickableElementByTr(e);
			    if (!stltree_isNull(imgClickable)){
				    if (!stltree_isNull(imgClickable.getAttribute('isOpen')) && imgClickable.getAttribute('isOpen') =='true'){
					    imgClickable.setAttribute('isOpen', 'false');
                        var iconPlusUrl = imgClickable.getAttribute('src').replace('minus','plus');
                        imgClickable.setAttribute('src', iconPlusUrl);
					    collection[index] = imgClickable;
					    index++;
				    }
			    }
		    }
        }
	}
	
	if (index > 0){
		for (i=0;i<=index;i++){
			stltree_displayChildren(collection[i]);
		}
	}
}
var stltree_isNodeTree = {isNodeTree};
</script>
";
                var treeDirectoryUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, "tree");
                var iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
                var iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
                var iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
                var iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");

                script = script.Replace("{iconFolderUrl}", iconFolderUrl);
                script = script.Replace("{iconMinusUrl}", iconMinusUrl);
                script = script.Replace("{iconOpenedFolderUrl}", iconOpenedFolderUrl);
                script = script.Replace("{iconPlusUrl}", iconPlusUrl);
                script = script.Replace("{isNodeTree}", "true");
                return script;
            }

        }

        private static string ParseImplAjax(PageInfo pageInfo, ContextInfo contextInfo, string channelIndex, string channelName, int upLevel, int topLevel, string groupChannel, string groupChannelNot, string title, bool isShowContentNum, bool isShowTreeLine, string currentFormatString)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.Jquery);

            var channelID = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelID, upLevel, topLevel);

            channelID = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelID, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelID);

            var target = "";

            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">");

            var theNodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(channel, EScopeType.SelfAndChildren, groupChannel, groupChannelNot);
            var nodeIDArrayList = new List<int>();

            var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PageNodeId);
            if (currentNodeInfo != null)
            {
                nodeIDArrayList = TranslateUtils.StringCollectionToIntList(currentNodeInfo.ParentsPath);
                nodeIDArrayList.Add(currentNodeInfo.NodeId);
            }

            foreach (int theNodeID in theNodeIdList)
            {
                var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, theNodeID);
                var nodeInfo = new NodeInfo(theNodeInfo);
                if (theNodeID == pageInfo.PublishmentSystemId && !string.IsNullOrEmpty(title))
                {
                    nodeInfo.NodeName = title;
                }

                var rowHtml = GetChannelRowHtml(pageInfo.PublishmentSystemInfo, nodeInfo, target, isShowTreeLine, isShowContentNum, currentFormatString, channelID, channel.ParentsCount, pageInfo.PageNodeId);

                htmlBuilder.Append(rowHtml);
            }

            htmlBuilder.Append("</table>");

            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAgStlTreeAjax, StlTreeItemAjax.GetScript(pageInfo, target, isShowTreeLine, isShowContentNum, currentFormatString, channelID, channel.ParentsCount, pageInfo.PageNodeId));

            return htmlBuilder.ToString();
        }

        public static string GetChannelRowHtml(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string target, bool isShowTreeLine, bool isShowContentNum, string currentFormatString, int topNodeID, int topParantsCount, int currentNodeID)
        {
            var nodeTreeItem = new StlTreeItemAjax(publishmentSystemInfo, nodeInfo, target, isShowTreeLine, isShowContentNum, currentFormatString, topNodeID, topParantsCount, currentNodeID);
            var title = nodeTreeItem.GetItemHtml();

            string rowHtml = $@"
<tr treeItemLevel=""{nodeInfo.ParentsCount + 1}"">
	<td nowrap>
		{title}
	</td>
</tr>
";

            return rowHtml;
        }

        private class StlTreeItemAjax
        {
            private string treeDirectoryUrl;
            private string iconFolderUrl;
            private string iconOpenedFolderUrl;
            private readonly string iconEmptyUrl;
            private readonly string iconMinusUrl;
            private readonly string iconPlusUrl;

            private PublishmentSystemInfo publishmentSystemInfo;
            private NodeInfo nodeInfo;
            private bool hasChildren = false;
            private string linkUrl = string.Empty;
            private string target = string.Empty;
            private bool isShowTreeLine = true;
            private bool isShowContentNum = false;
            string currentFormatString;
            private int topNodeID = 0;
            private int level = 0;
            private int currentNodeID = 0;

            private StlTreeItemAjax() { }

            public StlTreeItemAjax(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string target, bool isShowTreeLine, bool isShowContentNum, string currentFormatString, int topNodeID, int topParentsCount, int currentNodeID)
            {
                this.publishmentSystemInfo = publishmentSystemInfo;
                this.nodeInfo = nodeInfo;
                hasChildren = (nodeInfo.ChildrenCount != 0);
                linkUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                this.target = target;
                this.isShowTreeLine = isShowTreeLine;
                this.isShowContentNum = isShowContentNum;
                this.currentFormatString = currentFormatString;
                this.topNodeID = topNodeID;
                level = nodeInfo.ParentsCount - topParentsCount;
                this.currentNodeID = currentNodeID;

                treeDirectoryUrl = SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "tree");
                iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
                iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
                iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
                iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
                iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
            }

            public string GetItemHtml()
            {
                var htmlBuilder = new StringBuilder();

                for (var i = 0; i < level; i++)
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                }

                if (hasChildren)
                {
                    if (topNodeID != nodeInfo.NodeId)
                    {
                        htmlBuilder.Append(
                            $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isAjax=\"true\" isOpen=\"false\" id=\"{nodeInfo.NodeId}\" src=\"{iconPlusUrl}\"/>");
                    }
                    else
                    {
                        htmlBuilder.Append(
                            $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isAjax=\"false\" isOpen=\"true\" id=\"{nodeInfo.NodeId}\" src=\"{iconMinusUrl}\"/>");
                    }
                }
                else
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                }

                if (!string.IsNullOrEmpty(iconFolderUrl))
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconFolderUrl}\"/>");
                }

                htmlBuilder.Append("&nbsp;");

                var nodeName = nodeInfo.NodeName;
                if (currentNodeID == nodeInfo.NodeId)
                {
                    nodeName = string.Format(currentFormatString, nodeName);
                }

                if (!string.IsNullOrEmpty(linkUrl))
                {
                    var targetHtml = (string.IsNullOrEmpty(target)) ? string.Empty : $"target='{target}'";

                    htmlBuilder.Append($"<a href='{linkUrl}' {targetHtml} isTreeLink='true'>{nodeName}</a>");
                }
                else
                {
                    htmlBuilder.Append(nodeName);
                }

                if (isShowContentNum && nodeInfo.ContentNum >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append($"<span style=\"font-size:8pt;font-family:arial\">({nodeInfo.ContentNum})</span>");
                }

                return htmlBuilder.ToString();
            }

            public static string GetScript(PageInfo pageInfo, string target, bool isShowTreeLine, bool isShowContentNum, string currentFormatString, int topNodeID, int topParentsCount, int currentNodeID)
            {
                var script = @"
<script language=""JavaScript"">
function stltree_isNull(obj){
	if (typeof(obj) == 'undefined')
	  return true;
	  
	if (obj == undefined)
	  return true;
	  
	if (obj == null)
	  return true;
	 
	return false;
}

function stltree_getTreeLevel(e) {
	var length = 0;
	if (!stltree_isNull(e)){
		if (e.tagName == 'TR') {
			length = parseInt(e.getAttribute('treeItemLevel'));
		}
	}
	return length;
}

function stltree_getTrElement(element){
	if (stltree_isNull(element)) return;
	for (element = element.parentNode;;){
		if (element != null && element.tagName == 'TR'){
			break;
		}else{
			element = element.parentNode;
		} 
	}
	return element;
}

function stltree_getImgClickableElementByTr(element){
	if (stltree_isNull(element) || element.tagName != 'TR') return;
	var img = null;
	if (!stltree_isNull(element.childNodes)){
		var imgCol = element.getElementsByTagName('IMG');
		if (!stltree_isNull(imgCol)){
			for (x=0;x<imgCol.length;x++){
				if (!stltree_isNull(imgCol.item(x).getAttribute('isOpen'))){
					img = imgCol.item(x);
					break;
				}
			}
		}
	}
	return img;
}

var weightedLink = null;

function fontWeightLink(element){
    if (weightedLink != null)
    {
        weightedLink.style.fontWeight = 'normal';
    }
    element.style.fontWeight = 'bold';
    weightedLink = element;
}

var completedNodeID = null;
function stltree_displayChildren(img){
	if (stltree_isNull(img)) return;

	var tr = stltree_getTrElement(img);

    var isToOpen = img.getAttribute('isOpen') == 'false';
    var isByAjax = img.getAttribute('isAjax') == 'true';
    var nodeID = img.getAttribute('id');

	if (!stltree_isNull(img) && img.getAttribute('isOpen') != null){
		if (img.getAttribute('isOpen') == 'false'){
			img.setAttribute('isOpen', 'true');
            img.setAttribute('src', '{iconMinusUrl}');
		}else{
            img.setAttribute('isOpen', 'false');
            img.setAttribute('src', '{iconPlusUrl}');
		}
	}

    if (isToOpen && isByAjax)
    {
        var div = document.createElement('div');
        div.innerHTML = ""<img align='absmiddle' border='0' src='{iconLoadingUrl}' /> 加载中，请稍候..."";
        img.parentNode.appendChild(div);
        //Element.addClassName(div, 'loading');
        loadingChannels(tr, img, div, nodeID);
    }
    else
    {
        var level = stltree_getTreeLevel(tr);
    	
	    var collection = new Array();
	    var index = 0;

	    for ( var e = tr.nextSibling; !stltree_isNull(e) ; e = e.nextSibling) {
		    if (!stltree_isNull(e) && !stltree_isNull(e.tagName) && e.tagName == 'TR'){
		        var currentLevel = stltree_getTreeLevel(e);
		        if (currentLevel <= level) break;
		        if(e.style.display == '') {
			        e.style.display = 'none';
		        }else{
			        if (currentLevel != level + 1) continue;
			        e.style.display = '';
			        var imgClickable = stltree_getImgClickableElementByTr(e);
			        if (!stltree_isNull(imgClickable)){
				        if (!stltree_isNull(imgClickable.getAttribute('isOpen')) && imgClickable.getAttribute('isOpen') =='true'){
					        imgClickable.setAttribute('isOpen', 'false');
                            imgClickable.setAttribute('src', '{iconPlusUrl}');
					        collection[index] = imgClickable;
					        index++;
				        }
			        }
		        }
            }
	    }
    	
	    if (index > 0){
		    for (i=0;i<=index;i++){
			    stltree_displayChildren(collection[i]);
		    }
	    }
    }
}
";
                var loadingUrl = ActionsLoadingChannels.GetUrl(pageInfo.ApiUrl);
                var formatString = TranslateUtils.EncryptStringBySecretKey(currentFormatString);

                script += $@"
function loadingChannels(tr, img, div, nodeID){{
    var url = '{loadingUrl}';
    var pars = 'publishmentSystemID={pageInfo.PublishmentSystemId}&parentID=' + nodeID + '&target={target}&isShowTreeLine={isShowTreeLine}&isShowContentNum={isShowContentNum}&currentFormatString={formatString}&topNodeID={topNodeID}&topParentsCount={topParentsCount}&currentNodeID={currentNodeID}';

    //jQuery.post(url, pars, function(data, textStatus){{
        //$($.parseHTML(data)).insertAfter($(tr));
        //img.setAttribute('isAjax', 'false');
        //img.parentNode.removeChild(div);
        //completedNodeID = nodeID;
    //}});
    $.ajax({{
                url: url,
                type: ""POST"",
                mimeType:""multipart/form-data"",
                contentType: false,
                processData: false,
                cache: false,
                xhrFields: {{   
                    withCredentials: true   
                }},
                data: pars,
                success: function(json, textStatus){{
                    $($.parseHTML(data)).insertAfter($(tr));
                    img.setAttribute('isAjax', 'false');
                    img.parentNode.removeChild(div);
                    completedNodeID = nodeID;
                }}
    }});
}}

function loadingChannelsOnLoad(path){{
    if (path && path.length > 0){{
        var nodeIDs = path.split(',');
        var nodeID = nodeIDs[0];
        var img = $(nodeID);
        if (!img) return;
        if (img.getAttribute('isOpen') == 'false'){{
            stltree_displayChildren(img);
            new PeriodicalExecuter(function(pe){{
                if (completedNodeID && completedNodeID == nodeID){{
                    if (path.indexOf(',') != -1){{
                        var thePath = path.substring(path.indexOf(',') + 1);
                        loadingChannelsOnLoad(thePath);
                    }}
                    pe.stop();
                }} 
            }}, 1);
        }}
    }}
}}
</script>
";

                script += GetScriptOnLoad(pageInfo.PublishmentSystemId, topNodeID, pageInfo.PageNodeId);

                var treeDirectoryUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, "tree");
                var iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
                var iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
                var iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
                var iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");

                script = script.Replace("{iconFolderUrl}", iconFolderUrl);
                script = script.Replace("{iconMinusUrl}", iconMinusUrl);
                script = script.Replace("{iconOpenedFolderUrl}", iconOpenedFolderUrl);
                script = script.Replace("{iconPlusUrl}", iconPlusUrl);
                script = script.Replace("{isNodeTree}", "true");

                script = script.Replace("{iconLoadingUrl}", SiteFilesAssets.GetUrl(pageInfo.ApiUrl, SiteFilesAssets.FileLoading));

                return script;
            }

            public static string GetScriptOnLoad(int publishmentSystemID, int topNodeID, int currentNodeID)
            {
                if (currentNodeID != 0 && currentNodeID != publishmentSystemID && currentNodeID != topNodeID)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemID, currentNodeID);
                    if (nodeInfo != null)
                    {
                        var path = string.Empty;
                        if (nodeInfo.ParentId == publishmentSystemID)
                        {
                            path = currentNodeID.ToString();
                        }
                        else
                        {
                            path = nodeInfo.ParentsPath.Substring(nodeInfo.ParentsPath.IndexOf(",") + 1) + "," + currentNodeID.ToString();
                        }
                        return $@"
<script language=""JavaScript"">
Event.observe(window,'load', function(){{
    loadingChannelsOnLoad('{path}');
}});
</script>
";
                    }
                }
                return string.Empty;
            }
        }
	}
}
