<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContent" %>
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

  <script type="text/javascript">
  $(document).ready(function()
  {
    loopRows(document.getElementById('contents'), function(cur){ cur.onclick = chkSelect; });
    $(".popover-hover").popover({trigger:'hover',html:true});
  });
  </script>

  <div class="well well-small">
    <asp:Literal ID="LtlButtons" runat="server"></asp:Literal>
    <div id="contentSearch" style="display:none;margin-top:10px;">
      时间从：
      <bairong:DateTimeTextBox ID="TbDateFrom" class="input-small" Columns="12" runat="server" />
      目标：
      <asp:DropDownList ID="DdlSearchType" class="input-medium" runat="server"> </asp:DropDownList>
      关键字：
      <asp:TextBox class="input-medium" ID="TbKeyword" runat="server" />
      <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
    </div>
  </div>

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <td>内容标题(点击查看) </td>
      <asp:Literal ID="LtlHeadRows" runat="server"></asp:Literal>
      <asp:Literal ID="LtlHeadCommand" runat="server"></asp:Literal>
      <td width="50">状态</td>
      <td width="20">
        <input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);">
      </td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <td>
            <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
          </td>
          <asp:Literal ID="ltlRows" runat="server"></asp:Literal>
          <td class="center">
            <asp:Literal ID="ltlCommands" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="ltlStatus" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <input type="checkbox" name="ContentIDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

</form>
</body>
</html>
