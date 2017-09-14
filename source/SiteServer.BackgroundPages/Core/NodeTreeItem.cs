using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin.Features;

namespace SiteServer.BackgroundPages.Core
{
    public class NodeTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private readonly PublishmentSystemInfo _publishmentSystemInfo;
        private readonly NodeInfo _nodeInfo;
        private readonly bool _enabled;
        private readonly string _administratorName;

        public static NodeTreeItem CreateInstance(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool enabled, string administratorName)
        {
            return new NodeTreeItem(publishmentSystemInfo, nodeInfo, enabled, administratorName);
        }

        private NodeTreeItem(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool enabled, string administratorName)
        {
            _publishmentSystemInfo = publishmentSystemInfo;
            _nodeInfo = nodeInfo;
            _enabled = enabled;
            _administratorName = administratorName;

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            var contentTable = PluginCache.GetEnabledPluginMetadata<IContentTable>(nodeInfo.ContentModelId);
            if (contentTable != null)
            {
                _iconFolderUrl = PageUtils.GetPluginDirectoryUrl(contentTable.Id, contentTable.Icon);
            }

            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(ELoadingType loadingType, string returnUrl, NameValueCollection additional)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _nodeInfo.ParentsCount;
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_nodeInfo.ChildrenCount > 0)
            {
                htmlBuilder.Append(
                    _nodeInfo.PublishmentSystemId == _nodeInfo.NodeId
                        ? $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_nodeInfo
                            .NodeId}"" src=""{_iconMinusUrl}"" />"
                        : $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_nodeInfo
                            .NodeId}"" src=""{_iconPlusUrl}"" />");
            }
            else
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (!string.IsNullOrEmpty(_iconFolderUrl))
            {
                htmlBuilder.Append(
                    _nodeInfo.NodeId > 0
                        ? $@"<a href=""{PageRedirect.GetRedirectUrlToChannel(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId)}"" target=""_blank"" title=""浏览页面""><img align=""absmiddle"" border=""0"" src=""{_iconFolderUrl}"" style=""max-height: 22px; max-width: 22px"" /></a>"
                        : $@"<img align=""absmiddle"" src=""{_iconFolderUrl}"" style=""max-height: 22px; max-width: 22px"" />");
            }

            htmlBuilder.Append("&nbsp;");

            if (_enabled)
            {
                if (loadingType == ELoadingType.ContentTree)
                {
                    var linkUrl = PageContent.GetRedirectUrl(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId);

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_nodeInfo.NodeName}</a>");
                }
                else if (loadingType == ELoadingType.ChannelSelect)
                {
                    var linkUrl = ModalChannelSelect.GetRedirectUrl(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId);
                    if (additional != null)
                    {
                        if (!string.IsNullOrEmpty(additional["linkUrl"]))
                        {
                            linkUrl = additional["linkUrl"] + _nodeInfo.NodeId;
                        }
                        else
                        {
                            foreach (string key in additional.Keys)
                            {
                                linkUrl += $"&{key}={additional[key]}";
                            }
                        }
                    }
                    htmlBuilder.Append($"<a href='{linkUrl}'>{_nodeInfo.NodeName}</a>");
                }
                else
                {
                    if (AdminUtility.HasChannelPermissions(_administratorName, _nodeInfo.PublishmentSystemId, _nodeInfo.NodeId, AppManager.Permissions.Channel.ChannelEdit))
                    {
                        var onClickUrl = ModalChannelEdit.GetOpenWindowString(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId, returnUrl);
                        htmlBuilder.Append(
                            $@"<a href=""javascript:;;"" onClick=""{onClickUrl}"" title=""快速编辑栏目"">{_nodeInfo.NodeName}</a>");

                    }
                    else
                    {
                        htmlBuilder.Append($@"<a href=""javascript:;;"">{_nodeInfo.NodeName}</a>");
                    }
                }
            }
            else
            {
                htmlBuilder.Append(_nodeInfo.NodeName);
            }

            if (_nodeInfo.PublishmentSystemId != 0)
            {
                htmlBuilder.Append("&nbsp;");

                htmlBuilder.Append(NodeManager.GetNodeTreeLastImageHtml(_publishmentSystemInfo, _nodeInfo));

                if (_nodeInfo.ContentNum < 0) return htmlBuilder.ToString();

                htmlBuilder.Append("&nbsp;");
                htmlBuilder.Append(
                    $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({_nodeInfo.ContentNum})</span>");
            }

            return htmlBuilder.ToString();
        }

        public static string GetScript(PublishmentSystemInfo publishmentSystemInfo, ELoadingType loadingType, NameValueCollection additional)
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
    var url = '{AjaxOtherService.GetGetLoadingChannelsUrl()}';
    var pars = '{AjaxOtherService.GetGetLoadingChannelsParameters(publishmentSystemInfo.PublishmentSystemId, loadingType, additional)}&parentID=' + nodeID;

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
            if (completedNodeID && completedNodeID == nodeID){{
                if (paths.indexOf(',') != -1){{
paths = paths.substring(paths.indexOf(',') + 1);
                    setTimeout(""loadingChannelsOnLoad('"" + paths + ""')"", 1000);
                }}
            }} 
        }}
    }}
}}
</script>
";

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            script = script.Replace("{iconEmptyUrl}", PageUtils.Combine(treeDirectoryUrl, "empty.gif"));
            script = script.Replace("{iconMinusUrl}", PageUtils.Combine(treeDirectoryUrl, "minus.png"));
            script = script.Replace("{iconPlusUrl}", PageUtils.Combine(treeDirectoryUrl, "plus.png"));
            script = script.Replace("{iconLoadingUrl}", SiteServerAssets.GetIconUrl("loading.gif"));
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
