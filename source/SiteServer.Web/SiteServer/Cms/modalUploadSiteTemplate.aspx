<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadSiteTemplate" %>
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
      <td width="120"><bairong:help HelpText="选择导入站点模板的方式" Text="导入方式：" runat="server" ></bairong:help></td>
      <td>
      	<asp:RadioButtonList ID="rblImportType" AutoPostBack="true" OnSelectedIndexChanged="rblImportType_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
      </td>
    </tr>
    <asp:PlaceHolder ID="phUpload" runat="server">
    <tr>
      <td width="120"><bairong:help HelpText="选择需要上传的压缩包" Text="选择压缩包：" runat="server" ></bairong:help></td>
      <td><input type="file" id="myFile" size="35" runat="server"/>
        <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phDownload" runat="server">
    <tr>
      <td width="120"><bairong:help HelpText="输入下载地址" Text="压缩包下载地址：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Width="260" id="tbDownloadUrl" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbDownloadUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
