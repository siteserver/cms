<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalPublishmentSystemSelect" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

<style type="text/css">
.app-div{
  margin: 0 auto;
  width:100%;
  vertical-align:middle;
}
.icon-span{
  text-align: center; 
  padding: 30px;
  height: 100px;
  width: 100px;
}
.icon-span:hover{
  background-color: #eee;
}
.icon-span a{
  font-size: 14px;
}
.icon-span .icon-5 {
  font-size: 4em;
}
.icon-span h5{
  margin-top: 15px;
}
.icon-span .notavaliable{
  opacity: 0.6;
}
</style>

  <div class="app-div">
    <asp:Literal id="ltlHtml" runat="server" />
  </div>

</form>
</body>
</html>
