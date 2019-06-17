﻿<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAnalysisUser" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
  <script src="../assets/echarts/echarts.js"></script>
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item">
          <a class="nav-link" href="pageAnalysisSite.aspx">站点数据统计</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAnalysisAdminLogin.aspx">管理员登录统计</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageAnalysisAdminWork.aspx">管理员工作统计</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="pageAnalysisUser.aspx">会员数据统计</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-inline">
        <div class="form-group">
          <label class="col-form-label m-r-10">时间从</label>
          <ctrl:DateTimeTextBox ID="TbDateFrom" class="form-control" runat="server" />
        </div>

        <div class="form-group m-l-10">
          <label class="col-form-label m-r-10">到</label>
          <ctrl:DateTimeTextBox ID="TbDateTo" class="form-control" runat="server" />
        </div>

        <div class="form-group m-l-10">
          <label class="col-form-label m-r-10">X轴</label>
          <asp:DropDownList ID="DdlXType" class="form-control" runat="server"></asp:DropDownList>
        </div>

        <asp:Button class="btn btn-success m-l-10" OnClick="Search_OnClick" Text="统 计" runat="server" />
      </div>

      <hr />

      <p class="lead">
        <asp:Literal id="LtlPageTitle" runat="server"></asp:Literal>
      </p>

      <div id="main" style="height: 400px"></div>
      <script type="text/javascript">
        require.config({
          paths: {
            echarts: '../assets/echarts'
          }
        });
        require(
          [
            'echarts',
            'echarts/chart/line'
          ],
          function (ec) {
            var myChart = ec.init(document.getElementById('main'));
            var xArray = [];
            var yArray = [];

            <%=StrArray%>

            var option = {
              tooltip: {
                show: true
              },
              legend: {
                data: ['用户']
              },
              xAxis: [{
                type: 'category',
                data: xArray
              }],
              yAxis: [{
                type: 'value'
              }],
              series: [{
                "name": "增加量",
                "type": "line",
                "data": yArray
              }]
            };

            myChart.setOption(option);
          }
        );
      </script>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->