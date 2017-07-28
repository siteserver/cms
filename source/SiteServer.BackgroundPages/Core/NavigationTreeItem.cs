using System.Text;
using BaiRong.Core;

namespace SiteServer.BackgroundPages.Core
{
	public class NavigationTreeItem
	{
        private string _iconFolderUrl;
        private string _iconOpenedFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;
		private bool _isDisplay;
		private bool _selected;
		private int _parentsCount;
		private bool _hasChildren;
        private bool _openWindow;
		private string _text = string.Empty;
		private string _linkUrl = string.Empty;
		private string _onClickUrl = string.Empty;
		private string _target = string.Empty;
		private bool _enabled = true;
		private bool _isClickChange;

        public static NavigationTreeItem CreateNodeTreeItem(bool isDisplay, bool selected, int parentsCount, bool hasChildren, bool openWindow, string text, string linkUrl, string onClickUrl, string target, bool enabled, bool isClickChange)
		{
		    var item = new NavigationTreeItem
		    {
		        _isDisplay = isDisplay,
		        _selected = selected,
		        _parentsCount = parentsCount,
		        _hasChildren = hasChildren,
		        _openWindow = openWindow,
		        _text = text,
		        _linkUrl = linkUrl,
		        _onClickUrl = onClickUrl,
		        _target = target,
		        _enabled = enabled,
		        _isClickChange = isClickChange
		    };
		    return item;
		}

        public static NavigationTreeItem CreateNavigationBarItem(bool isDisplay, bool selected, int parentsCount, bool hasChildren, bool openWindow, string text, string linkUrl, string target, bool enabled, string iconUrl)
		{
		    var item = new NavigationTreeItem
		    {
		        _isDisplay = isDisplay,
		        _selected = selected,
		        _parentsCount = parentsCount,
		        _hasChildren = hasChildren,
		        _openWindow = openWindow,
		        _text = text,
		        _linkUrl = linkUrl,
		        _target = target,
		        _enabled = enabled,
		        _isClickChange = true,
		        _iconFolderUrl =
		            !string.IsNullOrEmpty(iconUrl)
		                ? SiteServerAssets.GetIconUrl(iconUrl)
		                : SiteServerAssets.GetIconUrl(hasChildren ? "menu/itemContainer.png" : "menu/item.png")
		    };
		    item._iconOpenedFolderUrl = item._iconFolderUrl;
			return item;
		}

        private NavigationTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            _iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

		public string GetTrHtml()
		{
            var displayHtml = _isDisplay ? StringUtils.Constants.ShowElementStyle : StringUtils.Constants.HideElementStyle;
			string trElementHtml = $@"
<tr style='{displayHtml}' treeItemLevel='{_parentsCount + 1}'>
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
            for (var i = 0; i < _parentsCount; i++)
            {
                htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
            }

            if (_isDisplay)
            {
                if (_hasChildren)
                {
                    htmlBuilder.Append(
                        _selected
                            ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"true\" src=\"{_iconMinusUrl}\"/>"
                            : $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"false\" src=\"{_iconPlusUrl}\"/>");
                }
                else
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
                }
            }
            else
            {
                htmlBuilder.Append(
                    _hasChildren
                        ? $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"false\" src=\"{_iconPlusUrl}\"/>"
                        : $"<img align=\"absmiddle\" src=\"{_iconEmptyUrl}\"/>");
            }
			
			if (!string.IsNullOrEmpty(_iconFolderUrl))
			{
                htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{_iconFolderUrl}\"/>");
			}

			htmlBuilder.Append("&nbsp;");

            if (_enabled)
            {
                if (!string.IsNullOrEmpty(_linkUrl))
                {
                    var targetHtml = string.IsNullOrEmpty(_target) ? string.Empty : $"target='{_target}'";
                    var clickChangeHtml = _isClickChange ? "onclick='openFolderByA(this);'" : string.Empty;

                    htmlBuilder.Append($"<a href='{_linkUrl}' {targetHtml} {clickChangeHtml} isTreeLink='true'>{_text}");

                    htmlBuilder.Append("</a>");

                    if (_openWindow)
                    {
                        htmlBuilder.Append(
                            $@"&nbsp;<a href='{_linkUrl}' target='_blank' title='新窗口打开'><img src=""{SiteServerAssets
                                .GetIconUrl("tree/open.png")}"" /></a>");
                    }
                }
                else if (!string.IsNullOrEmpty(_onClickUrl))
                {
                    htmlBuilder.Append(
                        $@"<a href=""javascript:;"" onClick=""{_onClickUrl}"" title='快速编辑栏目' isTreeLink='true'>{_text}</a>");
                }
                else
                {
                    htmlBuilder.Append(_text);
                }
            }
            else
            {
                htmlBuilder.Append(_text);
            }

