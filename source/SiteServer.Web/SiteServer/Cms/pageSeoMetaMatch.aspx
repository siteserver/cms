<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageSeoMetaMatch" %>
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
    <h3 class="popover-title">页面元数据匹配</h3>
    <div class="popover-content">
  
      <table class="table noborder">
        <tr>
          <td class="center">栏目列表：</td>
          <td class="center" width="80">&nbsp;</td>
          <td class="center">栏目页元数据：</td>
          <td class="center" width="80">&nbsp;</td>
          <td class="center">内容页元数据：</td>
        </tr>
        <tr>
          <td>
            <asp:ListBox ID="NodeIDCollectionToMatch" SelectionMode="Multiple" Rows="25" style="width:auto" runat="server"></asp:ListBox>
          </td>
          <td class="center">
            <asp:Button class="btn" id="MatchChannelSeoMetaButton" text="<- 匹配" onclick="MatchChannelSeoMetaButton_OnClick" runat="server" />
            <br>
            <br>
            <asp:Button class="btn" id="RemoveChannelSeoMetaButton" text="-> 取消" onclick="RemoveChannelSeoMetaButton_OnClick" runat="server" />
          </td>
          <td>
            <asp:ListBox ID="ChannelSeoMetaID" SelectionMode="Single" Rows="25" runat="server"></asp:ListBox>
          </td>
          <td class="center">
            <asp:Button class="btn" id="MatchContentSeoMetaButton" text="<- 匹配" onclick="MatchContentSeoMetaButton_OnClick" runat="server" />
            <br>
            <br>
            <asp:Button class="btn" id="RemoveContentSeoMetaButton" text="-> 取消" onclick="RemoveContentSeoMetaButton_OnClick" runat="server" />
          </td>
          <td>
            <asp:ListBox ID="ContentSeoMetaID" SelectionMode="Single" Rows="25" runat="server"></asp:ListBox>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
