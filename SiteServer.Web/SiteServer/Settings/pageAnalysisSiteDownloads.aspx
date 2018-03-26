<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAnalysisSiteDownloads" %>
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

        <div class="card-box">
          <ul class="nav nav-pills">
            <li class="nav-item">
              <a class="nav-link" href="pageAnalysisSite.aspx">站点数据统计</a>
            </li>
            <li class="nav-item">
              <a class="nav-link" href="pageAnalysisSiteHits.aspx">内容点击统计</a>
            </li>
            <li class="nav-item active">
              <a class="nav-link" href="pageAnalysisSiteDownloads.aspx">文件下载统计</a>
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
            <div id="hits" style="height: 400px; width: 90%; display: inline-block"></div>
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
                  var xArrayDownload = [];
                  var yArrayDownload = [];

                  var newTitle = "文件下载量";

                  <%=StrArray%>

                  if (xArrayDownload.length == 0) {
                    xArrayDownload = ["暂无数据"];
                    yArrayDownload = [0];
                  }

                  var option = {
                    tooltip: {
                      show: true
                    },
                    legend: {
                      data: ['下载量']
                    },
                    xAxis: [{
                      type: 'category',
                      data: xArrayDownload
                    }],
                    yAxis: [{
                      type: 'value'
                    }],
                    series: [{
                      "name": "值",
                      "type": "bar",
                      "data": yArrayDownload
                    }]
                  };

                  option.xAxis[0].data = xArrayDownload;
                  option.series[0].data = yArrayDownload;
                  option.series[0].name = "文件下载量";
                  option.legend.data = [newTitle];
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
                    <th>站点名称</th>
                    <th class="text-center">文件下载量</th>
                  </thead>
                  <tbody>
                    <asp:Repeater runat="server" ID="RptContents">
                      <ItemTemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlSiteName" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlDownloadNum" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                      <td>总计 </td>
                      <td class="text-center">
                        <asp:Literal id="LtlTotalNum" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->