			return htmlBuilder.ToString();
		}

		public static string GetNavigationBarScript()
		{
			return GetScript(false);
		}

		public static string GetNodeTreeScript()
		{
			return GetScript(true);
		}

		private static string GetScript(bool isNodeTree)
		{
			var script = @"
<script language=""JavaScript"">
//取得Tree的级别，1为第一级
function getTreeLevel(e) {
	var length = 0;
	if (!isNull(e)){
		if (e.tagName == 'TR') {
			length = parseInt(e.getAttribute('treeItemLevel'));
		}
	}
	return length;
}

function closeAllFolder(element){
    $(element).closest('table').find('td').removeClass('active');
}

function openFolderByA(element){
	closeAllFolder(element);
	if (isNull(element) || element.tagName != 'A') return;

	$(element).parent().addClass('active');

	if (isNodeTree){
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
		if (!isNull(element)){
			element.setAttribute('src', '{iconOpenedFolderUrl}');
		}
	}
}

function getTrElement(element){
	if (isNull(element)) return;
	for (element = element.parentNode;;){
		if (element != null && element.tagName == 'TR'){
			break;
		}else{
			element = element.parentNode;
		} 
	}
	return element;
}

function getImgClickableElementByTr(element){
	if (isNull(element) || element.tagName != 'TR') return;
	var img = null;
	if (!isNull(element.childNodes)){
		var imgCol = element.getElementsByTagName('IMG');
		if (!isNull(imgCol)){
			for (x=0;x<imgCol.length;x++){
				if (!isNull(imgCol.item(x).getAttribute('isOpen'))){
					img = imgCol.item(x);
					break;
				}
			}
		}
	}
	return img;
}

//显示、隐藏下级目录
function displayChildren(element){
	if (isNull(element)) return;

	var tr = getTrElement(element);

	var img = getImgClickableElementByTr(tr);//需要变换的加减图标

	if (!isNull(img) && img.getAttribute('isOpen') != null){
		if (img.getAttribute('isOpen') == 'false'){
			img.setAttribute('isOpen', 'true');
            img.setAttribute('src', '{iconMinusUrl}');
		}else{
            img.setAttribute('isOpen', 'false');
            img.setAttribute('src', '{iconPlusUrl}');
		}
	}

	var level = getTreeLevel(tr);//点击项菜单的级别
	
	var collection = new Array();
	var index = 0;

	for ( var e = tr.nextSibling; !isNull(e) ; e = e.nextSibling) {
		if (!isNull(e) && !isNull(e.tagName) && e.tagName == 'TR'){
		    var currentLevel = getTreeLevel(e);
		    if (currentLevel <= level) break;
		    if(e.style.display == '') {
			    e.style.display = 'none';
		    }else{//展开
			    if (currentLevel != level + 1) continue;
			    e.style.display = '';
			    var imgClickable = getImgClickableElementByTr(e);
			    if (!isNull(imgClickable)){
				    if (!isNull(imgClickable.getAttribute('isOpen')) && imgClickable.getAttribute('isOpen') =='true'){
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
			displayChildren(collection[i]);
		}
	}
}
var isNodeTree = {isNodeTree};
</script>
";
			var item = new NavigationTreeItem();
			script = script.Replace("{iconEmptyUrl}", item._iconEmptyUrl);
			script = script.Replace("{iconFolderUrl}", item._iconFolderUrl);
			script = script.Replace("{iconMinusUrl}", item._iconMinusUrl);
			script = script.Replace("{iconOpenedFolderUrl}", item._iconOpenedFolderUrl);
			script = script.Replace("{iconPlusUrl}", item._iconPlusUrl);
			script = script.Replace("{isNodeTree}", (isNodeTree) ? "true" : "false");
			return script;
		}

	}
}
