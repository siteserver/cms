<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerContentAnalysis" %>
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
  <bairong:alerts text="不设置开始时间将统计自建站以来所有的流量数据。" runat="server" />

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          栏目：
          <asp:DropDownList ID="NodeIDDropDownList" AutoPostBack="true" OnSelectedIndexChanged="NodeIDDropDownList_SelectedIndexChanged" runat="server"> </asp:DropDownList>
          开始时间：
          <bairong:DateTimeTextBox id="StartDate" class="input-small" Columns="30" runat="server" />
          结束时间：
          <bairong:DateTimeTextBox id="EndDate" class="input-small" Columns="30" runat="server" />
          <asp:Button class="btn" id="Analysis" OnClick="Analysis_OnClick" Text="分 析" runat="server" />
        </td>
      </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>&nbsp;内容标题(点击查看)</td>
      <td class="center" style="width:45px;">访问量</td>
      <td class="center" style="width:50px;">&nbsp;</td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <tr style="height:25px;">
          <td>&nbsp;
            <asp:Literal ID="ItemTitle" runat="server"></asp:Literal></td>
          <td class="center" style="width:45px;"><strong>
            <asp:Literal ID="ItemCount" runat="server"></asp:Literal>
            </strong></td>
          <td class="center" style="width:50px;"><asp:Literal ID="ItemView" runat="server"></asp:Literal></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

</form>
</body>
</html>
