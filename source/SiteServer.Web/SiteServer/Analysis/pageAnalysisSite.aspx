<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisSite" EnableViewState="false" %>
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
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>开始时间：
                        <bairong:DateTimeTextBox ID="StartDate" class="input-small" runat="server" />
                        结束时间：
                        <bairong:DateTimeTextBox ID="EndDate" class="input-small" runat="server" />
                        <asp:Button class="btn" ID="Analysis" OnClick="Analysis_OnClick" Text="分 析" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">站点数据统计</h3>
            <div class="popover-content">

                <div style="width: 100%">
                    <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
                    <div id="new" style="height: 400px; width: 30%; display: inline-block"></div>
                    <div id="update" style="height: 400px; width: 30%; display: inline-block"></div>
                    <div id="remark" style="height: 400px; width: 30%; display: inline-block"></div>
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
                                var newChart = ec.init(document.getElementById('new'));
                                var updateChart = ec.init(document.getElementById('update'));
                                var remarkChart = ec.init(document.getElementById('remark'));
                                //x array
                                var xArrayNew = [];
                                var xArrayUpdate = [];
                                var xArrayRemark = [];
                                //y array
                                var yArrayNew = [];
                                var yArrayUpdate = [];
                                var yArrayRemark = [];
                                //title
                                var newTitle = "新增信息数目";
                                var updateTitle = "更新信息数目";
                                var remarkTitle = "新增评论数目";

                                <asp:Literal id="LtlArray" runat="server"></asp:Literal>

                                if (xArrayRemark.length == 0) {
                                    xArrayRemark = ["暂无数据"];
                                    yArrayRemark = [0];
                                }
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
                                            "name": "值",
                                            "type": "bar",
                                            "data": []
                                        }
                                    ]
                                };
                                // 新增
                                option.xAxis[0].data = xArrayNew;
                                option.series[0].data = yArrayNew;
                                option.series[0].name = "增加量";
                                option.legend.data = [newTitle];
                                newChart.setOption(option);
                                //更新
                                option.xAxis[0].data = xArrayUpdate;
                                option.series[0].data = yArrayUpdate;
                                option.series[0].name = "更新量";
                                option.legend.data = [updateTitle];
                                updateChart.setOption(option);
                                //评论
                                option.xAxis[0].data = xArrayRemark;
                                option.series[0].data = yArrayRemark;
                                option.series[0].name = "增加量";
                                option.legend.data = [remarkTitle];
                                remarkChart.setOption(option);
                            }
                    );
                    </script>
                </div>


                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>站点名称</td>
                        <td>新增信息数目</td>
                        <td>更新信息数目</td>
                        <td>新增评论数目</td>
                        <td>合计</td>
                    </tr>

                    <asp:Repeater runat="server" ID="RpContents">
                        <ItemTemplate>
                            <tr>
                                <td style="text-align: left">
                                    <asp:Literal ID="ltlPublishmentSystemName" runat="server"></asp:Literal>
                                </td>
                                <td style="text-align: center">
                                    <asp:Literal ID="ltlNewContentNum" runat="server"></asp:Literal>
                                </td>
                                <td style="text-align: center">
                                    <asp:Literal ID="ltlUpdateContentNum" runat="server"></asp:Literal>
                                </td>
                                <td style="text-align: center">
                                    <asp:Literal ID="ltlNewRemarkNum" runat="server"></asp:Literal>
                                </td>
                                <td style="text-align: center">
                                    <asp:Literal ID="ltlTotalNum" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>

            </div>
        </div>

        <div class="popover popover-static">
            <h3 class="popover-title">总计</h3>
            <div class="popover-content">

                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>所有站点</td>
                        <td>新增信息数目</td>
                        <td>更新信息数目</td>
                        <td>新增评论数目</td>
                        <td>合计</td>
                    </tr>
                    <tr>
                        <td class="center" style="width: 250px;">总计 </td>
                        <td class="center" style="width: 100px;">
                            <asp:Literal id="LtlVerticalNew" runat="server"></asp:Literal>
                        </td>
                        <td class="center" style="width: 100px;">
                            <asp:Literal id="LtlVerticalUpdate" runat="server"></asp:Literal>
                        </td>
                        <td class="center" style="width: 100px;">
                            <asp:Literal id="LtlVerticalRemrk" runat="server"></asp:Literal>
                        </td>
                        <td class="center" style="width: 100px;">
                            <asp:Literal id="LtlVerticalTotalNum" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
