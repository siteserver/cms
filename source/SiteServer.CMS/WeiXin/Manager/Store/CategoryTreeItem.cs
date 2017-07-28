using BaiRong.Core;
using SiteServer.WeiXin.BackgroundPages;
using SiteServer.WeiXin.Model;
using System;
using System.Collections.Specialized;
using System.Text;

namespace SiteServer.WeiXin.Core
{
    public enum ECategoryLoadingType
    {
        Category,                     //platform/background_Category.aspx
    }

    public class ECategoryLoadingTypeUtils
    {
        public static string GetValue(ECategoryLoadingType type)
        {
            if (type == ECategoryLoadingType.Category)
            {
                return "Category";
            }
            else
            {
                throw new Exception();
            }
        }

        public static ECategoryLoadingType GetEnumType(string typeStr)
        {
            var retval = ECategoryLoadingType.Category;

            if (Equals(ECategoryLoadingType.Category, typeStr))
            {
                retval = ECategoryLoadingType.Category;
            }

            return retval;
        }

        public static bool Equals(ECategoryLoadingType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, ECategoryLoadingType type)
        {
            return Equals(type, typeStr);
        }
    }

    public class CategoryTreeItem
    {
        private string iconFolderUrl;
        private readonly string iconEmptyUrl;
        private readonly string iconMinusUrl;
        private readonly string iconPlusUrl;

        private StoreCategoryInfo categoryInfo;

        public static CategoryTreeItem CreateInstance(StoreCategoryInfo categoryInfo)
        {
            var item = new CategoryTreeItem();
            item.categoryInfo = categoryInfo;

            return item;
        }

        private CategoryTreeItem()
        {
            var treeDirectoryUrl = PageUtils.GetIconUrl("tree");
            iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(ECategoryLoadingType loadingType, NameValueCollection additional, bool isOpen)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = categoryInfo.ParentsCount;

            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.AppendFormat(@"<img align=""absmiddle"" src=""{0}"" />", iconEmptyUrl);
            }

            if (categoryInfo.ChildCount > 0)
            {
                if (isOpen)
                {
                    htmlBuilder.AppendFormat(@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{0}"" src=""{1}"" />", categoryInfo.ID, iconMinusUrl);
                }
                else
                {
                    htmlBuilder.AppendFormat(@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{0}"" src=""{1}"" />", categoryInfo.ID, iconPlusUrl);
                }
            }
            else
            {
                htmlBuilder.AppendFormat(@"<img align=""absmiddle"" src=""{0}"" />", iconEmptyUrl);
            }

            if (!string.IsNullOrEmpty(iconFolderUrl))
            {
                htmlBuilder.AppendFormat(@"<img align=""absmiddle"" src=""{0}"" />", iconFolderUrl);
            }

            htmlBuilder.Append("&nbsp;");

            htmlBuilder.Append(categoryInfo.CategoryName);
            if (categoryInfo.StoreCount > 0)
            {
                htmlBuilder.AppendFormat("&nbsp;({0})", categoryInfo.StoreCount);
            }

            htmlBuilder.Replace("displayChildren", "displayChildren_Category");

            return htmlBuilder.ToString();
        }

        public static string GetScript(int publishmentSystemID, ECategoryLoadingType loadingType, NameValueCollection additional)
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
        div.innerHTML = ""<img align='absmiddle' border='0' src='{iconLoadingUrl}' /> 栏目加载中，请稍候..."";
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
    var url = '{BackgroundService.GetRedirectUrl(publishmentSystemID, BackgroundService.TYPE_GetLoadingCategorys)}';
    var pars = 'parentID=' + nodeID + '&loadingType={ECategoryLoadingTypeUtils.GetValue(loadingType)}&additional={RuntimeUtils
                .EncryptStringByTranslate(TranslateUtils.NameValueCollectionToString(additional))}'; 
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

            var item = new CategoryTreeItem();
            script = script.Replace("{iconEmptyUrl}", item.iconEmptyUrl);
            script = script.Replace("{iconFolderUrl}", item.iconFolderUrl);
            script = script.Replace("{iconMinusUrl}", item.iconMinusUrl);
            script = script.Replace("{iconPlusUrl}", item.iconPlusUrl);

            script = script.Replace("{iconLoadingUrl}", PageUtils.GetIconUrl("loading.gif"));

            script = script.Replace("loadingChannels", "loadingChannels_Category");
            script = script.Replace("displayChildren", "displayChildren_Category");

            return script;
        }

        public static string GetScriptOnLoad(string path)
        {
            return $@"
<script language=""JavaScript"">
$(document).ready(function(){{
    loadingChannels_CategoryOnLoad('{path}');
}});
</script>
";
        }

    }

}
