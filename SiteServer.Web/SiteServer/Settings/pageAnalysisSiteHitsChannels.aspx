<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAnalysisSiteHitsChannels" %>
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
        <li class="nav-item">
          <a class="nav-link" href="pageAnalysisUser.aspx">会员数据统计</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-inline">
        <div class="form-group">
          <label class="col-form-label m-r-10">站点</label>
          <asp:DropDownList ID="DdlSiteId" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="Analysis_OnClick"
            runat="server" />
        </div>
      </div>

      <hr />

      <div style="width: 100%">
        <div id="hits" style="height: 400px; width: 100%; display: inline-block"></div>
        <script type="text/javascript">
          require.config({
            paths: {
              echarts: '../assets/echarts'
            }
          });
          require(
            [
              'echarts',
              'echarts/chart/bar'
            ],
            function (ec) {
              var newChart = ec.init(document.getElementById('hits'));
              var xArrayHits = [];
              var yArrayHits = [];
              var yArrayHitsDay = [];
              var yArrayHitsWeek = [];
              var yArrayHitsMonth = [];
              var hitsTitle = "点击量";
              var hitsTitleDay = "日点击量";
              var hitsTitleWeek = "周点击量";
              var hitsTitleMonth = "月点击量";

              <%=StrArray%>

              if (xArrayHits.length == 0) {
                xArrayHits = ["暂无数据"];
                yArrayHits = [0];
                yArrayHitsDay = [0];
                yArrayHitsWeek = [0];
                yArrayHitsMonth = [0];
              }

              var option = {
                tooltip: {
                  show: true
                },
                legend: {
                  data: []
                },
                xAxis: [{
                  type: 'category',
                  data: []
                }],
                yAxis: [{
                  type: 'value'
                }],
                series: [{
                    "name": hitsTitle,
                    "type": "bar",
                    "data": []
                  },
                  {
                    "name": hitsTitleDay,
                    "type": "bar",
                    "data": []
                  },
                  {
                    "name": hitsTitleWeek,
                    "type": "bar",
                    "data": []
                  },
                  {
                    "name": hitsTitleMonth,
                    "type": "bar",
                    "data": []
                  }
                ]
              };
              option.xAxis[0].data = xArrayHits;
              option.series[0].data = yArrayHits;
              option.series[1].data = yArrayHitsDay;
              option.series[2].data = yArrayHitsWeek;
              option.series[3].data = yArrayHitsMonth;
              newChart.setOption(option);
            }
          );
        </script>
      </div>

      <div class="panel panel-default">
        <div class="panel-body p-0">
          <div class="table-responsive">
            <table class="table tablesaw table-hover m-0">
              <thead>
                <th>内容标题(点击查看)</th>
                <th>所属栏目</th>
                <th class="text-center">点击量</th>
                <th class="text-center">日点击量</th>
                <th class="text-center">周点击量</th>
                <th class="text-center">月点击量</th>
                <th class="text-center">最后点击时间</th>
              </thead>
              <tbody>
                <asp:Repeater ID="RptContents" runat="server">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
                      </td>
                      <td>
                        <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlHits" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlHitsByDay" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlHitsByWeek" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlHitsByMonth" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlLastHitsDate" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <ctrl:SqlPager ID="SpContents" runat="server" class="table table-pager" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->