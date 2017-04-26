<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalInnerLinkAdd" Trace="false"%>
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
      <td width="30%"><bairong:help HelpText="需要添加链接的关键字" Text="链接关键字：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="45" id="InnerLinkName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="InnerLinkName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="InnerLinkName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="30%"><bairong:help HelpText="关键字的链接地址" Text="链接地址：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="45" id="LinkUrl" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="LinkUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="LinkUrl"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
        <br>
        <span class="gray">以“~/”开头代表系统根目录，以“@/”开头代表站点根目录</span>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
