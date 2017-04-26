<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Platform.PageUserProfile" %>

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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <ul class="nav nav-pills">
    <li class="active"><a href="pageUserProfile.aspx">修改资料</a></li>
    <li><a href="pageUserPassword.aspx">更改密码</a></li>
  </ul>

  <div class="popover popover-static">
  <h3 class="popover-title">修改资料</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="120">账号：</td>
        <td><asp:Literal ID="UserName" runat="server"></asp:Literal></td>
      </tr>
      <tr>
        <td>姓名：</td>
        <td><asp:TextBox ID="DisplayName" runat="server" Width="180px"></asp:TextBox>
          <asp:RequiredFieldValidator runat="server" ControlToValidate="DisplayName"
            ErrorMessage="姓名为必填项。" foreColor="red" ToolTip="姓名为必填项。"  Display="Dynamic"></asp:RequiredFieldValidator></td>
      </tr>
      <tr>
        <td>电子邮件：</td>
        <td><asp:TextBox ID="Email" runat="server" Width="180px"></asp:TextBox>
          <asp:RegularExpressionValidator ControlToValidate="Email"
            ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z-]*\.)+\w{2,})"
            ErrorMessage="邮件格式不正确。" foreColor="red" Display="Dynamic" runat="server"/></td>
      </tr>
      <tr>
        <td>手机号码：</td>
        <td>
          <asp:TextBox ID="Mobile" runat="server" Width="180px"></asp:TextBox>
          <asp:RegularExpressionValidator ControlToValidate="Mobile" ValidationExpression="^0?\d{11}$" ErrorMessage="手机号码格式不正确" foreColor="red" Display="Dynamic" runat="server" />
        </td>
      </tr>
    </table>

    <hr />
    <table class="table noborder">
      <tr>
        <td class="center">
          <asp:Button class="btn btn-primary" OnClick="Submit_Click" runat="server" Text="修 改"  />
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
