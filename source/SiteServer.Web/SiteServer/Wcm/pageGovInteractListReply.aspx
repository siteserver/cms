<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractListReply" %>
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

  <script type="text/javascript">
  $(document).ready(function()
  {
    loopRows(document.getElementById('contents'), function(cur){ cur.onclick = chkSelect; });
    $(".popover-hover").popover({trigger:'hover',html:true});
  });
  </script>

  <div class="well well-small">
    <div style="float:left">
        <asp:PlaceHolder ID="phSwitchToTranslate" runat="server">
        <asp:HyperLink ID="hlSwitchTo" NavigateUrl="javascript:;" runat="server" Text="转 办"></asp:HyperLink>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="phComment" runat="server">
        &nbsp;<span class="gray">|</span>&nbsp;
        <asp:HyperLink ID="hlComment" NavigateUrl="javascript:;" runat="server" Text="批 示"></asp:HyperLink>
      </asp:PlaceHolder>
      <asp:PlaceHolder ID="phDelete" runat="server">
        &nbsp;<span class="gray">|</span>&nbsp;
        <asp:HyperLink ID="hlDelete" NavigateUrl="javascript:;" runat="server" Text="删 除"></asp:HyperLink>
      </asp:PlaceHolder>
    </div>
    <div style="float:right">
      办件数：<asp:Literal id="ltlTotalCount" runat="server" />
    </div>
  </div>

  <table id="contents" class="table table-bordered table-hover">
    <tr class="info thead">
      <td width="40">编号</td>
      <td>办件标题(点击进入操作界面) </td>
      <td width="100">提交日期</td>
      <td width="200">意见</td>
      <td width="120">办理部门</td>
      <td width="60">期限</td>
      <td width="60">状态</td>
      <td width="60">流动轨迹</td>
      <td width="60">快速查看</td>
      <td width="60">回复办件</td>
      <td width="50"></td>
      <td width="20" class="center">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal ID="ltlTr" runat="server"></asp:Literal>
          <td class="center"><asp:Literal ID="ltlID" runat="server"></asp:Literal></td>
          <td>&nbsp;<asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
          <td><asp:Literal ID="ltlRemark" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlDepartment" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlLimit" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlState" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlFlowUrl" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlViewUrl" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlReplyUrl" runat="server"></asp:Literal></td>
          <td class="center"><asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal></td>
          <td class="center"><input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' /></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

</form>
</body>
</html>
