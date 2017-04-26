<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalAdminView" %>
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

  <table class="table table-bordered table-striped">
    <tr>
      <td width="118" height="25"><strong>账号：</strong></td>
      <td><asp:Literal id="ltlUserName" runat="server" /></td>
    </tr>
    <tr>
      <td width="118" height="25"><strong>姓名：</strong></td>
      <td><asp:Literal id="ltlDisplayName" runat="server" /></td>
    </tr>
    <tr>
      <td width="118" height="25"><strong>添加时间：</strong></td>
      <td><asp:Literal id="ltlCreationDate" runat="server" /></td>
    </tr>
    <tr>
      <td height="25"><strong>最后登录时间：</strong></td>
      <td><asp:Literal ID="ltlLastActivityDate" runat="server" /></td>
    </tr>
    <tr>
      <td height="25"><strong>电子邮箱：</strong></td>
      <td><asp:Literal id="ltlEmail" runat="server" /></td>
    </tr>
    <tr>
      <td height="25"><strong>手机号码：</strong></td>
      <td><asp:Literal id="ltlMobile" runat="server" /></td>
    </tr>
    <tr>
      <td height="25"><strong>角色：</strong></td>
      <td><asp:Literal ID="ltlRoles" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
