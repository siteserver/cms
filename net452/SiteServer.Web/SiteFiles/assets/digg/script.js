function stlDiggCheck(publishmentSystemID, relatedIdentity)
{
	var allcookies = document.cookie;
	var cookieName = "stlDigg_" + publishmentSystemID + "_" + relatedIdentity;
	var pos = allcookies.indexOf(cookieName + "=");
	if (pos != -1) {
		//对不起，不能重复操作!
		alert(decodeURIComponent("%E5%AF%B9%E4%B8%8D%E8%B5%B7%EF%BC%8C%E4%B8%8D%E8%83%BD%E9%87%8D%E5%A4%8D%E6%93%8D%E4%BD%9C!"));
		return false;
	}else{
		var str = cookieName + "=true";
		var date = new Date();
		var ms = 24*3600*1000;
		date.setTime(date.getTime() + ms);
		str += "; expires=" + date.toGMTString();
		document.cookie = str;
		return true;
	}
}