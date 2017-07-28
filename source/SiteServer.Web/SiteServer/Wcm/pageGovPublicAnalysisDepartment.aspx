<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicAnalysisDepartment" %>
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
  <bairong:alerts text="不设置开始时间将统计自建站以来所有的统计数据。" runat="server" />

  <div class="well well-small">
    <table class="table table-noborder">
      <tr>
        <td>
          开始时间：
          <bairong:DateTimeTextBox id="StartDate" class="input-small" Columns="30" runat="server" />
          结束时间：
          <bairong:DateTimeTextBox id="EndDate" class="input-small" Columns="30" runat="server" />
          <asp:Button class="btn" id="Analysis" OnClick="Analysis_OnClick" Text="统 计" runat="server" />
        </td>
      </tr>
    </table>
  </div>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>统计对象</td>
      <td width="70">处理文档数</td>
      <td width="405">对比图</td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlTrHtml" runat="server"></asp:Literal>
          <td>
            <asp:Literal id="ltlTarget" runat="server"></asp:Literal>
          </td>
          <td class="center" style="font-weight:bold">
            <asp:Literal id="ltlCount" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal id="ltlBar" runat="server"></asp:Literal>
          </td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <br>

</form>
</body>
</html>
