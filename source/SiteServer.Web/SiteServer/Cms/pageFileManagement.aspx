<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageFileManagement" %>
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

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          <asp:HyperLink ID="UploadLink" NavigateUrl="javascript:;" runat="server">
            <asp:ImageButton  runat="server" ImageUrl="../assets/icons/add.gif" ImageAlign="AbsBottom"></asp:ImageButton>
            上传文件
          </asp:HyperLink>
          <asp:ImageButton ID="DeleteButton" runat="server" ImageUrl="../assets/icons/filesystem/management/delete.gif" CommandName="NavigationBar" CommandArgument="Delete" OnCommand="LinkButton_Command"></asp:ImageButton>
          <code>&nbsp;当前目录：
          <asp:Literal id="ltlCurrentDirectory" runat="server" /></code>
        </td>
        <td class="pull-right"><asp:DropDownList ID="ListType" class="input-medium" runat="server" OnSelectedIndexChanged="ListType_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
      </tr>
    </table>
  </div>

  <table class="table">
    <tr>
      <td>
        <asp:Literal id="ltlFileSystems" runat="server" enableViewState="false" />
      </td>
    </tr>
  </table>

  <br>

</form>
</body>
</html>
