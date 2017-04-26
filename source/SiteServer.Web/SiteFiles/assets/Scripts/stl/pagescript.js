var stl_url_ss = '';
var stl_site_id = '';
var stl_site_url = '';

function stlInit(url_ss, site_id, site_url){
	stl_url_ss = url_ss;
	stl_site_id = site_id;
	stl_site_url = site_url;
}

function stlRefresh() { window.location.reload( false );}

function _random()
{
    var rnd="";
    for(var i=0;i<10;i++)
        rnd+=Math.floor(Math.random()*10);
    return rnd;
}

function getStlInputParameters(ajaxDivID)
{
var ajaxElement = document.getElementById(ajaxDivID);
if(ajaxElement == null) return;

var inputs = ajaxElement.getElementsByTagName("input");
var myhash = new Hash();
if(inputs != null && inputs.length > 0)
{
	for(var i = 0 ;i < inputs.length; i++)
	{
		var name = inputs[i].getAttribute('name');
		if (name != null && name.length > 0)
		{
			var values = '';
			if(inputs[i].type=="radio" || inputs[i].type=="checkbox")
			{
				if (inputs[i].checked)
				{
					if (myhash.get(name) != null)
					{
						values = myhash.get(name);
						values = values + "," + inputs[i].value;
					}
					else
					{
						values = inputs[i].value;
					}
					myhash.set(name, values.replace(/</g,"_lessthan_"));
				}
			}
			else if (inputs[i].type=="file")
			{
				
			}
			else if (inputs[i].type=="hidden")
			{
				var eWebEditor = document.getElementById("eWebEditor_" + name);
				if (eWebEditor)
				{
					values = window.frames("eWebEditor_" + name).window.frames("eWebEditor").document.getElementsByTagName("body")[0].innerHTML;
				}
				else
				{
					values = inputs[i].value;
				}
				myhash.set(name, values.replace(/</g,"_lessthan_"));
			}
			else
			{
				values = inputs[i].value;
				myhash.set(name, values.replace(/</g,"_lessthan_"));
			}
		}
	}
}

var textareas = ajaxElement.getElementsByTagName("textarea");
if(textareas != null && textareas.length > 0)
{
	for(var i = 0 ;i < textareas.length; i++)
	{
		var name = textareas[i].getAttribute('name');
		if (name != null && name.length > 0)
		{
			myhash.set(name, textareas[i].value.replace(/</g,"_lessthan_"));
			try{
				var editor = FCKeditorAPI.GetInstance(name);
				if (editor != undefined)
				{
					myhash.set(name, editor.GetXHTML().replace(/</g,"_lessthan_")); 
				}
			}catch(e){}		
		}
	}
}

var selects = ajaxElement.getElementsByTagName("select");
if(selects != null && selects.length > 0)
{
	for(var i = 0 ;i < selects.length; i++)
	{
		var name = selects[i].getAttribute('name');
		if (name != null && name.length > 0)
		{
			var values = '';
			try{
				for(var j = 0 ;j < selects[i].options.length; j++)
				{
					if (selects[i].options[j].selected)
					{
						if (values != '')
						{
							values = values + "," + selects[i].options[j].value;
						}
						else
						{
							values = selects[i].options[j].value;
						}
					}
				}
				myhash.set(name, values.replace(/</g,"_lessthan_"));
			}catch(e){}		
		}
	}
}

return myhash;
}

function stlInputSubmit(resultsPageUrl, ajaxDivID, isSuccessHide, isSuccessReload, successTemplate, failureTemplate, successCallback, successArgument){
	$("inputSuccess").style.display = 'none';
	$("inputFailure").style.display = 'none';
	$("inputTemplate").style.display = 'none';
	
	if (checkFormValueById(ajaxDivID) == false){
		return;	
	}
	var myhash = getStlInputParameters(ajaxDivID);
	myhash.set('successTemplate', successTemplate);
	myhash.set('failureTemplate', failureTemplate);
	var option = {
		method:'post',
		parameters: myhash,
		evalScripts:true,
		onSuccess: function(transport) {
			var obj = eval('(' + transport.responseText + ')');
			if (obj){
				if (obj.isSuccess == "false"){
					if (obj.template.length > 0){
						$("inputTemplate").style.display = '';
						$("inputTemplate").innerHTML = obj.template;
					}else{
						$("inputFailure").style.display = '';
						$("inputFailure").innerHTML = obj.message;
					}
				}else{
					if (obj.template.length > 0){
						$("inputTemplate").style.display = '';
						$("inputTemplate").innerHTML = obj.template;
					}else{
						$("inputSuccess").style.display = '';
						$("inputSuccess").innerHTML = obj.message;
					}
					if (isSuccessHide){
						$("inputContainer").style.display = 'none';
					}
					if (isSuccessReload){
						setTimeout("stlRefresh()", 2000);
					}else{
						try{
							if (successCallback){
								eval(successCallback + "(" + successArgument + ")");
							}
						}
						catch(e){}
					}
				}
			}
		}
	};
	new Ajax.Request(resultsPageUrl, option);
}

