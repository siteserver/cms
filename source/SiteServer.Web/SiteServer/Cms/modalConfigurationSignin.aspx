<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalConfigurationSignin" Trace="false"%>
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
      <td width="140"><bairong:help HelpText="内容签收的对象：" Text="内容签收对象：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="TypeList" RepeatDirection="Horizontal" class="noborder" OnSelectedIndexChanged="TypeList_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:RadioButtonList></td>
    </tr>
    <asp:PlaceHolder ID="phGroup" runat="server">
      <tr>
        <td><bairong:help HelpText="选择签收内容的用户组" Text="签收内容的用户组：" runat="server" ></bairong:help></td>
        <td><asp:CheckBoxList ID="GroupIDList" runat="server" RepeatColumns="3" RepeatDirection="Horizontal" class="noborder"></asp:CheckBoxList></td>
      </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phUser" Visible="false" runat="server">
      <tr>
        <td><bairong:help HelpText="设置签收内容的用户名列表" Text="签收用户名列表：" runat="server" ></bairong:help></td>
        <td><asp:TextBox Columns="30" Rows="4" TextMode="MultiLine" id="UserNameList" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="UserNameList" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <br>
        <span class="gray">多个用户名以“,”分割</span>
        </td>
      </tr>
    </asp:PlaceHolder>
    <tr>
      <td><bairong:help HelpText="签收优先级" Text="签收优先级：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList id="Priority" runat="server" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="截止日期" Text="截止日期：" runat="server" ></bairong:help></td>
      <td><bairong:DateTimeTextBox id="EndDate" showTime="true" Columns="20" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
