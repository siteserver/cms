<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalNodeGroupAdd" Trace="false"%>
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
      <td width="30%">栏目组名称：</td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="tbNodeGroupName" runat="server" />
        <asp:RequiredFieldValidator controlToValidate="tbNodeGroupName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbNodeGroupName"
            ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="30%">栏目组简介：</td>
      <td><asp:TextBox Columns="35" Rows="4" TextMode="MultiLine" id="tbDescription" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbDescription"
            ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
  </table>

</form>
</body>
</html>
