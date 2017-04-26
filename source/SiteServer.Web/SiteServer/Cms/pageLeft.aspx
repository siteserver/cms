<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageLeft" Trace="False" enableViewState = "false"%>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<style type="text/css">
body { padding:0; margin:0; }
.container {width:153px;}
.dropdown-toggle {width:153px;}
.container,.dropdown,.dropdown-menu {margin-left: 6px; float: none;}
.navbar, .navbar-inner, .nav{margin-bottom: 5px; padding:0; }
.dropdown-menu {width:90%;}
.navbar-inner {height: 35px; min-height: 35px;}
.table-condensed td {padding:2px 5px;}
</style>
<!--[if IE]>
<style type="text/css">
.navbar-inner {height: 40px; min-height: 40px;}
.dropdown {margin-left: 0;}
.table-condensed {margin-top:50px;}
.container {width:100%;}
</style>
<![endif]--> 
<div class="container">
<div class="navbar navbar-fixed-top">
  <div class="navbar-inner">
  	<ul class="nav">
        <li class="dropdown">
            <a href="#" class="dropdown-toggle" data-toggle="dropdown"><asp:Literal ID="ltlTitle" runat="server"/> <b class="caret"></b></a>
            <ul class="dropdown-menu">
                <asp:Literal id="ltlDropdown" runat="server"></asp:Literal>
            </ul>
        </li>
    </ul>
  </div>
</div>
</div>

<form runat="server">
  <table class="table table-condensed noborder table-hover">
    <bairong:NodeNaviTree ID="nodeNaviTree" runat="server" />
  </table>
</form>

</body>
</html>
