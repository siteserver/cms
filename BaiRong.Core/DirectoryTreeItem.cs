using System.Text;

namespace BaiRong.Core
{
	public class DirectoryTreeItem
	{
        private string iconFolderUrl;
        private string iconOpenedFolderUrl;
        private readonly string iconEmptyUrl;
        private readonly string iconMinusUrl;
        private readonly string iconPlusUrl;

		private bool isDisplay = false;
		private bool selected = false;
		private int parentsCount = 0;
		private bool hasChildren = false;
		private string text = string.Empty;
		private string linkUrl = string.Empty;
		private string onClickUrl = string.Empty;
		private string target = string.Empty;
		private bool enabled = true;
		private bool isClickChange = false;

		public static DirectoryTreeItem CreateDirectoryTreeItem(bool isDisplay, bool selected, int parentsCount, bool hasChildren, string text, string linkUrl, string onClickUrl, string target, bool enabled, bool isClickChange)
		{
            var item = new DirectoryTreeItem();
			item.isDisplay = isDisplay;
			item.selected = selected;
			item.parentsCount = parentsCount;
			item.hasChildren = hasChildren;
			item.text = text;
			item.linkUrl = linkUrl;
			item.onClickUrl = onClickUrl;
			item.target = target;
			item.enabled = enabled;
			item.isClickChange = isClickChange;
			return item;
		}

        private DirectoryTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
            iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

		public string GetTrHtml()
		{
			var displayHtml = (isDisplay) ? "display:" : "display:none";
			string trElementHtml = $@"
<tr style='{displayHtml}' treeItemLevel='{parentsCount + 1}'>
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
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
            }

            if (isDisplay)
            {
                if (hasChildren)
                {
                    if (selected)
                    {
                        htmlBuilder.Append(
                            $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"true\" src=\"{iconMinusUrl}\"/>");
                    }
                    else
                    {
                        htmlBuilder.Append(
                            $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"false\" src=\"{iconPlusUrl}\"/>");
                    }
                }
                else
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                }
            }
            else
            {
                if (hasChildren)
                {
                    htmlBuilder.Append(
                        $"<img align=\"absmiddle\" style=\"cursor:pointer;\" onClick=\"displayChildren(this);\" isOpen=\"false\" src=\"{iconPlusUrl}\"/>");
                }
                else
                {
                    htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconEmptyUrl}\"/>");
                }
            }
			
			if (!string.IsNullOrEmpty(iconFolderUrl))
			{
                htmlBuilder.Append($"<img align=\"absmiddle\" src=\"{iconFolderUrl}\"/>");
			}

			htmlBuilder.Append("&nbsp;");

            if (enabled)
            {
                if (!string.IsNullOrEmpty(linkUrl))
                {
                    var targetHtml = (string.IsNullOrEmpty(target)) ? string.Empty : $"target='{target}'";
                    var clickChangeHtml = (isClickChange) ? "onclick='openFolderByA(this);'" : string.Empty;

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' {targetHtml} {clickChangeHtml} isTreeLink='true'>{text}</a>");
                }
                else if (!string.IsNullOrEmpty(onClickUrl))
                {
                    htmlBuilder.Append(
                        $@"<a href=""javascript:;"" onClick=""{onClickUrl}"" isTreeLink='true'>{text}</a>");
                }
                else
                {
                    htmlBuilder.Append(text);
                }
            }
            else
            {
                htmlBuilder.Append(text);
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

function displayChildren(element){
	if (isNull(element)) return;

	var tr = getTrElement(element);

	var img = getImgClickableElementByTr(tr);

	if (!isNull(img) && img.getAttribute('isOpen') != null){
		if (img.getAttribute('isOpen') == 'false'){
			img.setAttribute('isOpen', 'true');
            img.setAttribute('src', '{iconMinusUrl}');
		}else{
            img.setAttribute('isOpen', 'false');
            img.setAttribute('src', '{iconPlusUrl}');
		}
	}

	var level = getTreeLevel(tr);
	
	var collection = new Array();
	var index = 0;

	for ( var e = tr.nextSibling; !isNull(e) ; e = e.nextSibling) {
		if (!isNull(e) && !isNull(e.tagName) && e.tagName == 'TR'){
		    var currentLevel = getTreeLevel(e);
		    if (currentLevel <= level) break;
		    if(e.style.display == '') {
			    e.style.display = 'none';
		    }else{
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
            var item = new DirectoryTreeItem();
			script = script.Replace("{iconEmptyUrl}", item.iconEmptyUrl);
			script = script.Replace("{iconFolderUrl}", item.iconFolderUrl);
			script = script.Replace("{iconMinusUrl}", item.iconMinusUrl);
			script = script.Replace("{iconOpenedFolderUrl}", item.iconOpenedFolderUrl);
			script = script.Replace("{iconPlusUrl}", item.iconPlusUrl);
			script = script.Replace("{isNodeTree}", (isNodeTree) ? "true" : "false");
			return script;
		}

	}
}
