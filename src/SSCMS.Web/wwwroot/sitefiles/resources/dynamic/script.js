function stlSearchSubmit(ajaxDivID, url, openWin)
{
	var oForm = $(ajaxDivID);
	
	var myhash = new Hash();
	if (oForm.elements){
		for (var i=0 ; i < oForm.elements.length; i++) {
			if (oForm.elements[i].name != "")
			{
				myhash.set(oForm.elements[i].name, encodeURIComponent(oForm.elements[i].value)); 
			}
		}
	}
	
	var searchUrl = url + myhash.toQueryString();
	if (openWin){
		window.open(searchUrl);
	}else{
		window.location.href = searchUrl;	
	}
	
	//stlEncodeFormElements(oForm);
	//oForm.submit();
}

function stlEncodeFormElements(oForm) {
	if (oForm.elements){
		for (var i=0 ; i < oForm.elements.length; i++) {
			if (oForm.elements[i].name != "")
			{
				oForm.elements[i].style.visibility = 'hidden';
				oForm.elements[i].value = encodeURIComponent(oForm.elements[i].value);
			}
		}
	}
}