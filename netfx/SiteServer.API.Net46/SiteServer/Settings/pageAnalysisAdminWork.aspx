<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageAnalysisAdminWork" %>
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
        <li class="nav-item active">
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

      <asp:PlaceHolder id="PhAnalysis" runat="server">

        <hr />

        <div style="width: 100%">
          <div id="user" style="height: 400px; width: 90%; display: inline-block"></div>
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
                var newChart = ec.init(document.getElementById('user'));
                var xArrayNew = [];
                var yArrayNew = [];
                var yArrayUpdate = [];
                var newTitle = "新增信息数目";
                var updateTitle = "更新信息数目";

                <%=StrArray%>

                if (xArrayNew.length == 0) {
                  xArrayNew = ["暂无数据"];
                  yArrayNew = [0];
                }

                var option = {
                  tooltip: {
                    show: true
                  },
                  legend: {
                    data: [newTitle, updateTitle]
                  },
                  toolbox: {
                    show: true,
                    feature: {
                      dataView: {
                        show: true,
                        readOnly: false
                      },
                      restore: {
                        show: true
                      },
                      saveAsImage: {
                        show: true
                      }
                    }
                  },
                  xAxis: [{
                    type: 'category',
                    data: []
                  }],
                  yAxis: [{
                    type: 'value'
                  }],
                  series: [{
                      "name": newTitle,
                      "type": "bar",
                      "data": []
                    },
                    {
                      "name": updateTitle,
                      "type": "bar",
                      "data": []
                    }
                  ]
                };
                option.xAxis[0].data = xArrayNew;
                option.series[0].data = yArrayNew;
                option.series[1].data = yArrayUpdate;
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
                  <th>登录名 </th>
                  <th>显示名 </th>
                  <th class="text-center">新增内容 </th>
                  <th class="text-center">更新内容 </th>
                </thead>
                <tbody>
                  <asp:Repeater ID="RptContents" runat="server">
                    <ItemTemplate>
                      <tr>
                        <td>
                          <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                        </td>
                        <td>
                          <asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center">
                          <asp:Literal ID="ltlContentAdd" runat="server"></asp:Literal>
                        </td>
                        <td class="text-center">
                          <asp:Literal ID="ltlContentUpdate" runat="server"></asp:Literal>
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

      </asp:PlaceHolder>

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->