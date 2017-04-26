<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalInputContentImport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td><bairong:help HelpText="选择需要上传的栏目文件" Text="栏目文件：" runat="server"></bairong:help></td>
      <td><input type=file  id=myFile size="35" runat="server"/>
        <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置从第几条开始导入" Text="从第几条开始导入：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="5" id="ImportStart" runat="server"/>
        (默认为第一条) </td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置共导入几条" Text="共导入几条：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="5" id="ImportCount" runat="server"/>
        (默认为全部导入) </td>
    </tr>
    <tr>
      <td><bairong:help HelpText="设置内容的审核状态" Text='审核状态：'  runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList ID="IsChecked" RepeatDirection="Horizontal" class="noborder" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
