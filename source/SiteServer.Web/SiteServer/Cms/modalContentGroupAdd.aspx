<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentGroupAdd" Trace="false"%>
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
      <td width="30%"><bairong:help HelpText="此内容组的名称" Text="内容组名称：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="ContentGroupName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="ContentGroupName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="ContentGroupName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="30%"><bairong:help HelpText="此内容组的简介" Text="内容组简介：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="35" Rows="4" TextMode="MultiLine" id="Description" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Description"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
  </table>

</form>
</body>
</html>
