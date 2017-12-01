<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageInputContent" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
<script type="text/javascript">
$(document).ready(function()
{
  loopRows(document.getElementById('contents'), function(cur){ cur.onclick = chkSelect; });
});
</script>
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <asp:Literal ID="LtlColumnHeadRows" runat="server"></asp:Literal>
      <td class="center" style="width:80px;">添加时间</td>
      <td class="center" style="width:40px;">&nbsp;</td>
      <td class="center" style="width:40px;">&nbsp;</td>
      <asp:Literal ID="LtlHeadRowReply" runat="server"></asp:Literal>
      <td width="20" class="center">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <asp:Literal ID="ltlColumnRows" runat="server"></asp:Literal>
          <td class="center">
            <asp:Literal ID="ltlDateTime" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlEidt" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlView" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ltlReplyRows" runat="server"></asp:Literal>
          <td class="center">
            <input type="checkbox" name="ContentIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnAdd" Text="添加信息" runat="server" />
    <asp:Button class="btn" id="BtnImportExcel" runat="server" Text="导入Excel"></asp:Button>
    <asp:Button class="btn" id="BtnExportExcel" runat="server" Text="导出Excel"></asp:Button>
    <asp:Button class="btn" id="BtnTaxis" Text="排 序" runat="server" />
    <asp:Button class="btn" id="BtnCheck" OnClick="BtnCheck_OnClick" Text="审 核" runat="server" />
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="BtnSelectList" Text="选择列表项" runat="server" />
    <asp:Button class="btn" id="BtnSelectForm" Text="选择表单项" runat="server" />
    <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
    &nbsp; 
  </ul>

</form>
</body>
</html>
