using System.Text;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Core
{
    public class NodeTreeItem
    {
        private readonly string _iconFolderUrl;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private readonly SiteInfo _siteInfo;
        private readonly ChannelInfo _channelInfo;
        private readonly bool _enabled;
        private readonly string _administratorName;

        public static NodeTreeItem CreateInstance(SiteInfo siteInfo, ChannelInfo channelInfo, bool enabled, string administratorName)
        {
            return new NodeTreeItem(siteInfo, channelInfo, enabled, administratorName);
        }

        private NodeTreeItem(SiteInfo siteInfo, ChannelInfo channelInfo, bool enabled, string administratorName)
        {
            _siteInfo = siteInfo;
            _channelInfo = channelInfo;
            _enabled = enabled;
            _administratorName = administratorName;

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");

            _iconFolderUrl = PageUtils.Combine(treeDirectoryUrl, "folder.gif");
            if (!string.IsNullOrEmpty(channelInfo.ContentModelPluginId))
            {
                var iconUrl = PluginManager.GetPluginIconUrl(channelInfo.ContentModelPluginId);
                if (!string.IsNullOrEmpty(iconUrl))
                {
                    _iconFolderUrl = iconUrl;
                }
            }

            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public string GetItemHtml(ELoadingType loadingType, string returnUrl, NameValueCollection additional)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _channelInfo.ParentsCount;
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_channelInfo.ChildrenCount > 0)
            {
                htmlBuilder.Append(
                    _channelInfo.SiteId == _channelInfo.Id
                        ? $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_channelInfo
                            .Id}"" src=""{_iconMinusUrl}"" />"
                        : $@"<img align=""absmiddle"" style=""cursor:pointer"" onClick=""displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_channelInfo
                            .Id}"" src=""{_iconPlusUrl}"" />");
            }
            else
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (!string.IsNullOrEmpty(_iconFolderUrl))
            {
                htmlBuilder.Append(
                    _channelInfo.Id > 0
                        ? $@"<a href=""{PageRedirect.GetRedirectUrlToChannel(_channelInfo.SiteId, _channelInfo.Id)}"" target=""_blank"" title=""浏览页面""><img align=""absmiddle"" border=""0"" src=""{_iconFolderUrl}"" style=""max-height: 22px; max-width: 22px"" /></a>"
                        : $@"<img align=""absmiddle"" src=""{_iconFolderUrl}"" style=""max-height: 22px; max-width: 22px"" />");
            }

            htmlBuilder.Append("&nbsp;");

            if (_enabled)
            {
                if (loadingType == ELoadingType.ContentTree)
                {
                    var linkUrl = PageContent.GetRedirectUrl(_channelInfo.SiteId, _channelInfo.Id);

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_channelInfo.ChannelName}</a>");
                }
                else if (loadingType == ELoadingType.ChannelSelect)
                {
                    var linkUrl = ModalChannelSelect.GetRedirectUrl(_channelInfo.SiteId, _channelInfo.Id);
                    if (additional != null)
                    {
                        if (!string.IsNullOrEmpty(additional["linkUrl"]))
                        {
                            linkUrl = additional["linkUrl"] + _channelInfo.Id;
                        }
                        else
                        {
                            foreach (string key in additional.Keys)
                            {
                                linkUrl += $"&{key}={additional[key]}";
                            }
                        }
                    }
                    htmlBuilder.Append($"<a href='{linkUrl}'>{_channelInfo.ChannelName}</a>");
                }
                else
                {
                    if (AdminUtility.HasChannelPermissions(_administratorName, _channelInfo.SiteId, _channelInfo.Id, ConfigManager.Permissions.Channel.ChannelEdit))
                    {
                        var onClickUrl = ModalChannelEdit.GetOpenWindowString(_channelInfo.SiteId, _channelInfo.Id, returnUrl);
                        htmlBuilder.Append(
                            $@"<a href=""javascript:;;"" onClick=""{onClickUrl}"" title=""快速编辑栏目"">{_channelInfo.ChannelName}</a>");

                    }
                    else
                    {
                        htmlBuilder.Append($@"<a href=""javascript:;;"">{_channelInfo.ChannelName}</a>");
                    }
                }
            }
            else
            {
                htmlBuilder.Append(_channelInfo.ChannelName);
            }

            if (_channelInfo.SiteId != 0)
            {
                htmlBuilder.Append("&nbsp;");

                htmlBuilder.Append(ChannelManager.GetNodeTreeLastImageHtml(_siteInfo, _channelInfo));

                if (_channelInfo.ContentNum < 0) return htmlBuilder.ToString();

                htmlBuilder.Append(
                    $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({_channelInfo.ContentNum})</span>");
            }

            return htmlBuilder.ToString();
        }

        public static string GetScript(SiteInfo siteInfo, ELoadingType loadingType, NameValueCollection additional)
        {
            var script = @"
<script language=""JavaScript"">
function getTreeLevel(e) {
	var length = 0;
	if (e){
		if (e.tagName == 'TR') {
			length = parseInt(e.getAttribute('treeItemLevel'));
		}
	}
	return length;
}

function getTrElement(element){
	if (!element) return;
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
	if (!element || element.tagName != 'TR') return;
	var img = null;
	if (element.childNodes){
		var imgCol = element.getElementsByTagName('IMG');
		if (imgCol){
			for (x=0;x<imgCol.length;x++){
				if (imgCol.item(x).getAttribute('isOpen')){
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

var completedChannelId = null;
function displayChildren(img){
	if (!img) return;

	var tr = getTrElement(img);

    var isToOpen = img.getAttribute('isOpen') == 'false';
    var isByAjax = img.getAttribute('isAjax') == 'true';
    var channelId = img.getAttribute('id');

	if (img && img.getAttribute('isOpen') != null){
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
        loadingChannels(tr, img, div, channelId);
    }
    else
    {
        var level = getTreeLevel(tr);
    	
	    var collection = new Array();
	    var index = 0;

	    for ( var e = tr.nextSibling; e != null ; e = e.nextSibling) {
		    if (e && e.tagName && e.tagName == 'TR'){
		        var currentLevel = getTreeLevel(e);
		        if (currentLevel <= level) break;
		        if(e.style.display == '') {
			        e.style.display = 'none';
		        }else{
			        if (currentLevel != level + 1) continue;
			        e.style.display = '';
			        var imgClickable = getImgClickableElementByTr(e);
			        if (imgClickable){
				        if (imgClickable.getAttribute('isOpen') && imgClickable.getAttribute('isOpen') =='true'){
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
function loadingChannels(tr, img, div, channelId){{
    var url = '{AjaxOtherService.GetGetLoadingChannelsUrl()}';
    var pars = '{AjaxOtherService.GetGetLoadingChannelsParameters(siteInfo.Id, loadingType, additional)}&parentID=' + channelId;

    jQuery.post(url, pars, function(data, textStatus)
    {{
        $($.parseHTML(data)).insertAfter($(tr));
        img.setAttribute('isAjax', 'false');
        img.parentNode.removeChild(div);
    }});
    completedChannelId = channelId;
}}

function loadingChannelsOnLoad(paths){{
    if (paths && paths.length > 0){{
        var channelIds = paths.split(',');
        var channelId = channelIds[0];
        var img = $('#' + channelId);
        if (img.attr('isOpen') == 'false'){{
            displayChildren(img[0]);
            if (completedChannelId && completedChannelId == channelId){{
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
