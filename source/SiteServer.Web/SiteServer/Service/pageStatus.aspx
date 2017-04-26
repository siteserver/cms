<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Service.PageStatus" %>
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
  <bairong:alerts runat="server"></bairong:alerts>

  <style type="text/css">
  .normal {
    background: url('../pic/icon/normal.png') no-repeat 0 0;margin-right: 8px;display: inline-block;width: 18px;height: 18px;vertical-align: middle;
  }
  .issue{
    background: url('../pic/icon/issue.png') no-repeat 0 0;margin-right: 8px;display: inline-block;width: 18px;height: 18px;vertical-align: middle;
  }
  .text{
    vertical-align: middle;margin-right: 0px;width: 180px;height: 20px;color:#333333 !important;
  }
  </style>

  <div class="well-small">
    <asp:Literal id="LtlMessage" runat="server" />
    <table style="float:right; margin-top:5px;">
      <tr>
      <td width="60"><span class="normal"></span><span class="text">正常</span></td>
      <td width="60"><span class="issue"></span><span class="text">问题</span></td>
      </tr>
    </table>
    <div style="clear:both"></div>
  </div>

  <div class="popover popover-static">
  <h3 class="popover-title">siteserver.exe 服务组件</h3>
  <div class="popover-content">

    <table class="table noborder">
      <tr>
        <td>
          <table class="table">
            <tr>
              <asp:Literal id="LtlCreateWatch" runat="server" />
              <asp:Literal id="LtlTaskCreate" runat="server" />
              <asp:Literal id="LtlTaskGather" runat="server" />
              <asp:Literal id="LtlTaskBackup" runat="server" />
            </tr>
          </table>
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
