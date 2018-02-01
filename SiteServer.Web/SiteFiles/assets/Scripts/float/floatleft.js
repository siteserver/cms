document.ns = navigator.appName == "Microsoft Internet Explorer"

var imgheight
var imgleft
window.screen.width>800 ? imgheight=100:imgheight=100
window.screen.width>800 ? imgleft=15:imgleft=122
function myload()
{
myleft.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight;
myleft.style.left=imgleft;
leftmove();
}
function leftmove()
 { 
 myleft.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight;
 myleft.style.left=imgleft;
 setTimeout("leftmove();",80)
 }

function MM_reloadPage(init) {  //reloads the window if Nav4 resized
  if (init==true) with (navigator) {if ((appName=="Netscape")&&(parseInt(appVersion)==4)) {
    document.MM_pgW=innerWidth; document.MM_pgH=innerHeight; onresize=MM_reloadPage; }}
  else if (innerWidth!=document.MM_pgW || innerHeight!=document.MM_pgH) location.reload();
}
MM_reloadPage(true)

if(document.ns){
	if(ad_float_left_type!="swf")
		document.write("<div id=myleft style='position: absolute;width:80;top:300;left:5;visibility: visible;z-index: 1'><a href='" + ad_float_left_url + "' target = '_blank'><img src='" + ad_float_left_src + "' WIDTH=80 HEIGHT=80  border = 0></a></div>");
	else
		document.write("<div id=myleft style='position: absolute;width:80;top:300;left:5;visibility: visible;z-index: 1'><EMBED src='" + ad_float_left_src + "' quality=high  WIDTH=80 HEIGHT=80 TYPE='application/x-shockwave-flash' id=changhongout ></EMBED></div>");
	myload()
}