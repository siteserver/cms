<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.PageAdministratorAdd" %>
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

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server" /></h3>
    <div class="popover-content">

      <table class="table noborder table-hover">
        <tr>
          <td width="120">所属部门：</td>
          <td><asp:DropDownList ID="ddlDepartmentID" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>账号：</td>
          <td>
            <asp:TextBox ID="tbUserName" MaxLength="50" Size="35" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="tbUserName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbUserName" ValidationExpression="[^']+" ErrorMessage="不能用单引号" foreColor="red" Display="Dynamic" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbUserName" ValidationExpression="[^<]+" ErrorMessage="不能用&lt;注册" foreColor="red" Display="Dynamic" />
            <span>（帐号用于登录系统，由字母、数字组成）</span>
          </td>
        </tr>
        <tr>
          <td>姓名：</td>
          <td>
            <asp:TextBox ID="tbDisplayName" MaxLength="50" Size="35" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="tbDisplayName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
          </td>
        </tr>
        <asp:PlaceHolder ID="phPassword" runat="server">
        <tr>
          <td>密码：</td>
          <td>
            <asp:TextBox TextMode="Password" ID="tbPassword" MaxLength="50" Size="35" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="tbPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
          </td>
        </tr>
        <tr>
          <td>确认密码：</td>
          <td>
            <asp:TextBox TextMode="Password" ID="tbConfirmPassword" MaxLength="50" Size="35" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="tbConfirmPassword" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
            <asp:CompareValidator ID="tbNewPasswordCompare" runat="server" ControlToCompare="tbPassword" ControlToValidate="tbConfirmPassword" Display="Dynamic" foreColor="red" ErrorMessage=" 两次输入的密码不一致！请再输入一遍您上面填写的密码。"></asp:CompareValidator>
          </td>
        </tr>
        </asp:PlaceHolder>
        <tr>
          <td>所在区域：</td>
          <td><asp:DropDownList ID="ddlAreaID" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>电子邮箱：</td>
          <td>
            <asp:TextBox ID="tbEmail" runat="server" Size="35"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="tbEmail" ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z-]*\.)+\w{2,})" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
          </td>
        </tr>
        <tr>
          <td>手机号码：</td>
          <td>
            <asp:TextBox ID="tbMobile" runat="server" Size="35"></asp:TextBox>
            <asp:RegularExpressionValidator ControlToValidate="tbMobile" ValidationExpression="^0?\d{11}$" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
          </td>
        </tr>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" OnClick="Submit_OnClick" Text="确 定" runat="server" />
            <asp:Button class="btn" ID="btnReturn" Text="返 回" runat="server" />
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
