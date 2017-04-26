<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTrackerIpView" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <asp:DataList ID="MyDataList" ShowHeader="true" cellspacing="0" cellpadding="0" class="center" border="0" Width="100%" runat="server">
    <headertemplate>
      <table class="table">
        <tr class="info">
          <td width="120">&nbsp;访问来源</td>
          <td>比例</td>
          <td width="60">&nbsp;访问数</td>
        </tr>
      </table>
    </headertemplate>
    <itemtemplate>
      <table class="table">
        <tr>
          <td width="120">&nbsp;
            <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
          </td>
          <td>
            <asp:Literal id="ltlAccessNumBar" runat="server"></asp:Literal>
          </td>
          <td class="center" width="60">
            <asp:Literal ID="ltlItemCount" runat="server"></asp:Literal>
          </td>
        </tr>
      </table>
    </itemtemplate>
  </asp:DataList>
  <table width="100%" class="center">      
    <tr>
      <td class="center">
        <ul class="breadcrumb">
          <asp:Button Cssclass="btn" id="ExportAndDelete" OnClick="ExportAndDelete_Click" Text="导出Excel并删除" Visible="false" runat="server" />
          &nbsp;&nbsp;
          <asp:Button Cssclass="btn" id="Export" OnClick="Export_Click" Text="导出Excel" runat="server" />
        </ul>
      </td>
    </tr>
  </table>

</form>
</body>
</html>
