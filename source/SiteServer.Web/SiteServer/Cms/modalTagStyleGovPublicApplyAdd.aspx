<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTagStyleGovPublicApplyAdd" Trace="false"%>
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
      <td width="160"><bairong:help HelpText="样式名称" Text="样式名称：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="StyleName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="StyleName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="StyleName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置提交内容后是否需要发送短信给管理员" Text="是否发送短信提醒：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="IsSMS" AutoPostBack="true" OnSelectedIndexChanged="IsSMS_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="发送短信" Value="True"></asp:ListItem>
          <asp:ListItem Text="不发送短信" Value="False" Selected="true"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <asp:PlaceHolder ID="phSMS" Visible="false" runat="server">
      <tr>
        <td><bairong:help HelpText="发送到管理员手机" Text="发送到管理员手机：" runat="server" ></bairong:help></td>
        <td><asp:TextBox Columns="35" MaxLength="50" id="SMSTo" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="SMSTo" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="SMSTo" ValidationExpression="^1[358]\d{9}$" Display="Dynamic" runat="server"><br>
            * 手机号码格式不正确!</asp:RegularExpressionValidator></td>
      </tr>
      <tr>
        <td><bairong:help HelpText="短信标题" Text="短信标题：" runat="server" ></bairong:help></td>
        <td><asp:TextBox Columns="35" MaxLength="50" id="SMSTitle" Text="提醒：" runat="server" /></td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
