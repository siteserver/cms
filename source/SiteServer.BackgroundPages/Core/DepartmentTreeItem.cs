using System;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.BackgroundPages.Admin;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Wcm;

namespace SiteServer.BackgroundPages.Core
{
    public enum EDepartmentLoadingType
    {
        AdministratorTree,
        ContentList,
        DepartmentSelect,
        ContentTree,
        GovPublicDepartment,
        List
    }

    public class EDepartmentLoadingTypeUtils
    {
        public static string GetValue(EDepartmentLoadingType type)
        {
            if (type == EDepartmentLoadingType.AdministratorTree)
            {
                return "AdministratorTree";
            }
            else if (type == EDepartmentLoadingType.ContentList)
            {
                return "ContentList";
            }
            else if (type == EDepartmentLoadingType.DepartmentSelect)
            {
                return "DepartmentSelect";
            }
            else if (type == EDepartmentLoadingType.ContentTree)
            {
                return "ContentTree";
            }
            else if (type == EDepartmentLoadingType.GovPublicDepartment)
            {
                return "GovPublicDepartment";
            }
            else if (type == EDepartmentLoadingType.List)
            {
                return "List";
            }           
            else
            {
                throw new Exception();
            }
        }

        public static EDepartmentLoadingType GetEnumType(string typeStr)
        {
            var retval = EDepartmentLoadingType.AdministratorTree;

            if (Equals(EDepartmentLoadingType.AdministratorTree, typeStr))
            {
                retval = EDepartmentLoadingType.AdministratorTree;
            }
            else if (Equals(EDepartmentLoadingType.ContentList, typeStr))
            {
                retval = EDepartmentLoadingType.ContentList;
            }
            else if (Equals(EDepartmentLoadingType.DepartmentSelect, typeStr))
            {
                retval = EDepartmentLoadingType.DepartmentSelect;
            }
            else if (Equals(EDepartmentLoadingType.ContentTree, typeStr))
            {
                retval = EDepartmentLoadingType.ContentTree;
            }
            else if (Equals(EDepartmentLoadingType.GovPublicDepartment, typeStr))
            {
                retval = EDepartmentLoadingType.GovPublicDepartment;
            }
            else if (Equals(EDepartmentLoadingType.List, typeStr))
            {
                retval = EDepartmentLoadingType.List;
            }

            return retval;
        }

        public static bool Equals(EDepartmentLoadingType type, string typeStr)
        {
            if (string.IsNullOrEmpty(typeStr)) return false;
            if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
            {
                return true;
            }
            return false;
        }

        public static bool Equals(string typeStr, EDepartmentLoadingType type)
        {
            return Equals(type, typeStr);
        }
    }

    public class DepartmentTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private DepartmentInfo _departmentInfo;

        public static DepartmentTreeItem CreateInstance(DepartmentInfo departmentInfo)
        {
            var item = new DepartmentTreeItem {_departmentInfo = departmentInfo};

            return item;
        }

        private DepartmentTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(EDepartmentLoadingType loadingType, NameValueCollection additional, bool isOpen)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _departmentInfo.ParentsCount;

            if (loadingType == EDepartmentLoadingType.AdministratorTree || loadingType == EDepartmentLoadingType.DepartmentSelect || loadingType == EDepartmentLoadingType.ContentTree)
            {
                parentsCount = parentsCount + 1;
            }
            
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_departmentInfo.ChildrenCount > 0)
            {
                if (isOpen)
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_departmentInfo
                            .DepartmentId}"" src=""{_iconMinusUrl}"" />");
                }
                else
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_departmentInfo
                            .DepartmentId}"" src=""{_iconPlusUrl}"" />");
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

            if (loadingType == EDepartmentLoadingType.AdministratorTree)
            {
                var linkUrl = PageAdministrator.GetRedirectUrl(_departmentInfo.DepartmentId);

                htmlBuilder.Append(
                    $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='department'>{_departmentInfo.DepartmentName}</a>");
            }
            else if (loadingType == EDepartmentLoadingType.DepartmentSelect)
            {
                var linkUrl = PageUtils.AddQueryString(additional["UrlFormatString"], new NameValueCollection
                {
                    {"DepartmentId", _departmentInfo.DepartmentId.ToString() }
                });

                htmlBuilder.Append($"<a href='{linkUrl}'>{_departmentInfo.DepartmentName}</a>");
            }
            else if (loadingType == EDepartmentLoadingType.ContentTree)
            {
                var linkUrl = PageGovPublicContent.GetRedirectUrl(TranslateUtils.ToInt(additional["PublishmentSystemID"]), _departmentInfo.DepartmentId);

                htmlBuilder.Append(
                    $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_departmentInfo.DepartmentName}</a>");
            }
            else
            {
                htmlBuilder.Append(_departmentInfo.DepartmentName);
            }

            if (loadingType == EDepartmentLoadingType.AdministratorTree)
            {
                if (_departmentInfo.CountOfAdmin >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append(
                        $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({_departmentInfo.CountOfAdmin})</span>");
                }
            }

            htmlBuilder.Replace("displayChildren", "displayChildren_Department");

            return htmlBuilder.ToString();
        }

        public static string GetScript(EDepartmentLoadingType loadingType, NameValueCollection additional)
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
    var url = '{AjaxSystemService.GetLoadingDepartmentsUrl()}';
    var pars = '{AjaxSystemService.GetLoadingDepartmentsParameters(loadingType, additional)}&parentID=' + nodeID;

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

            var item = new DepartmentTreeItem();
            script = script.Replace("{iconEmptyUrl}", item._iconEmptyUrl);
            script = script.Replace("{iconFolderUrl}", item._iconFolderUrl);
            script = script.Replace("{iconMinusUrl}", item._iconMinusUrl);
            script = script.Replace("{iconPlusUrl}", item._iconPlusUrl);

            script = script.Replace("{iconLoadingUrl}", SiteServerAssets.GetIconUrl("loading.gif"));

            script = script.Replace("loadingChannels", "loadingChannels_Department");
            script = script.Replace("displayChildren", "displayChildren_Department");

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
