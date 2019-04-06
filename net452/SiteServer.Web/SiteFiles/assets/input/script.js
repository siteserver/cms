function stlGetQueryStringValue(sParam)
{   
  var re = eval("/" + sParam + "=([^&]*)/");
  if (re.test(window.location.search)){   
	  return   RegExp.$1   
  }   
  else{   
	  return "";
  }   
}

function UrlDecode(str){ 
	var ret=""; 
	for(var i=0;i<str.length;i++){ 
		var chr = str.charAt(i); 
		if(chr == "+"){ 
			ret+=" "; 
		}else if(chr=="%"){ 
			var asc = str.substring(i+1,i+3); 
			if(parseInt("0x"+asc)>0x7f){ 
				ret+=asc2str(parseInt("0x"+asc+str.substring(i+4,i+6))); 
				i+=5; 
			}else{ 
				ret+=asc2str(parseInt("0x"+asc)); 
				i+=2; 
			} 
		}else{ 
			ret+= chr; 
		} 
	} 
	return ret; 
} 

function stlInputLoadValues(ajaxDivID)
{
	var oForm = document.getElementById(ajaxDivID);
	if (oForm && oForm.elements){
		for (var i=0 ; i < oForm.elements.length; i++) {
			if (oForm.elements[i].name != "")
			{
				var value = stlGetQueryStringValue(oForm.elements[i].name);
				if (value && value.length > 0){
					value = decodeURI(value);
					oForm.elements[i].value = UrlDecode(value);
				}
			}
		}
	}
}