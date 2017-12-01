<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageCache" %>
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
    <bairong:Alerts runat="server" />
    <div class="popover popover-static">
      <h3 class="popover-title">清空缓存日志</h3>
      <div class="popover-content">
        <table class="table noborder table-hover">
          <tr>
            <td style="width: 200px;">当前缓存数量：</td>
            <td><asp:Literal id="LtlCount" runat="server" /> 个</td>
          </tr>
          <tr>
            <td style="width: 200px;">当前已使用缓存百分比：</td>
            <td><asp:Literal id="LtlPercentage" runat="server" /></td>
          </tr>
        </table>

        <hr />

        <table class="table table-bordered table-hover">
          <tr class="info thead">
            <td>缓存键</td>
            <td>缓存值</td>
          </tr>
          <asp:Repeater id="RptContents" runat="server">
            <itemtemplate>
              <tr>
                <td style="word-break: break-all;">
                  <span><asp:Literal id="ltlKey" runat="server"></asp:Literal></span>
                </td>
                <td style="word-break: break-all;">
                  <span><asp:Literal id="ltlValue" runat="server"></asp:Literal></span>
                </td>
              </tr>
            </itemtemplate>
          </asp:Repeater>
        </table>
      </div>
    </div>

    <table class="table noborder">
      <tr>
        <td class="center">
          <asp:Button class="btn btn-primary" id="Submit" Text="清除缓存" OnClick="Submit_OnClick" runat="server" />
        </td>
      </tr>
    </table>

  </form>
</body>

</html>