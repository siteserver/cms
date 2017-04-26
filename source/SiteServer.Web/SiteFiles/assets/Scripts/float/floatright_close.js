var imgheight_close
document.ns = navigator.appName == "Microsoft Internet Explorer"

window.screen.width>800 ? imgheight_close=120:imgheight_close=120
 function myload()
{
myright.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight_close;
myright.style.left=document.body.offsetWidth-120;
mymove();
}
 function mymove()
 {
 myright.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight_close;
 myright.style.left=document.body.scrollLeft+document.body.offsetWidth-120;
 setTimeout("mymove();",50)
 }

function MM_reloadPage(init) {  //reloads the window if Nav4 resized
  if (init==true) with (navigator) {if ((appName=="Netscape")&&(parseInt(appVersion)==4)) {
    document.MM_pgW=innerWidth; document.MM_pgH=innerHeight; onresize=MM_reloadPage; }}
  else if (innerWidth!=document.MM_pgW || innerHeight!=document.MM_pgH) location.reload();
}
MM_reloadPage(true)

function close_float_right(){
	myright.style.visibility='hidden';
}

if(document.ns){
document.write("<div id=myright style='position: absolute;width:80;top:0;left:578;visibility: visible;z-index: 1'>\
<style>\
A.closefloat:link,A.refloat:visited {text-decoration:none;color:#000000;font-size:12px}\
A.closefloat:active,A.refloat:hover {text-decoration:underline;color:#0000FF;font-size:12px}\
</style>\
<table border=0 cellpadding=0 cellspacing=0><tr><td>");

if(ad_float_right_type!="swf")
	document.write("<a href='" + ad_float_right_url + "' target = '_blank'><img src='" + ad_float_right_src + "' WIDTH=80 HEIGHT=80  border = 0></a>");
else
	document.write("<EMBED src='" + ad_float_right_src + "' FlashVars='" + ad_float_right_url + "' quality=high WIDTH=80 HEIGHT=80 TYPE='application/x-shockwave-flash' wmode=opaque></EMBED>");

document.write("</td></tr><tr><td width=80 height=20 align=right><a href='javascript:close_float_right();void(0);'><b><font color=#ff0000>¹Ø±Õ</font></b></a></td></tr>\
</table>\
</div>");

myload()
}