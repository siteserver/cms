<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentModelAdd" Trace="false"%>
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
      <td width="0"><bairong:help HelpText="内容模型标识" Text="模型标识：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="tbModelID" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbModelID" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbModelID"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="0"><bairong:help HelpText="内容模型名称" Text="模型名称：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="tbModelName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbModelName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbModelName"
						ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="0"><bairong:help HelpText="图标" Text="图标：" runat="server" ></bairong:help></td>
      <td width="0"><asp:TextBox Columns="25" MaxLength="255" id="tbIconUrl" runat="server" /><br>
      <span class="gray">（图标必须位于assets\icons\tree文件夹下,可以为空）</span></td>
    </tr>
    <tr>
      <td width="0"><bairong:help HelpText="辅助表类型" Text="辅助表类型：" runat="server" ></bairong:help></td>
      <td width="0"><asp:RadioButtonList ID="rblTableType" RepeatDirection="Horizontal" class="noborder" OnSelectedIndexChanged="rblTableType_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:RadioButtonList></td>
    </tr>
    <tr>
      <td width="0"><bairong:help HelpText="辅助表名称" Text="辅助表名称：" runat="server" ></bairong:help></td>
      <td width="0"><asp:DropDownList ID="ddlTableName" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td width="0"><bairong:help HelpText="模型简介。" Text="模型简介：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Width="350" Rows="4" MaxLength="200" TextMode="MultiLine" id="tbDescription" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
