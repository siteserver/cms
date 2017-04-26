<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovPublicCategoryClassAdd" Trace="false"%>
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

  <table class="table noborder table-hover">
    <tr>
      <td width="120">分类法名称：</td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="tbClassName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbClassName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbClassName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td>分类代码：</td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="tbClassCode" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbClassCode" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbClassCode"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td>是否启用分类：</td>
      <td><asp:RadioButtonList ID="rblIsEnabled" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="启用" Value="True" Selected="true"></asp:ListItem>
          <asp:ListItem Text="不启用" Value="False"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td>说明：</td>
      <td><asp:TextBox Columns="30" style="height:50px" TextMode="MultiLine" MaxLength="50" id="tbDescription" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
