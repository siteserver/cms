<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserConfigRegister" %>
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
  <h3 class="popover-title">用户注册设置</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="200">允许新用户注册：</td>
        <td>
          <asp:RadioButtonList ID="RblIsRegisterAllowed" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
          <span>选择否将禁止新用户注册, 但不影响过去已注册的会员的使用</span>
        </td>
      </tr>
      <tr>
        <td>注册密码最小长度：</td>
        <td>
          <asp:TextBox ID="TbRegisterPasswordMinLength" class="input-mini" runat="server"></asp:TextBox>
          <span>0代表不限制</span>
        </td>
      </tr>
      <tr>
        <td>注册密码规则限制：</td>
        <td>
          <asp:DropDownList ID="DdlRegisterPasswordRestriction" runat="server"></asp:DropDownList>
        </td>
      </tr>
      <tr>
        <td>新用户注册验证：</td>
        <td>
          <asp:DropDownList ID="DdlRegisterVerifyType" runat="server"></asp:DropDownList>
          <br>
          <span>选择<code>短信验证</code>将向用户发送短信验证码以确认手机号码，此选项需要开启短信发送功能</span>
        </td>
      </tr>
      <tr>
        <td>同一IP注册间隔限制：</td>
        <td>
          <asp:TextBox class="input-mini" MaxLength="10" id="TbRegisterMinMinutesOfIpAddress" runat="server" /> 分钟
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbRegisterMinMinutesOfIpAddress" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          <br>
          <span>同一IP在本时间间隔内将只能注册一个帐号，0 为不限制</span>
        </td>
      </tr>
    </table>

    <hr />
    <table class="table noborder">
      <tr>
        <td class="center">
          <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
