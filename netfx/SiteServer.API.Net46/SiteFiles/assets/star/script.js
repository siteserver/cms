function stlStarDraw(point, totalNum, updaterID, style, directoryUrl){
	for(var i=1;i<=totalNum;i++){
		if(i<=point){
			document.getElementById("stl_star_item_" + updaterID + "_" + i).src = directoryUrl + "/" + style + "_on.gif"
		}
		else{
			document.getElementById("stl_star_item_" + updaterID + "_" + i).src = directoryUrl + "/" + style + "_off.gif"
		}
	}
}

function stlStarInit(totalNum, updaterID){
	for(var i=1;i<=totalNum;i++){
		document.getElementById("stl_star_item_" + updaterID + "_" + i).src = document.getElementById("stl_star_item_" + updaterID + "_" + i).getAttribute("oriSrc");
	}
}

function stlStarCheck(publishmentSystemID, channelID, contentID, failureMessage)
{
	var allcookies = document.cookie;
	var cookieName = "stlStar_" + publishmentSystemID + "_" + channelID + "_" + contentID;
	var pos = allcookies.indexOf(cookieName + "=");
	if (pos != -1) {
		alert(decodeURIComponent(failureMessage));
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

function stlSuccessAlert(successMessage)
{
	alert(successMessage);
}