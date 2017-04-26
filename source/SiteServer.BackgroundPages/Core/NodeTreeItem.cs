using System.Text;
using BaiRong.Core;
using SiteServer.CMS.Model;
using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.BackgroundPages.Wcm;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public class NodeTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconOpenedFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private bool _enabled = true;
        private NodeInfo _nodeInfo;
        private string _administratorName;

        public static NodeTreeItem CreateInstance(NodeInfo nodeInfo, bool enabled, string administratorName)
        {
            return new NodeTreeItem
            {
                _enabled = enabled,
                _nodeInfo = nodeInfo,
                _administratorName = administratorName
            };
        }

        private NodeTreeItem()
        {
            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            _iconOpenedFolderUrl = PageUtils.Combine(treeDirectoryUrl, "openedfolder.gif");
            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(ELoadingType loadingType, string returnUrl, NameValueCollection additional)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _nodeInfo.ParentsCount;
            if (loadingType == ELoadingType.GovPublicChannelAdd || loadingType == ELoadingType.GovPublicChannelTree)
            {
                parentsCount = parentsCount - 1;
            }
            else if (loadingType == ELoadingType.GovPublicChannel || loadingType == ELoadingType.GovInteractChannel)
            {
                parentsCount = parentsCount - 2;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_nodeInfo.ChildrenCount > 0)
            {
                if (_nodeInfo.PublishmentSystemId == _nodeInfo.NodeId)
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_nodeInfo.NodeId}"" src=""{_iconMinusUrl}"" />");
                }
                else
                {
                    htmlBuilder.Append(
                        $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_nodeInfo.NodeId}"" src=""{_iconPlusUrl}"" />");
                }
            }
            else
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (!string.IsNullOrEmpty(_iconFolderUrl))
            {
                if (_nodeInfo.NodeId > 0)
                {
                    htmlBuilder.Append(
                        $@"<a href=""{PageActions.GetRedirectUrl(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId)}"" target=""_blank"" title=""浏览页面""><img align=""absmiddle"" border=""0"" src=""{_iconFolderUrl}"" /></a>");
                }
                else
                {
                    htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconFolderUrl}"" />");
                }
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
                else if (loadingType == ELoadingType.GovPublicChannelAdd)
                {
                    if (EContentModelTypeUtils.Equals(_nodeInfo.ContentModelId, EContentModelType.GovPublic))
                    {
                        htmlBuilder.Append($@"<a href=""{ModalGovPublicCategoryChannelSelect.GetRedirectUrl(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId)}"">{_nodeInfo.NodeName}</a>");
                    }
                    else
                    {
                        htmlBuilder.Append(_nodeInfo.NodeName);
                    }
                }
                else if (loadingType == ELoadingType.GovPublicChannelTree)
                {
                    var linkUrl = PageContent.GetRedirectUrl(_nodeInfo.PublishmentSystemId, _nodeInfo.NodeId);

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_nodeInfo.NodeName}</a>");
                }
                else
                {
                    if (AdminUtility.HasChannelPermissions(_administratorName, _nodeInfo.PublishmentSystemId, _nodeInfo.NodeId, AppManager.Cms.Permission.Channel.ChannelEdit))
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
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(_nodeInfo.PublishmentSystemId);

                htmlBuilder.Append("&nbsp;");

                htmlBuilder.Append(NodeManager.GetNodeTreeLastImageHtml(publishmentSystemInfo, _nodeInfo));

                if (_nodeInfo.ContentNum >= 0)
                {
                    htmlBuilder.Append("&nbsp;");
                    htmlBuilder.Append(
                        $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({_nodeInfo.ContentNum})</span>");
                }
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

            var item = new NodeTreeItem();
            script = script.Replace("{iconEmptyUrl}", item._iconEmptyUrl);
            script = script.Replace("{iconFolderUrl}", item._iconFolderUrl);
            script = script.Replace("{iconMinusUrl}", item._iconMinusUrl);
            script = script.Replace("{iconOpenedFolderUrl}", item._iconOpenedFolderUrl);
            script = script.Replace("{iconPlusUrl}", item._iconPlusUrl);

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
