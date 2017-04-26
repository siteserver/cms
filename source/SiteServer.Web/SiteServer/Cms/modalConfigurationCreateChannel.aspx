<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalConfigurationCreateChannel" Trace="false"%>
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
      <td width="220">当内容变动时是否生成本栏目：</td>
      <td><asp:RadioButtonList ID="IsCreateChannelIfContentChanged" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
    </tr>
    <tr>
      <td>当内容变动时需要生成的栏目：</td>
      <td><asp:ListBox ID="NodeIDCollection" SelectionMode="Multiple" Rows="12" runat="server"></asp:ListBox></td>
    </tr>
  </table>

</form>
</body>
</html>
