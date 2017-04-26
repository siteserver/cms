<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PagePublishmentSystemDelete" %>
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
    <h3 class="popover-title">删除站点</h3>
    <div class="popover-content">

      <asp:PlaceHolder id="phIsRetainFiles" runat="server">

      <table class="table noborder table-hover">
        <tr>
          <td width="120">是否保留文件：</td>
          <td>
            <asp:RadioButtonList ID="RetainFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="保留文件" Value="true"></asp:ListItem>
              <asp:ListItem Text="删除文件" Value="false" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
            <span>选择保留文件删除操作将仅在数据库中删除此站点。</span>
          </td>
        </tr>
      </table>

      <hr />

      </asp:PlaceHolder>

      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-danger" id="Submit" text="删 除" OnClick="Submit_OnClick" runat="server"/>
            <asp:PlaceHolder id="phReturn" runat="server">
              <input type="button" class="btn" value="返 回" onClick="javascript:location.href='<%=GetReturnUrl()%>';" />
            </asp:PlaceHolder>
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
