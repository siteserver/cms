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

function UrlEncode(str){ 
	var ret=""; 
	var strSpecial="!\"#$%&'()*+,/:;<=>?[]^`{|}~%"; 
	var tt= ""; 

	for(var i=0;i<str.length;i++){ 
		var chr = str.charAt(i); 
		var c=str2asc(chr); 
		tt += chr+":"+c+"n"; 
		if(parseInt("0x"+c) > 0x7f){ 
			ret+="%"+c.slice(0,2)+"%"+c.slice(-2); 
		}else{ 
			if(chr==" ") 
				ret+="+"; 
			else if(strSpecial.indexOf(chr)!=-1) 
				ret+="%"+c.toString(16); 
			else 
				ret+=chr; 
			} 
		} 
	return ret; 
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

function stlSearchLoadValues(ajaxDivID, isUtf8)
{
	var oForm = document.getElementById(ajaxDivID);
	if (oForm && oForm.elements){
		for (var i=0 ; i < oForm.elements.length; i++) {
			if (oForm.elements[i].name != "")
			{
				if (oForm.elements[i].name.toLowerCase() == "channelid")
				{
					var value = stlGetQueryStringValue(oForm.elements[i].name);
					if (value && value.length > 0)
					{
						if (oForm.elements[i].tagName == "SELECT")
						{
							oForm.elements[i].value=value;
						}
						else
						{
							if (document.getElementById("channelID_" + value))
							{
								document.getElementById("channelID_" + value).checked = true;
							}
						}
					}
				}
				else
				{
					var value = stlGetQueryStringValue(oForm.elements[i].name);
					
					if (value && value.length > 0){
						
						if (isUtf8){
							value = decodeURI(value);
						}
						
						oForm.elements[i].value = UrlDecode(value);
					}
				}
			}
		}
	}
}