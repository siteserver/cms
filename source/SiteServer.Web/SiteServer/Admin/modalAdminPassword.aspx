<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalAdminPassword" %>
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
      <td width="130"><bairong:help HelpText="需要重设密码的用户名" Text="用户名：" runat="server" ></bairong:help></td>
      <td><asp:Label id="UserName" runat="server"/></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="输入需要设置的新密码" Text="新密码：" runat="server" ></bairong:help></td>
      <td><asp:TextBox TextMode="Password" id="Password" MaxLength="50" Size="20" runat="server"/>
        <asp:RequiredFieldValidator
			ControlToValidate="Password"
			ErrorMessage=" *" foreColor="red"
			Display="Dynamic"
			runat="server"
			/></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="再次输入新密码" Text="再次输入新密码：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox TextMode="Password" id="ConfirmPassword" MaxLength="50" Size="20" runat="server"/>
        <asp:RequiredFieldValidator
    			ControlToValidate="ConfirmPassword"
    			ErrorMessage=" *" foreColor="red"
    			Display="Dynamic"
    			runat="server"
    			/>
        <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" Display="Dynamic" ErrorMessage=" 两次输入的密码不一致！请再输入一遍您上面填写的密码。" foreColor="red"></asp:CompareValidator>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
