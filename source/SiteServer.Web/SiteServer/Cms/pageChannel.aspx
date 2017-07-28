<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannel" enableViewState = "false" %>
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

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>栏目名</td>
      <td width="300">所属栏目组</td>
      <td width="100">栏目索引</td>
      <td width="30">上升</td>
      <td width="30">下降</td>
      <td width="50">&nbsp;</td>
      <td width="20"></td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
          <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:PlaceHolder id="PlaceHolder_AddChannel" runat="server">
      <asp:Button class="btn btn-success" id="AddChannel1" Text="快速添加" runat="server" />
      <asp:Button class="btn" id="AddChannel2" Text="添加栏目" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PlaceHolder_ChannelEdit" runat="server">
      <asp:Button class="btn" id="AddToGroup" Text="设置栏目组" runat="server" />
      <asp:Button class="btn" id="SelectEditColumns" Text="编辑项" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PlaceHolder_Translate" runat="server">
      <asp:Button class="btn" id="Translate" Text="转 移" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PlaceHolder_Import" runat="server">
      <asp:Button class="btn" id="Import" Text="导 入" runat="server" />
    </asp:PlaceHolder>
    <asp:Button class="btn" id="Export" Text="导 出" runat="server" />
    <asp:PlaceHolder id="PlaceHolder_Delete" runat="server">
      <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PlaceHolder_Create" runat="server">
      <asp:Button class="btn" id="Create" Text="生 成" runat="server" />
    </asp:PlaceHolder>
  </ul>

</form>
</body>
</html>
