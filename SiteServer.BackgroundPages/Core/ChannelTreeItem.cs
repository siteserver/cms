using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SiteServer.BackgroundPages.Ajax;
using SiteServer.BackgroundPages.Cms;
using SiteServer.CMS.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;

namespace SiteServer.BackgroundPages.Core
{
    public class ChannelTreeItem
    {
        private readonly string _contentModelIconClass;
        private readonly string _iconEmptyUrl;
        private readonly string _iconMinusUrl;
        private readonly string _iconPlusUrl;

        private readonly Site _site;
        private readonly Channel _channel;
        private readonly bool _enabled;
        private readonly PermissionsImpl _permissionsImpl;

        public static ChannelTreeItem CreateInstance(Site site, Channel channel, bool enabled, PermissionsImpl permissionsImpl)
        {
            return new ChannelTreeItem(site, channel, enabled, permissionsImpl);
        }

        private ChannelTreeItem(Site site, Channel channel, bool enabled, PermissionsImpl permissionsImpl)
        {
            _site = site;
            _channel = channel;
            _enabled = enabled;
            _permissionsImpl = permissionsImpl;

            var treeDirectoryUrl = SiteServerAssets.GetIconUrl("tree");
            
            //为后台栏目树中的首页和外链栏目添加图标
            if (_channel.ParentId == 0) _contentModelIconClass = "ion-ios-home";
            else if (!string.IsNullOrEmpty(_channel.LinkUrl)) _contentModelIconClass = "ion-link";
            else _contentModelIconClass = "ion-folder";

            _iconEmptyUrl = PageUtils.Combine(treeDirectoryUrl, "empty.gif");
            _iconMinusUrl = PageUtils.Combine(treeDirectoryUrl, "minus.png");
            _iconPlusUrl = PageUtils.Combine(treeDirectoryUrl, "plus.png");
        }

        public async Task<string> GetItemHtmlAsync(ELoadingType loadingType, string returnUrl, NameValueCollection additional)
        {
            var htmlBuilder = new StringBuilder();
            var parentsCount = _channel.ParentsCount;
            for (var i = 0; i < parentsCount; i++)
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            if (_channel.ChildrenCount > 0)
            {
                htmlBuilder.Append(
                    _channel.SiteId == _channel.Id
                        ? $@"<img align=""absmiddle"" style=""cursor:pointer; margin-top: -5px; margin-right: 2px;"" onClick=""event.stopPropagation();displayChildren(this);"" isAjax=""false"" isOpen=""true"" id=""{_channel
                            .Id}"" src=""{_iconMinusUrl}"" />"
                        : $@"<img align=""absmiddle"" style=""cursor:pointer; margin-top: -5px; margin-right: 2px;"" onClick=""event.stopPropagation();displayChildren(this);"" isAjax=""true"" isOpen=""false"" id=""{_channel
                            .Id}"" src=""{_iconPlusUrl}"" />");
            }
            else
            {
                htmlBuilder.Append($@"<img align=""absmiddle"" src=""{_iconEmptyUrl}"" />");
            }

            var contentModelIconHtml = $@"<i class=""{_contentModelIconClass}""></i>";

            if (_channel.Id > 0)
            {
                contentModelIconHtml = $@"<a href=""{PageUtils.GetRedirectUrlToChannel(_channel.SiteId, _channel.Id)}"" target=""_blank"" title=""浏览页面"" onclick=""event.stopPropagation()"">{contentModelIconHtml}</a>";
            }

            htmlBuilder.Append(contentModelIconHtml);
            htmlBuilder.Append("&nbsp;");

            if (_enabled)
            {
                if (loadingType == ELoadingType.ContentTree)
                {
                    var linkUrl = CmsPages.GetContentsUrl(_channel.SiteId, _channel.Id);
                    if (!string.IsNullOrEmpty(additional?["linkUrl"]))
                    {
                        linkUrl = PageUtils.AddQueryStringIfNotExists(additional["linkUrl"], new NameValueCollection
                        {
                            ["channelId"] = _channel.Id.ToString()
                        });
                    }

                    //linkUrl = PageUtils.GetLoadingUrl(linkUrl);

                    htmlBuilder.Append(
                        $"<a href='{linkUrl}' isLink='true' onclick='fontWeightLink(this)' target='content'>{_channel.ChannelName}</a>");
                }
                else if (loadingType == ELoadingType.ChannelClickSelect)
                {
                    var linkUrl = ModalChannelSelect.GetRedirectUrl(_channel.SiteId, _channel.Id);
                    if (additional != null)
                    {
                        if (!string.IsNullOrEmpty(additional["linkUrl"]))
                        {
                            linkUrl = additional["linkUrl"] + _channel.Id;
                        }
                        else
                        {
                            foreach (string key in additional.Keys)
                            {
                                linkUrl += $"&{key}={additional[key]}";
                            }
                        }
                    }
                    htmlBuilder.Append($"<a href='{linkUrl}'>{_channel.ChannelName}</a>");
                }
                else
                {
                    if (await _permissionsImpl.HasChannelPermissionsAsync(_channel.SiteId, _channel.Id, Constants.ChannelPermissions.ChannelEdit))
                    {
                        var onClickUrl = ModalChannelEdit.GetOpenWindowString(_channel.SiteId, _channel.Id, returnUrl);
                        htmlBuilder.Append(
                            $@"<a href=""javascript:;;"" onClick=""{onClickUrl}"" title=""快速编辑栏目"">{_channel.ChannelName}</a>");

                    }
                    else
                    {
                        htmlBuilder.Append($@"<a href=""javascript:;"">{_channel.ChannelName}</a>");
                    }
                }
            }
            else
            {
                htmlBuilder.Append($"<span>{_channel.ChannelName}</span>");
            }

            if (_channel.SiteId != 0)
            {
                htmlBuilder.Append("&nbsp;");

                htmlBuilder.Append(DataProvider.ChannelRepository.GetNodeTreeLastImageHtmlAsync(_site, _channel).GetAwaiter().GetResult());

                var count = DataProvider.ContentRepository.GetCountAsync(_site, _channel).GetAwaiter().GetResult();

                htmlBuilder.Append(
                    $@"<span style=""font-size:8pt;font-family:arial"" class=""gray"">({count})</span>");
            }

            return htmlBuilder.ToString();
        }

        public static string GetScript(Site site, ELoadingType loadingType, string contentModelPluginId, NameValueCollection additional)
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

var activeTrElement = null;
function fontWeightLink(element){
    if (activeTrElement)
    {
        activeTrElement.setAttribute('class', '');
    }
    activeTrElement = getTrElement(element);
    if (activeTrElement) {
        activeTrElement.setAttribute('class', 'table-active');
    }
}

function unSelectRow(tr) {
    tr = $(tr);
    var cb = tr.find('input:checkbox:first');
    if (cb.length  === 0) return;
    var checked = cb.is(':checked');
    cb[0].checked = false;
    tr.removeClass('table-active');
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
        div.innerHTML = ""<img align='absmiddle' width='30' height='12' border='0' src='{iconLoadingUrl}' />"";
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
                    unSelectRow(e);
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
    var pars = '{AjaxOtherService.GetGetLoadingChannelsParameters(site.Id, contentModelPluginId, loadingType, additional)}&parentID=' + channelId;

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
            script = script.Replace("{iconLoadingUrl}", SiteServerAssets.GetUrl("layer/skin/default/xubox_loading0.gif"));
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
