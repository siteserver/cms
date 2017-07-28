<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageInputContent" %>
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
      <asp:Literal ID="ltlColumnHeadRows" runat="server"></asp:Literal>
      <td class="center" style="width:80px;">添加时间</td>
      <td class="center" style="width:40px;">&nbsp;</td>
      <td class="center" style="width:40px;">&nbsp;</td>
      <asp:Literal ID="ltlHeadRowReply" runat="server"></asp:Literal>
      <td width="20" class="center">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <tr>
          <asp:Literal ID="ColumnItemRows" runat="server"></asp:Literal>
          <td class="center">
            <asp:Literal ID="ItemDateTime" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ItemEidtRow" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ItemViewRow" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ItemRowReply" runat="server"></asp:Literal>
          <td class="center">
            <input type="checkbox" name="ContentIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddButton" Text="添加信息" runat="server" />
    <asp:Button class="btn" id="ImportExcel" runat="server" Text="导入Excel"></asp:Button>
    <asp:Button class="btn" id="ExportExcel" runat="server" Text="导出Excel"></asp:Button>
    <asp:Button class="btn" id="TaxisButton" Text="排 序" runat="server" />
    <asp:Button class="btn" id="Check" OnClick="Check_OnClick" Text="审 核" runat="server" />
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="SelectListButton" Text="选择列表项" runat="server" />
    <asp:Button class="btn" id="SelectFormButton" Text="选择表单项" runat="server" />
    <asp:Button class="btn" id="btnReturn" Text="返 回" runat="server" />
    &nbsp; 
  </ul>

</form>
</body>
</html>
