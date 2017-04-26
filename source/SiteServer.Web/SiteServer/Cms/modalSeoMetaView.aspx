<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalSeoMetaView" Trace="false"%>
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

  <table class="table table-noborder table-hover">
    <tr>
      <td>页面元数据名称：
        <bairong:NoTagText id="MetaName" runat="server" /></td>
    </tr>
    <tr>
      <td><asp:TextBox Width="400" Rows="8" TextMode="MultiLine" id="MetaCode" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
