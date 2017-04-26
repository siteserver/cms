<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageLeftSiteSelect" Trace="False"%>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<script language="JavaScript" type="text/javascript">
//取得Tree的级别，1为第一级
function getTreeLevel(e) {
	var length = 0;
	if (e) {
		if (e.tagName == "TR") {
			length = parseInt(e.getAttribute("treeItemLevel"));
		}
	}
	return length;
}

function getTrElement(element) {
    if (isNull(element)) return;
    for (element = element.parentNode; ; ) {
        if (element != null && element.tagName == "TR") {
			break;
		} else {
    		element = element.parentNode;   
		}
	}
	return element;
}

function getImgClickableElementByTr(element){
	if (isNull(element) || element.tagName != "TR" ) return;
	var img = null;
	if (!isNull(element.all)){
		var imgCol = element.all.tags('IMG');
		if (!isNull(imgCol)){
			for (x=0;x<imgCol.length;x++){
				if (!isNull(imgCol.item(x).getAttribute("altSrc"))){
					img = imgCol.item(x);
					break;
				}
			}
		}
	}
	return img;
}

function toggleChildren(element, level) {
   var src = element.src;
   element.setAttribute("src", element.getAttribute("altSrc"));
    element.setAttribute("altSrc", src);
    var tr = getTrElement(element);
    for (var e = tr.nextSibling; !isNull(e) && tr.tagName == "TR"; e = e.nextSibling) {
       
        if (!e.tagName) continue;
        var currentLevel = getTreeLevel(e);
		if (currentLevel <= level) break;
		if (e.style.display == "") {
			e.style.display = "none";
			var img = getImgClickableElementByTr(e);
			if (!isNull(img)){
			    img.setAttribute("src", element.getAttribute("src"));
				img.setAttribute("altSrc", src);
			}
        } else {//展开
			if (currentLevel != level + 1) continue;
			e.style.display = "";
		}
	}
}
</script>
<style type="text/css">
body { padding:0; margin:0; }
.container,.dropdown,.dropdown-menu {margin-left: 6px; float: none;}
.navbar, .navbar-inner, .nav{margin-bottom: 5px; padding:0; }
.dropdown,.dropdown-toggle{width:100%}
.navbar-inner {height: 35px; min-height: 35px;}
</style>
<!--[if IE]>
<style type="text/css">
.navbar-inner {height: 40px; min-height: 40px;}
.dropdown {margin-left: 0;}
</style>
<![endif]--> 
<div class="container" style="height:50px;width:153px;">
<div class="navbar navbar-fixed-top">
  <div class="navbar-inner">
  	<ul class="nav">
	  <li><a href="#">请选择站点进行管理</a></li>
	</ul>
  </div>
</div>
</div>

<table class="table table-condensed noborder table-hover">
	<asp:Literal runat="server" ID="ltlAllSystem"></asp:Literal>
</table>

</body>
</html>
