using System;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Core
{
    public enum EGovPublicCategoryLoadingType
    {
        Tree,
        List,
        Select
    }

    public class EGovPublicCategoryLoadingTypeUtils
    {
        public static string GetValue(EGovPublicCategoryLoadingType type)
        {
            if (type == EGovPublicCategoryLoadingType.Tree)
            {
                return "Tree";
            }
            else if (type == EGovPublicCategoryLoadingType.Select)
            {
                return "Select";
            }
            else if (type == EGovPublicCategoryLoadingType.List)
            {
                return "List";
            }
            else
            {
                throw new Exception();
            }
        }

        public static EGovPublicCategoryLoadingType GetEnumType(string typeStr)
        {
            var retval = EGovPublicCategoryLoadingType.List;

            if (Equals(EGovPublicCategoryLoadingType.Tree, typeStr))
            {
                retval = EGovPublicCategoryLoadingType.Tree;
            }
            else if (Equals(EGovPublicCategoryLoadingType.Select, typeStr))
            {
                retval = EGovPublicCategoryLoadingType.Select;
            }

            return retval;
        }

        public static bool Equals(EGovPublicCategoryLoadingType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EGovPublicCategoryLoadingType type)
        {
            return Equals(type, typeStr);
        }
    }

    public class GovPublicCategoryTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private bool _enabled = true;
        private GovPublicCategoryInfo _categoryInfo;

        public static GovPublicCategoryTreeItem CreateInstance(GovPublicCategoryInfo categoryInfo, bool enabled)
        {
            var item = new GovPublicCategoryTreeItem
            {
                _enabled = enabled,
                _categoryInfo = categoryInfo
            };

            return item;
        }

        private GovPublicCategoryTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(EGovPublicCategoryLoadingType loadingType)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _categoryInfo.ParentsCount;

            if (loadingType == EGovPublicCategoryLoadingType.Tree || loadingType == EGovPublicCategoryLoadingType.Select)
            {
                parentsCount = parentsCount + 1;
            }
            
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_categoryInfo.ChildrenCount > 0)
            {
                htmlBuilder.Append(
                    $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_categoryInfo
                        .CategoryID}"" src=""{_iconPlusUrl}"" />");
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

            if (_enabled)
            {
                if (loadingType == EGovPublicCategoryLoadingType.Tree)
                {
                    var linkUrl = PageGovPublicContent.GetRedirectUrl(_categoryInfo.PublishmentSystemID, _categoryInfo.ClassCode, _categoryInfo.CategoryID);

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_categoryInfo.CategoryName}</a>");

                }
                else if (loadingType == EGovPublicCategoryLoadingType.Select)
                {
                    htmlBuilder.Append($@"<a href=""{ModalGovPublicCategorySelect.GetRedirectUrl(_categoryInfo.PublishmentSystemID, _categoryInfo.ClassCode, _categoryInfo.CategoryID)}"" href=""javascript:;"">{_categoryInfo.CategoryName}</a>");
                }
                else
                {
                    htmlBuilder.Append(_categoryInfo.CategoryName);
                }
            }
            else
            {
                htmlBuilder.Append(_categoryInfo.CategoryName);
            }

            if (_categoryInfo.ContentNum >= 0)
            {
                htmlBuilder.Append("&nbsp;");
                htmlBuilder.Append(
                    $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({_categoryInfo.ContentNum})</span>");
            }

            htmlBuilder.Replace("displayChildren", $"displayChildren_{_categoryInfo.ClassCode}");

            return htmlBuilder.ToString();
        }

        public static string GetScript(string classCode, int publishmentSystemId, EGovPublicCategoryLoadingType loadingType, NameValueCollection additional)
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

var completedClassID = null;
function displayChildren(img){
	if (isNull(img)) return;

	var tr = getTrElement(img);

    var isToOpen = img.getAttribute('isOpen') == 'false';
    var isByAjax = img.getAttribute('isAjax') == 'true';
    var classID = img.getAttribute('id');

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
        loadingChannels(tr, img, div, classID);
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
function loadingChannels(tr, img, div, classID){{
    var url = '{AjaxWcmService.GetLoadingGovPublicCategoriesUrl()}';
    var pars = '{AjaxWcmService.GetLoadingGovPublicCategoriesParameters(publishmentSystemId, classCode, loadingType, additional)}&parentID=' + classID;

    jQuery.post(url, pars, function(data, textStatus)
    {{
        $($.parseHTML(data)).insertAfter($(tr));
        img.setAttribute('isAjax', 'false');
        img.parentNode.removeChild(div);
    }});
    completedClassID = classID;
}}

function loadingChannelsOnLoad(paths){{
    if (paths && paths.length > 0){{
        var nodeIDs = paths.split(',');
        var classID = nodeIDs[0];
        var img = $('#' + classID);
        if (img.attr('isOpen') == 'false'){{
            displayChildren(img[0]);
//            if (completedClassID && completedClassID == classID){{
//                if (paths.indexOf(',') != -1){{
//                    setTimeout(""loadingChannelsOnLoad("" + paths + "")"", 3000);
//                }}
//            }} 
        }}
    }}
}}
</script>
";

            var item = new GovPublicCategoryTreeItem();
            script = script.Replace("{iconEmptyUrl}", item._iconEmptyUrl);
            script = script.Replace("{iconFolderUrl}", item._iconFolderUrl);
            script = script.Replace("{iconMinusUrl}", item._iconMinusUrl);
            script = script.Replace("{iconPlusUrl}", item._iconPlusUrl);

            script = script.Replace("{iconLoadingUrl}", SiteServerAssets.GetIconUrl("loading.gif"));

            script = script.Replace("loadingChannels", $"loadingChannels_{classCode}");
            script = script.Replace("displayChildren", $"displayChildren_{classCode}");

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
