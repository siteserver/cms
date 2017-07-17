<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Settings.ModalImportZip" %>
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
        <td width="120">导入方式：</td>
        <td>
          <asp:RadioButtonList ID="RblImportType" AutoPostBack="true" OnSelectedIndexChanged="RblImportType_SelectedIndexChanged" runat="server"></asp:RadioButtonList>
        </td>
      </tr>
      <asp:PlaceHolder ID="PhUpload" runat="server">
        <tr>
          <td width="120">选择压缩包：</td>
          <td><input type="file" id="HifFile" size="35" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="HifFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
          </td>
        </tr>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="PhDownload" runat="server">
        <tr>
          <td width="120">压缩包下载地址：</td>
          <td>
            <asp:TextBox Width="260" id="TbDownloadUrl" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbDownloadUrl" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
            />
          </td>
        </tr>
      </asp:PlaceHolder>
    </table>

  </form>
</body>

</html>