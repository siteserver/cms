<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTracker" %>
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

  <table class="table table-bordered table-striped">
    <tr>
      <td WIDTH="220">开始统计时间：</td>
      <td><asp:Label ID="StartDateTime" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="原有访问量(PageView)" Text="原有访问量(PageView)：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="TrackerPageView" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="原有访客数(UniqueVisitor)" Text="原有访客数(UniqueVisitors)：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="TrackerUniqueVisitor" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="当前访问网站的人数" Text="当前在线人数：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="CurrentVisitorNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="从开始统计到当前所经过的天数" Text="总统计天数：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="TrackingDayNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="网站统计的所有页面访问量之和，同一访客数的每次访问均被记录" Text="总访问量：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="TotalAccessNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="相同用户在过期时间以内访问网站的所有次数，在过期时间内相同IP地址只被计算1次" Text="总访客数：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="TotalUniqueAccessNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="网站平均每天的访问量，同一访客数的每次访问均被记录" Text="平均日访问量：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="AverageDayAccessNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="网站平均每天的访客数，在过期时间内相同IP地址只被计算1次" Text="平均日访客数：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="AverageDayUniqueAccessNum" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="访问量最大的日期" Text="访问量最大的日期：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="MaxAccessDay" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="由每天的数据来计算出网站一天内的最大访问量，同一访客数的每次访问均被记录" Text="最大访问量（日）：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="MaxAccessNumOfDay" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="由每天的数据来计算出网站一个月内的最大访问量，同一访客数的每次访问均被记录" Text="最大访问量（月）：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="MaxAccessNumOfMonth" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="由每天的数据来计算出网站一天内的最大访客数，在过期时间内相同IP地址只被计算1次" Text="最大访客数（日）：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="MaxUniqueAccessNumOfDay" runat="server"></asp:Label></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="由每天的数据来计算出网站一个月内的最大访客数，在过期时间内相同IP地址只被计算1次" Text="最大访客数（月）：" runat="server" ></bairong:help></td>
      <td><asp:Label ID="MaxUniqueAccessNumOfMonth" runat="server"></asp:Label></td>
    </tr>
  </table>

</form>
</body>
</html>
