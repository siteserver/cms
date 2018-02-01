document.ns = navigator.appName == "Microsoft Internet Explorer"

var imgheight
window.screen.width>800 ? imgheight=100:imgheight=100
 function myload()
{
myright.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight;
myright.style.left=document.body.offsetWidth-120;
mymove();
}
 function mymove()
 {
 myright.style.top=document.body.scrollTop+document.body.offsetHeight-imgheight;
 myright.style.left=document.body.scrollLeft+document.body.offsetWidth-120;
 setTimeout("mymove();",50)
 }

function MM_reloadPage(init) {  //reloads the window if Nav4 resized
  if (init==true) with (navigator) {if ((appName=="Netscape")&&(parseInt(appVersion)==4)) {
    document.MM_pgW=innerWidth; document.MM_pgH=innerHeight; onresize=MM_reloadPage; }}
  else if (innerWidth!=document.MM_pgW || innerHeight!=document.MM_pgH) location.reload();
}
MM_reloadPage(true)

if(document.ns){
	
if(ad_float_right_type!="swf")
	document.write("<div id=myright style='position: absolute;width:80;top:0;left:578;visibility: visible;z-index: 1'><a href='" + ad_float_right_url + "' target = '_blank'><img src='" + ad_float_right_src + "' WIDTH=80 HEIGHT=80  border = 0></a></div>");
else
	document.write("<div id=myright style='position: absolute;width:80;top:0;left:578;visibility: visible;z-index: 1'><EMBED src='" + ad_float_right_src + "' quality=high  WIDTH=80 HEIGHT=80 TYPE='application/x-shockwave-flash'></EMBED></div>");
myload()

}