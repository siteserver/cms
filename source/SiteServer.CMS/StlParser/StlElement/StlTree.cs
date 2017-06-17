using System;
using System.Collections.Generic;
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
    [Stl(Usage = "树状导航", Description = "通过 stl:tree 标签在模板中显示树状导航")]
    public class StlTree
	{
        private StlTree() { }
        public const string ElementName = "stl:tree";

		public const string AttributeChannelIndex = "channelIndex";
		public const string AttributeChannelName = "channelName";
        public const string AttributeUpLevel = "upLevel";
        public const string AttributeTopLevel = "topLevel";
        public const string AttributeGroupChannel = "groupChannel";
        public const string AttributeGroupChannelNot = "groupChannelNot";
        public const string AttributeTitle = "title";
        public const string AttributeIsLoading = "isLoading";
        public const string AttributeIsShowContentNum = "isShowContentNum";
        public const string AttributeIsShowTreeLine = "isShowTreeLine";
        public const string AttributeCurrentFormatString = "currentFormatString";
        public const string AttributeTarget = "target";
        public const string AttributeIsDynamic = "isDynamic";

	    public static SortedList<string, string> AttributeList => new SortedList<string, string>
	    {
	        {AttributeChannelIndex, "栏目索引"},
	        {AttributeChannelName, "栏目名称"},
	        {AttributeUpLevel, "上级栏目的级别"},
	        {AttributeTopLevel, "从首页向下的栏目级别"},
	        {AttributeGroupChannel, "指定显示的栏目组"},
	        {AttributeGroupChannelNot, "指定不显示的栏目组"},
	        {AttributeTitle, "根节点显示名称"},
	        {AttributeIsLoading, "是否AJAX方式即时载入"},
	        {AttributeIsShowContentNum, "是否显示栏目内容数"},
	        {AttributeIsShowTreeLine, "是否显示树状线"},
	        {AttributeCurrentFormatString, "当前项格式化字符串"},
	        {AttributeTarget, "打开窗口目标"},
	        {AttributeIsDynamic, "是否动态显示"}
	    };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfo)
		{
			string parsedContent;
			try
			{
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
                var attributes = new LowerNameValueCollection();

                var ie = node.Attributes?.GetEnumerator();
			    if (ie != null)
			    {
			        while (ie.MoveNext())
			        {
			            var attr = (XmlAttribute) ie.Current;

			            if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelIndex))
			            {
			                channelIndex = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeChannelName))
			            {
			                channelName = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeUpLevel))
			            {
			                upLevel = TranslateUtils.ToInt(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTopLevel))
			            {
			                topLevel = TranslateUtils.ToInt(attr.Value);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannel))
			            {
			                groupChannel = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeGroupChannelNot))
			            {
			                groupChannelNot = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo,
			                    contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeTitle))
			            {
			                title = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsLoading))
			            {
			                isLoading = TranslateUtils.ToBool(attr.Value, false);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsShowContentNum))
			            {
			                isShowContentNum = TranslateUtils.ToBool(attr.Value, false);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsShowTreeLine))
			            {
			                isShowTreeLine = TranslateUtils.ToBool(attr.Value, true);
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeCurrentFormatString))
			            {
			                currentFormatString = attr.Value;
			                if (!StringUtils.Contains(currentFormatString, "{0}"))
			                {
			                    currentFormatString += "{0}";
			                }
			            }
			            else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
			            {
			                isDynamic = TranslateUtils.ToBool(attr.Value);
			            }
			            else
			            {
			                attributes.Set(attr.Name, attr.Value);
			            }
			        }
			    }

			    if (isDynamic)
                {
                    parsedContent = StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo);
                }
                else
                {
                    parsedContent = isLoading ? ParseImplAjax(pageInfo, contextInfo, channelIndex, channelName, upLevel, topLevel, groupChannel, groupChannelNot, title, isShowContentNum, isShowTreeLine, currentFormatString) : ParseImplNotAjax(pageInfo, contextInfo, channelIndex, channelName, upLevel, topLevel, groupChannel, groupChannelNot, title, isShowContentNum, isShowTreeLine, currentFormatString);
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
            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

            var target = "";

            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">");

            var theNodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(channel, EScopeType.All, groupChannel, groupChannelNot);
            var isLastNodeArray = new bool[theNodeIdList.Count];
            var nodeIdArrayList = new List<int>();

            var currentNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, pageInfo.PageNodeId);
            if (currentNodeInfo != null)
            {
                nodeIdArrayList = TranslateUtils.StringCollectionToIntList(currentNodeInfo.ParentsPath);
                nodeIdArrayList.Add(currentNodeInfo.NodeId);
            }

            foreach (int theNodeId in theNodeIdList)
            {
                var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, theNodeId);
                var nodeInfo = new NodeInfo(theNodeInfo);
                if (theNodeId == pageInfo.PublishmentSystemId && !string.IsNullOrEmpty(title))
                {
                    nodeInfo.NodeName = title;
                }
                var isDisplay = nodeIdArrayList.Contains(theNodeId);
                if (!isDisplay)
                {

                    isDisplay = (nodeInfo.ParentId == channelId || nodeIdArrayList.Contains(nodeInfo.ParentId));
                }

                var selected = (theNodeId == channelId);
                if (!selected && nodeIdArrayList.Contains(nodeInfo.NodeId))
                {
                    selected = true;
                }
                var hasChildren = (nodeInfo.ChildrenCount != 0);

                var linkUrl = PageUtility.GetChannelUrl(pageInfo.PublishmentSystemInfo, theNodeInfo);
                var level = theNodeInfo.ParentsCount - channel.ParentsCount;
                var item = new StlTreeItemNotAjax(isDisplay, selected, pageInfo, nodeInfo, hasChildren, linkUrl, target, isShowTreeLine, isShowContentNum, isLastNodeArray, currentFormatString, channelId, level);

                htmlBuilder.Append(item.GetTrHtml());
            }

            htmlBuilder.Append("</table>");

            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAgStlTreeNotAjax, StlTreeItemNotAjax.GetNodeTreeScript(pageInfo));

            return htmlBuilder.ToString();
        }

        private class StlTreeItemNotAjax
        {
            private readonly string _treeDirectoryUrl;
            private readonly string _iconFolderUrl;
            private readonly string _iconEmptyUrl;
            private readonly string _iconMinusUrl;
            private readonly string _iconPlusUrl;

            private readonly bool _isDisplay;
            private readonly bool _selected;
            private readonly PageInfo _pageInfo;
            private readonly NodeInfo _nodeInfo;
            private readonly bool _hasChildren;
            private readonly string _linkUrl;
            private readonly string _target;
            private readonly bool _isShowTreeLine;
            private readonly bool _isShowContentNum;
            private readonly bool[] _isLastNodeArray;
            private readonly string _currentFormatString;
            private readonly int _topNodeId;
            private readonly int _level;

            public StlTreeItemNotAjax(bool isDisplay, bool selected, PageInfo pageInfo, NodeInfo nodeInfo, bool hasChildren, string linkUrl, string target, bool isShowTreeLine, bool isShowContentNum, bool[] isLastNodeArray, string currentFormatString, int topNodeId, int level)
            {
                _isDisplay = isDisplay;
                _selected = selected;
                _pageInfo = pageInfo;
                _nodeInfo = nodeInfo;
                _hasChildren = hasChildren;
                _linkUrl = linkUrl;
                _target = target;
                _isShowTreeLine = isShowTreeLine;
                _isShowContentNum = isShowContentNum;
                _isLastNodeArray = isLastNodeArray;
                _currentFormatString = currentFormatString;
                _topNodeId = topNodeId;
                _level = level;

                _treeDirectoryUrl = SiteFilesAssets.GetUrl(pageInfo.ApiUrl, "tree");
                _iconFolderUrl = PageUtils.Combine(_treeDirectoryUrl, "folder.gif");
                _iconEmptyUrl = PageUtils.Combine(_treeDirectoryUrl, "empty.gif");
                _iconMinusUrl = PageUtils.Combine(_treeDirectoryUrl, "minus.png");
                _iconPlusUrl = PageUtils.Combine(_treeDirectoryUrl, "plus.png");
            }

            public string GetTrHtml()
            {
                var displayHtml = (_isDisplay) ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
                string trElementHtml = $@"
<tr style='{displayHtml}' treeItemLevel='{_level + 1}'>
	<td nowrap>
		{GetItemHtml()}
	</td>
</tr>
";

                return trElementHtml;
            }

            private string GetItemHtml()
            {
                var htmlBuilder = new StringBuilder();
                if (_isShowTreeLine)
                {
                    if (_topNodeId == _nodeInfo.NodeId)
                    {
                        _nodeInfo.IsLastNode = true;
                    }
                    if (_nodeInfo.IsLastNode == false)
                    {
                        _isLastNodeArray[_level] = false;
                    }
                    else
                    {
                        _isLastNodeArray[_level] = true;
                    }
                    for (var i = 0; i < _level; i++)
                    {
                        htmlBuilder.Append(_isLastNodeArray[i]
                            ? $"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>"
                            : $"<img align=\"absmiddle\" src=\"{_treeDirectoryUrl}/tree_line.gif\"/>");
                    }
                }
                else
                {
                    for (var i = 0; i < _level; i++)
                    {
                        htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                    }
                }

                if (_isDisplay)
                {
                    if (_isShowTreeLine)
                    {
                        if (_nodeInfo.IsLastNode)
                        {
                            if (_hasChildren)
                            {
                                htmlBuilder.Append(
                                    _selected
                                        ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{_treeDirectoryUrl}/tree_minusbottom.gif\"/>"
                                        : $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_treeDirectoryUrl}/tree_plusbottom.gif\"/>");
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{_treeDirectoryUrl}/tree_bottom.gif\"/>");
                            }
                        }
                        else
                        {
                            if (_hasChildren)
                            {
                                htmlBuilder.Append(
                                    _selected
                                        ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{_treeDirectoryUrl}/tree_minusmiddle.gif\"/>"
                                        : $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_treeDirectoryUrl}/tree_plusmiddle.gif\"/>");
                            }
                            else
                            {
                                htmlBuilder.Append(
                                    $"<img align=\"absmiddle\" src=\"{_treeDirectoryUrl}/tree_middle.gif\"/>");
                            }
                        }
                    }
                    else
                    {
                        if (_hasChildren)
                        {
                            htmlBuilder.Append(
                                _selected
                                    ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"true\" src=\"{_iconMinusUrl}\"/>"
                                    : $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_iconPlusUrl}\"/>");
                        }
                        else
                        {
                            htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                        }
                    }
                }
                else
                {
                    if (_isShowTreeLine)
                    {
                        if (_nodeInfo.IsLastNode)
                        {
                            htmlBuilder.Append(
                                _hasChildren
                                    ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_treeDirectoryUrl}/tree_plusbottom.gif\"/>"
                                    : $"<img align=\"absmiddle\" src=\"{_treeDirectoryUrl}/tree_bottom.gif\"/>");
                        }
                        else
                        {
                            htmlBuilder.Append(
                                _hasChildren
                                    ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_treeDirectoryUrl}/tree_plusmiddle.gif\"/>"
                                    : $"<img align=\"absmiddle\" src=\"{_treeDirectoryUrl}/tree_middle.gif\"/>");
                        }
                    }
                    else
                    {
                        htmlBuilder.Append(
                            _hasChildren
                                ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isOpen=\"false\" src=\"{_iconPlusUrl}\"/>"
                                : $"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                    }
                }

                if (!string.IsNullOrEmpty(_iconFolderUrl))
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconFolderUrl}\"/>");
                }

                htmlBuilder.Append("&nbsp;");

                var nodeName = _nodeInfo.NodeName;
                if ((_pageInfo.TemplateInfo.TemplateType == ETemplateType.ChannelTemplate || _pageInfo.TemplateInfo.TemplateType == ETemplateType.ContentTemplate) && _pageInfo.PageNodeId == _nodeInfo.NodeId)
                {
                    nodeName = string.Format(_currentFormatString, nodeName);
                }

                if (!string.IsNullOrEmpty(_linkUrl))
                {
                    var targetHtml = (string.IsNullOrEmpty(_target)) ? string.Empty : $"target='{_target}'";
                    var clickChangeHtml = "onclick='stltree_openFolderByA(this);'";

                    htmlBuilder.Append(
                        $"<a href='{_linkUrl}' {targetHtml} {clickChangeHtml} isTreeLink='true'>{nodeName}</a>");
                }
                else
                {
                    htmlBuilder.Append(nodeName);
                }

                if (_isShowContentNum && _nodeInfo.ContentNum >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append($"<span style=\"font-size:8pt;font-family:arial\">({_nodeInfo.ContentNum})</span>");
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

            var channelId = StlDataUtility.GetNodeIdByLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId, upLevel, topLevel);

            channelId = StlCacheManager.NodeId.GetNodeIdByChannelIdOrChannelIndexOrChannelName(pageInfo.PublishmentSystemId, channelId, channelIndex, channelName);

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, channelId);

            var target = "";

            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append(@"<table border=""0"" cellpadding=""0"" cellspacing=""0"" style=""width:100%;"">");

            var theNodeIdList = DataProvider.NodeDao.GetNodeIdListByScopeType(channel, EScopeType.SelfAndChildren, groupChannel, groupChannelNot);

            foreach (int theNodeId in theNodeIdList)
            {
                var theNodeInfo = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, theNodeId);
                var nodeInfo = new NodeInfo(theNodeInfo);
                if (theNodeId == pageInfo.PublishmentSystemId && !string.IsNullOrEmpty(title))
                {
                    nodeInfo.NodeName = title;
                }

                var rowHtml = GetChannelRowHtml(pageInfo.PublishmentSystemInfo, nodeInfo, target, isShowTreeLine, isShowContentNum, currentFormatString, channelId, channel.ParentsCount, pageInfo.PageNodeId);

                htmlBuilder.Append(rowHtml);
            }

            htmlBuilder.Append("</table>");

            pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAgStlTreeAjax, StlTreeItemAjax.GetScript(pageInfo, target, isShowTreeLine, isShowContentNum, currentFormatString, channelId, channel.ParentsCount, pageInfo.PageNodeId));

            return htmlBuilder.ToString();
        }

        public static string GetChannelRowHtml(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string target, bool isShowTreeLine, bool isShowContentNum, string currentFormatString, int topNodeId, int topParantsCount, int currentNodeId)
        {
            var nodeTreeItem = new StlTreeItemAjax(publishmentSystemInfo, nodeInfo, target, isShowContentNum, currentFormatString, topNodeId, topParantsCount, currentNodeId);
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
            private readonly string _iconFolderUrl;
            private readonly string _iconEmptyUrl;
            private readonly string _iconMinusUrl;
            private readonly string _iconPlusUrl;

            private readonly NodeInfo _nodeInfo;
            private readonly bool _hasChildren;
            private readonly string _linkUrl;
            private readonly string _target;
            private readonly bool _isShowContentNum;
            private readonly string _currentFormatString;
            private readonly int _topNodeId;
            private readonly int _level;
            private readonly int _currentNodeId;

            public StlTreeItemAjax(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, string target, bool isShowContentNum, string currentFormatString, int topNodeId, int topParentsCount, int currentNodeId)
            {
                _nodeInfo = nodeInfo;
                _hasChildren = nodeInfo.ChildrenCount != 0;
                _linkUrl = PageUtility.GetChannelUrl(publishmentSystemInfo, nodeInfo);
                _target = target;
                _isShowContentNum = isShowContentNum;
                _currentFormatString = currentFormatString;
                _topNodeId = topNodeId;
                _level = nodeInfo.ParentsCount - topParentsCount;
                _currentNodeId = currentNodeId;

                var treeDirectoryUrl = SiteFilesAssets.GetUrl(publishmentSystemInfo.Additional.ApiUrl, "tree");
                _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
                _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
                _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
                _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
            }

            public string GetItemHtml()
            {
                var htmlBuilder = new StringBuilder();

                for (var i = 0; i < _level; i++)
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                }

                if (_hasChildren)
                {
                    htmlBuilder.Append(
                        _topNodeId != _nodeInfo.NodeId
                            ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isAjax=\"true\" isOpen=\"false\" id=\"{_nodeInfo.NodeId}\" src=\"{_iconPlusUrl}\"/>"
                            : $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"stltree_displayChildren(this);\" isAjax=\"false\" isOpen=\"true\" id=\"{_nodeInfo.NodeId}\" src=\"{_iconMinusUrl}\"/>");
                }
                else
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                }

                if (!string.IsNullOrEmpty(_iconFolderUrl))
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconFolderUrl}\"/>");
                }

                htmlBuilder.Append("&nbsp;");

                var nodeName = _nodeInfo.NodeName;
                if (_currentNodeId == _nodeInfo.NodeId)
                {
                    nodeName = string.Format(_currentFormatString, nodeName);
                }

                if (!string.IsNullOrEmpty(_linkUrl))
                {
                    var targetHtml = (string.IsNullOrEmpty(_target)) ? string.Empty : $"target='{_target}'";

                    htmlBuilder.Append($"<a href='{_linkUrl}' {targetHtml} isTreeLink='true'>{nodeName}</a>");
                }
                else
                {
                    htmlBuilder.Append(nodeName);
                }

                if (_isShowContentNum && _nodeInfo.ContentNum >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append($"<span style=\"font-size:8pt;font-family:arial\">({_nodeInfo.ContentNum})</span>");
                }

                return htmlBuilder.ToString();
            }

            public static string GetScript(PageInfo pageInfo, string target, bool isShowTreeLine, bool isShowContentNum, string currentFormatString, int topNodeId, int topParentsCount, int currentNodeId)
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
    var pars = 'publishmentSystemID={pageInfo.PublishmentSystemId}&parentID=' + nodeID + '&target={target}&isShowTreeLine={isShowTreeLine}&isShowContentNum={isShowContentNum}&currentFormatString={formatString}&topNodeID={topNodeId}&topParentsCount={topParentsCount}&currentNodeID={currentNodeId}';

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

                script += GetScriptOnLoad(pageInfo.PublishmentSystemId, topNodeId, pageInfo.PageNodeId);

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

            private static string GetScriptOnLoad(int publishmentSystemId, int topNodeId, int currentNodeId)
            {
                if (currentNodeId == 0 || currentNodeId == publishmentSystemId || currentNodeId == topNodeId)
                    return string.Empty;
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, currentNodeId);
                if (nodeInfo != null)
                {
                    string path;
                    if (nodeInfo.ParentId == publishmentSystemId)
                    {
                        path = currentNodeId.ToString();
                    }
                    else
                    {
                        path = nodeInfo.ParentsPath.Substring(nodeInfo.ParentsPath.IndexOf(",", StringComparison.Ordinal) + 1) + "," + currentNodeId;
                    }
                    return $@"
<script language=""JavaScript"">
Event.observe(window,'load', function(){{
    loadingChannelsOnLoad('{path}');
}});
</script>
";
                }
                return string.Empty;
            }
        }
	}
}
