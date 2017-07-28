<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisContentHits" %>
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
        <bairong:Alerts runat="server" />
        <asp:Literal ID="ltlBreadCrumb" runat="server" />

        <div style="width: 100%">
            <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
            <div id="hits" style="height: 400px; width: 100%; display: inline-block"></div>
            <!-- ECharts单文件引入 -->
            <script src="../assets/echarts/echarts.js"></script>
            <script type="text/javascript">
                // 路径配置
                require.config({
                    paths: {
                        echarts: '../assets/echarts'
                    }
                });
                // 新增信息数目
                require(
                    [
                        'echarts',
                        'echarts/chart/bar' // 使用柱状图就加载bar模块，按需加载
                    ],
                    function (ec) {
                        // 基于准备好的dom，初始化echarts图表
                        var newChart = ec.init(document.getElementById('hits'));
                        //x array
                        var xArrayHits = [];
                        //y array
                        var yArrayHits = [];
                        var yArrayHitsDay = [];
                        var yArrayHitsWeek = [];
                        var yArrayHitsMonth = [];
                        //title
                        var hitsTitle = "点击量";
                        var hitsTitleDay = "日点击量";
                        var hitsTitleWeek = "周点击量";
                        var hitsTitleMonth = "月点击量";

                        <asp:Literal id="LtlArray" runat="server"></asp:Literal>

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
                            xAxis: [
                                {
                                    type: 'category',
                                    data: []
                                }
                            ],
                            yAxis: [
                                {
                                    type: 'value'
                                }
                            ],
                            series: [
                                {
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
                        //前10条
                        option.xAxis[0].data = xArrayHits;
                        //点击量
                        option.series[0].data = yArrayHits;
                        //日点击量
                        option.series[1].data = yArrayHitsDay;
                        //周点击量
                        option.series[2].data = yArrayHitsWeek;
                        //月点击量
                        option.series[3].data = yArrayHitsMonth;
                        newChart.setOption(option);
                    }
                    );
            </script>
        </div>

        <table class="table table-bordered table-hover">
            <tr class="info thead">
                <td>内容标题(点击查看)</td>
                <td>所属栏目</td>
                <td width="60" class="center">点击量</td>
                <td width="60" class="center">日点击量</td>
                <td width="60" class="center">周点击量</td>
                <td width="60" class="center">月点击量</td>
                <td width="120" class="center">最后点击时间</td>
            </tr>
            <asp:Repeater ID="RptContents" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlChannel" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlHits" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlHitsByDay" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlHitsByWeek" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlHitsByMonth" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlLastHitsDate" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <bairong:SqlPager ID="SpContents" runat="server" class="table table-pager" />

    </form>
</body>
</html>
