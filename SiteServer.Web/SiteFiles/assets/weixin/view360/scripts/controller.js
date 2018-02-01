function hideUrlBar() {
if (((navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i)))) {
container = document.getElementById("container");
if (container) {
var cheight;
switch(window.innerHeight) {
case 208:cheight=268; break; // landscape
case 260:cheight=320; break; // landscape, fullscreen
case 336:cheight=396; break; // portrait, in call status bar
case 356:cheight=416; break; // portrait 
case 424:cheight=484; break; // portrait iPhone5, in call status bar
case 444:cheight=504; break; // portrait iPhone5 
default:
cheight=window.innerHeight;
}
if ((cheight) && ((container.offsetHeight!=cheight) || (window.innerHeight!=cheight))) {
container.style.height=cheight + "px";
setTimeout(function() { hideUrlBar(); }, 1000);
}
}
}

document.getElementsByTagName("body")[0].style.marginTop="1px";
window.scrollTo(0, 1);
}
window.addEventListener("load", hideUrlBar);
window.addEventListener("resize", hideUrlBar);
window.addEventListener("orientationchange", hideUrlBar);

function getUrlVar (key)
{
    var result = new RegExp(key + "=([^&]*)", "i").exec(window.location.search);
    return result && decodeURIComponent(result[1]) || ""; 
}
// create the panorama player with the container
var pano=new pano2vrPlayer("container");
var publishmentSystemID=getUrlVar("publishmentSystemID");
var view360ID=getUrlVar("view360ID");
var url = '/api/wx_view360/GetConfigXML?publishmentSystemID='+publishmentSystemID+'&view360ID='+view360ID;
pano.readConfigUrl(url);
// hide the URL bar on the iPhone
hideUrlBar();