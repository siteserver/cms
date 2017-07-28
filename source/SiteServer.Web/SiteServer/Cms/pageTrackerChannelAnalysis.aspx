<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerChannelAnalysis" %>
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
          开始时间：
          <bairong:DateTimeTextBox id="StartDate" class="input-small" Columns="30" runat="server" />
          &nbsp;&nbsp;
          结束时间：
          <bairong:DateTimeTextBox id="EndDate" class="input-small" Columns="30" runat="server" />
          &nbsp;&nbsp;
          <asp:Button class="btn" id="Analysis" OnClick="Analysis_OnClick" Text="分 析" runat="server" />
        </td>
      </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>栏目名</td>
      <td width="400">比例</td>
      <td width="80">栏目访问量</td>
      <td width="80">内容访问量</td>
      <td width="70">总访问量</td>
      <td width="50">&nbsp;</td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlTrHtml" runat="server" />
          <td>
            <asp:Literal id="ltlNodeTitle" runat="server" />
          </td>
          <td>
            <asp:Literal id="ltlAccessNumBar" runat="server"></asp:Literal>
          </td>
          <td class="center"><strong>
            <asp:Literal id="ltlChannelCount" runat="server" />
            </strong></td>
          <td class="center"><strong>
            <asp:Literal id="ltlContentCount" runat="server" />
            </strong></td>
          <td class="center"><strong>
            <asp:Literal id="ltlTotalCount" runat="server" />
            </strong></td>
          <td class="center"><asp:Literal ID="ltlItemView" runat="server"></asp:Literal></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <br>

</form>
</body>
</html>
