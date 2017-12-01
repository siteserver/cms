<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageLotteryWinner" %>
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

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <td width="20"></td>
      <td>奖项</td>
      <td>姓名</td>
      <td>手机</td>
      <td>邮箱</td>
      <td>地址</td>
      <td>状态</td>
      <td>中奖时间</td>
      <td>兑奖码</td>
      <td>兑奖时间</td>
      <td width="20"><input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);" /></td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <td class="center">
            <asp:Literal ID="LtlItemIndex" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlAward" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal ID="LtlRealName" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlMobile" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlEmail" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlAddress" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlStatus" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlAddDate" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlCashSN" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlCashDate" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="SpContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
    <asp:Button class="btn" id="BtnSetting" Text="设置状态" runat="server" />
    <asp:Button class="btn" id="BtnExport" Text="导出CSV" runat="server" />
    <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
  </ul>

</form>
</body>
</html>