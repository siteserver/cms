function SetCookie(sName, sValue, timeKeep)
{
	var now=new Date();
	var expireTime= new Date(now.valueOf()+timeKeep*60000*60);
	document.cookie = sName + "=" + escape(sValue) + "; path=/; expires=" + expireTime.toGMTString() + ";";
}

function GetCookie(sName)
{
	var aCookie = document.cookie.split("; ");
	for (var i=0; i < aCookie.length; i++)
	{
		var aCrumb = aCookie[i].split("=");
		if (sName == aCrumb[0]) 
			return unescape(aCrumb[1]);
	}
	return null;
}

function GetCurrentDateTime()
{
    var date = new Date();
    var current=new String("");
    current += date.getFullYear()+"-";
    current += date.getMonth() + 1+"-";
    current += date.getDate()+" ";
    current += date.getHours()+":";
    current += date.getMinutes()+":";
    current += date.getSeconds();
    return current;
}

function AddTrackerCount(url, siteID){
	try{
		var str_cookie_unique = "tracker_cookie_" + siteID;
		var str_cookie_datetime = "tracker_cookie_datetime_" + siteID;
		
		var str_firstAccessUser;
		if (GetCookie(str_cookie_unique)==null)
		{
			str_firstAccessUser = "True";
			SetCookie(str_cookie_unique,"True",24);
		} else {
			str_firstAccessUser="False";
		}
		
		var str_tracker_lastAccess_datetime = GetCookie(str_cookie_datetime);
		
		SetCookie(str_cookie_datetime,GetCurrentDateTime(),365*24);
		if (str_tracker_lastAccess_datetime==null) {
			str_tracker_lastAccess_datetime = "";
		}

		var pars = '?isFirstAccess=' + str_firstAccessUser + '&location=' + encodeURIComponent(location.href) + '&referrer=' + encodeURIComponent(document.referrer) + '&lastAccessDateTime=' + encodeURIComponent(str_tracker_lastAccess_datetime);
		
		document.write(unescape("%3Cscript src='" + url + pars + "' type='text/javascript'%3E%3C/script%3E"));
	}catch(e){}
}