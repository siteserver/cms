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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
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
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
          <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:PlaceHolder id="PhAddChannel" runat="server">
      <asp:Button class="btn btn-success" id="BtnAddChannel1" Text="快速添加" runat="server" />
      <asp:Button class="btn" id="BtnAddChannel2" Text="添加栏目" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PhChannelEdit" runat="server">
      <asp:Button class="btn" id="BtnAddToGroup" Text="设置栏目组" runat="server" />
      <asp:Button class="btn" id="BtnSelectEditColumns" Text="编辑项" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PhTranslate" runat="server">
      <asp:Button class="btn" id="BtnTranslate" Text="转 移" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PhImport" runat="server">
      <asp:Button class="btn" id="BtnImport" Text="导 入" runat="server" />
    </asp:PlaceHolder>
    <asp:Button class="btn" id="BtnExport" Text="导 出" runat="server" />
    <asp:PlaceHolder id="PhDelete" runat="server">
      <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
    </asp:PlaceHolder>
    <asp:PlaceHolder id="PhCreate" runat="server">
      <asp:Button class="btn" id="BtnCreate" Text="生 成" runat="server" />
    </asp:PlaceHolder>
  </ul>

</form>
</body>
</html>
