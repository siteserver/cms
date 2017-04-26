<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTagAdd" Trace="false"%>
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
      <td width="80">标签：</td>
      <td><asp:TextBox TextMode="MultiLine" Columns="50" Rows="5" MaxLength="100" id="tbTags" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbTags" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <br />
        <span class="gray">多个标签请用英文逗号（,）分开 </span></td>
    </tr>
  </table>

</form>
</body>
</html>
