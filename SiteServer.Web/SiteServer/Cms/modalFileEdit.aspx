<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalFileEdit" Trace="false"%>
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
      <td width="80" class="center"><bairong:help HelpText="文件名" Text="文件名：" runat="server" ></bairong:help></td>
      <td>
        <asp:TextBox Columns="60" MaxLength="50" id="FileName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="FileName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
        &nbsp;
        <asp:Literal ID="ltlOpen" runat="server"></asp:Literal>
        <asp:Literal ID="ltlView" runat="server"></asp:Literal>
      </td>
    </tr>
    <tr>
      <td width="80" class="center"><bairong:help HelpText="文件编码" Text="文件编码：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList id="Charset" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td width="80" class="center"><bairong:help HelpText="文件编辑方式" Text="编辑方式：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="IsPureText" AutoPostBack="true" OnSelectedIndexChanged="IsPureText_OnSelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="纯文本编辑" Value="True" Selected="true"></asp:ListItem>
          <asp:ListItem Text="使用编辑器" Value="False"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td class="center" colspan="2"><asp:PlaceHolder ID="PlaceHolder_PureText" runat="server">
          <asp:TextBox ID="FileContentTextBox" runat="server" TextMode="MultiLine" Width="98%" Height="300"></asp:TextBox>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="PlaceHolder_TextEditor" Visible="false" runat="server">
          <bairong:UEditor id="FileContent" width="540" height="300" runat="server"></bairong:UEditor>
        </asp:PlaceHolder></td>
    </tr>
    <tr>
      <td class="center" colspan="2">
        <asp:Button class="btn" text="保 存" onclick="Save_OnClick" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
