<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageMessageContent" %>
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
      <td>称呼</td>
      <td>留言内容</td>
      <td>类型</td>
      <td>微信码</td>
      <td>IP地址</td>
      <td>留言时间</td>
      <td width="20"><input type="checkbox" onClick="selectRows(document.getElementById('contents'), this.checked);" /></td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <tr>
          <td class="center">
            <asp:Literal ID="LtlItemIndex" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlDisplayName" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal ID="LtlContent" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlIsReply" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlWXOpenID" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlIPAddress" runat="server"></asp:Literal>
          </td>
          <td class="center">
            <asp:Literal ID="LtlAddDate" runat="server"></asp:Literal>
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
    <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
  </ul>

</form>
</body>
</html>