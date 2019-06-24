<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAnalysisSite" %>
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
        <li class="nav-item active">
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

        <div class="form-group m-l-10">
          <label class="col-form-label m-r-10">时间从</label>
          <ctrl:DateTimeTextBox ID="TbStartDate" class="form-control" runat="server" />
        </div>

        <div class="form-group m-l-10">
          <label class="col-form-label m-r-10">到</label>
          <ctrl:DateTimeTextBox ID="TbEndDate" class="form-control" runat="server" />
        </div>

        <asp:Button class="btn btn-success m-l-10" OnClick="Analysis_OnClick" Text="统 计" runat="server" />
      </div>

      <hr />

      <div style="width: 100%">
        <div id="new" style="height: 400px; width: 45%; display: inline-block"></div>
        <div id="update" style="height: 400px; width: 45%; display: inline-block"></div>
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
              var newChart = ec.init(document.getElementById('new'));
              var updateChart = ec.init(document.getElementById('update'));
              var xArrayNew = [];
              var xArrayUpdate = [];
              var yArrayNew = [];
              var yArrayUpdate = [];
              var newTitle = "新增内容";
              var updateTitle = "修改内容";

              <%=StrArray%>

              if (xArrayUpdate.length == 0) {
                xArrayUpdate = ["暂无数据"];
                yArrayUpdate = [0];
              }
              if (xArrayNew.length == 0) {
                xArrayNew = ["暂无数据"];
                yArrayNew = [0];
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
                  "name": "值",
                  "type": "bar",
                  "data": []
                }]
              };
              option.xAxis[0].data = xArrayNew;
              option.series[0].data = yArrayNew;
              option.series[0].name = "增加量";
              option.legend.data = [newTitle];
              newChart.setOption(option);
              option.xAxis[0].data = xArrayUpdate;
              option.series[0].data = yArrayUpdate;
              option.series[0].name = "更新量";
              option.legend.data = [updateTitle];
              updateChart.setOption(option);
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
                <th class="text-center">新增内容</th>
                <th class="text-center">修改内容</th>
                <th class="text-center">合计</th>
              </thead>
              <tbody>
                <asp:Repeater runat="server" ID="RptContents">
                  <ItemTemplate>
                    <tr>
                      <td>
                        <asp:Literal ID="ltlSiteName" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlNewContentNum" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlUpdateContentNum" runat="server"></asp:Literal>
                      </td>
                      <td class="text-center">
                        <asp:Literal ID="ltlTotalNum" runat="server"></asp:Literal>
                      </td>
                    </tr>
                  </ItemTemplate>
                </asp:Repeater>
                <tr>
                  <td>总计 </td>
                  <td class="text-center">
                    <asp:Literal id="LtlVerticalNew" runat="server"></asp:Literal>
                  </td>
                  <td class="text-center">
                    <asp:Literal id="LtlVerticalUpdate" runat="server"></asp:Literal>
                  </td>
                  <td class="text-center">
                    <asp:Literal id="LtlVerticalTotalNum" runat="server"></asp:Literal>
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