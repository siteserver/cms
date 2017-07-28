<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisSiteDownload" EnableViewState="false" %>
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

        <div class="popover popover-static">
            <h3 class="popover-title">站点文件下载统计</h3>
            <div class="popover-content">

                <div style="width: 100%">
                    <!-- 为ECharts准备一个具备大小（宽高）的Dom -->
                    <div id="hits" style="height: 400px; width: 90%; display: inline-block"></div>
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
                                var xArrayDownload = [];

                                //y array
                                var yArrayDownload = [];

                                //title
                                var newTitle = "文件下载量";

                                <asp:Literal id="LtlArray" runat="server"></asp:Literal>

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
                                    xAxis: [
                                        {
                                            type: 'category',
                                            data: xArrayDownload
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
                                            "data": yArrayDownload
                                        }
                                    ]
                                };
                                // 文件下载量
                                option.xAxis[0].data = xArrayDownload;
                                option.series[0].data = yArrayDownload;
                                option.series[0].name = "文件下载量";
                                option.legend.data = [newTitle];
                                newChart.setOption(option);
                            }
                    );
                    </script>
                </div>


                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>站点名称</td>
                        <td>文件下载量</td>
                    </tr>

                    <asp:Repeater runat="server" ID="RpContents">
                        <ItemTemplate>
                            <tr>
                                <td style="text-align: left">
                                    <asp:Literal ID="ltlPublishmentSystemName" runat="server"></asp:Literal></td>
                                <td style="text-align: center">
                                    <asp:Literal ID="ltlDownloadNum" runat="server"></asp:Literal></td>
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
                        <td>文件下载量</td>
                    </tr>
                    <tr>
                        <td class="center" style="width: 250px;">总计 </td>
                        <td class="center" style="width: 100px;">
                            <asp:Literal id="LtlTotalNum" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
