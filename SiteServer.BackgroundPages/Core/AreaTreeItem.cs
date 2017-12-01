using System;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Ajax;

namespace SiteServer.BackgroundPages.Core
{
    public enum EAreaLoadingType
    {
        Management
    }

    public class EAreaLoadingTypeUtils
    {
        public static string GetValue(EAreaLoadingType type)
        {
            if (type == EAreaLoadingType.Management)
            {
                return "Management";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EAreaLoadingType GetEnumType(string typeStr)
        {
            var retval = EAreaLoadingType.Management;

            if (Equals(EAreaLoadingType.Management, typeStr))
            {
                retval = EAreaLoadingType.Management;
            }

            return retval;
        }

        public static bool Equals(EAreaLoadingType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EAreaLoadingType type)
        {
            return Equals(type, typeStr);
        }
    }

    public class AreaTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private AreaInfo _areaInfo;

        public static AreaTreeItem CreateInstance(AreaInfo areaInfo)
        {
            var item = new AreaTreeItem {_areaInfo = areaInfo};

            return item;
        }

        private AreaTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(EAreaLoadingType loadingType, NameValueCollection additional, bool isOpen)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _areaInfo.ParentsCount;
            
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_areaInfo.ChildrenCount > 0)
            {
                if (isOpen)
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_areaInfo
                            .AreaId}"" src=""{_iconMinusUrl}"" />");
                }
                else
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_areaInfo
                            .AreaId}"" src=""{_iconPlusUrl}"" />");
                }
            }
            else
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (!string.IsNullOrEmpty(_iconFolderUrl))
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconFolderUrl}"" />");
            }

            htmlBuilder.Append("&nbsp;");

            htmlBuilder.Append(_areaInfo.AreaName);

            htmlBuilder.Replace("displayChildren", "displayChildren_Area");

            return htmlBuilder.ToString();
        }

        public static string GetScript(EAreaLoadingType loadingType, NameValueCollection additional)
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
function displayChildren(img){
	if (isNull(img)) return;

	var tr = getTrElement(img);

    var isToOpen = img.getAttribute('isOpen') == 'false';
    var isByAjax = img.getAttribute('isAjax') == 'true';
    var nodeID = img.getAttribute('id');

	if (!isNull(img) && img.getAttribute('isOpen') != null){
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
        $(div).addClass('loading');
        loadingChannels(tr, img, div, nodeID);
    }
    else
    {
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
}
";
            script += $@"
function loadingChannels(tr, img, div, nodeID){{
    var url = '{AjaxSystemService.GetLoadingAreasUrl()}';
    var pars = '{AjaxSystemService.GetLoadingAreasParameters(loadingType, additional)}&parentID=' + nodeID;

    jQuery.post(url, pars, function(data, textStatus)
    {{
        $($.parseHTML(data)).insertAfter($(tr));
        img.setAttribute('isAjax', 'false');
        img.parentNode.removeChild(div);
    }});
    completedNodeID = nodeID;
}}

function loadingChannelsOnLoad(paths){{
    if (paths && paths.length > 0){{
        var nodeIDs = paths.split(',');
        var nodeID = nodeIDs[0];
        var img = $('#' + nodeID);
        if (img.attr('isOpen') == 'false'){{
            displayChildren(img[0]);
//            if (completedNodeID && completedNodeID == nodeID){{
//                if (paths.indexOf(',') != -1){{
//                    setTimeout(""loadingChannelsOnLoad("" + paths + "")"", 3000);
//                }}
//            }} 
        }}
    }}
}}
</script>
";

            var item = new AreaTreeItem();
            script = script.Replace("{iconEmptyUrl}", item._iconEmptyUrl);
            script = script.Replace("{iconFolderUrl}", item._iconFolderUrl);
            script = script.Replace("{iconMinusUrl}", item._iconMinusUrl);
            script = script.Replace("{iconPlusUrl}", item._iconPlusUrl);

            script = script.Replace("{iconLoadingUrl}", SiteServerAssets.GetIconUrl("loading.gif"));

            script = script.Replace("loadingChannels", "loadingChannels_Area");
            script = script.Replace("displayChildren", "displayChildren_Area");

            return script;
        }

        public static string GetScriptOnLoad(string path)
        {
            return $@"
<script language=""JavaScript"">
$(document).ready(function(){{
    loadingChannelsOnLoad('{path}');
}});
</script>
";
        }

    }
}