function stlInputReplaceTextarea(attributeName, editorUrl, height, width){
	var oFCKeditor = new FCKeditor(attributeName) ;
	oFCKeditor.BasePath = editorUrl ;
	oFCKeditor.Config['CustomConfigurationsPath'] = editorUrl + '/my.config.js' ;
	oFCKeditor.ToolbarSet = 'MyToolbarSet' ;
	if (height > 0){
		oFCKeditor.Height = height ;
	}else{
		oFCKeditor.Height = 360 ;
	}
	if (width > 0){
		oFCKeditor.Width = width ;
	}else{
		oFCKeditor.Width = 550 ;
	}
	oFCKeditor.ReplaceTextarea();
}

function stlGetQueryString(isWidthAnd)
{ 
	var queryString = document.location.search;
	
	if (queryString == null || queryString.length <= 1) return "";
	if (isWidthAnd){
		return "&" + decodeURI(decodeURI(queryString.substring(1)));
	}else{
		return decodeURI(decodeURI(queryString.substring(1)));
	}
}

function stlGetCrossDomainQueryString(isWidthAnd)
{ 
	var retval = '';
	var queryString = document.location.search;
	
	if (queryString == null || queryString.length <= 1) return "";
	if (isWidthAnd){
		retval = "&" + decodeURI(decodeURI(queryString.substring(1)));
	}else{
		retval = decodeURI(decodeURI(queryString.substring(1)));
	}
	retval = retval.replace('=', '%3d');
	retval = retval.replace('&', '%26');
	return retval;
}

function stlGetQueryStringHash()     
{
	var queryString = document.location.search;
	
	if (queryString == null || queryString.length <= 1) return null;
	
	return $H(queryString.toQueryParams());
}

function stlGetQueryStringValue(sParam)
{   
  var   sBase   =   window.location.search   
  var   re         =   eval("/"   +   sParam   +   "=([^&]*)/")   
  if   (re.test(sBase)){   
	  return   RegExp.$1   
  }   
  else{   
	  return "";
  }   
}

function stlRedirectPage(pageUrl, pageNum)     
{
	var hash = stlGetQueryStringHash();
	if (!hash)
	{
		hash = new Hash();
	}
	hash.set('pageNum', pageNum);
	
	window.location.href = pageUrl + hash.toQueryString();
}

function stlGetXmlDocumentElement(responseText)
{
	var xmlDoc;
	//alert(responseText);
				
	if (document.implementation.createDocument) {
		var parser = new DOMParser();
		xmlDoc = parser.parseFromString(responseText, "text/xml");
	} else if (window.ActiveXObject) {
		xmlDoc = new ActiveXObject("Microsoft.XMLDOM");
		xmlDoc.async = "false";
		xmlDoc.loadXML(responseText);
	}
	return xmlDoc.documentElement;
}

function stlLoadValues(ajaxDivID)
{
	var oForm = $(ajaxDivID);
	if (oForm.elements){
		for (var i=0 ; i < oForm.elements.length; i++) {
			if (oForm.elements[i].name != "")
			{
				var value = stlGetQueryStringValue(oForm.elements[i].name);
				if (value && value.length > 0){
					oForm.elements[i].value = decodeURIComponent(decodeURIComponent(value));
				}
			}
		}
	}
}

function getCookie(name) {
	var search = name + "=";
	var offset = document.cookie.indexOf(search);
	if (offset != -1) {
		offset += search.length;
		var end = document.cookie.indexOf(";", offset);
		if (end == -1) {
			end = document.cookie.length;
		}
		return unescape(document.cookie.substring(offset, end));
	} else{
		return "";
	}	
}

function setCookie(name, value, hours){

	var expireDate=new Date(new Date().getTime()+hours*3600000);

	document.cookie = name + "=" + escape(value) + "; path=/; expires=" + expireDate.toGMTString() ;

}

function delCookie(name) {

	var expireDate=new Date(new Date().getTime());

	document.cookie = name + "= ; path=/; expires=" + expireDate.toGMTString() ;

}

function stlRedirect(redirectUrl, openWin)     
{
	if (openWin){
		window.open(redirectUrl);
	}else{
		window.location.href = redirectUrl;
	}
}

function stlGetTextContent(node)
{
	if(document.all){
		return node.text;   
	} else{
		return node.textContent;
	}
}

function stlGetNodeText(oNode) {
    var sText = "";
    for (var i = 0; i < oNode.childNodes.length; i++) {
       if (oNode.childNodes[i].hasChildNodes()) {
           sText += getText(oNode.childNodes[i]);
       } else {
           sText += oNode.childNodes[i].nodeValue;
       }
    }
    return sText;
}