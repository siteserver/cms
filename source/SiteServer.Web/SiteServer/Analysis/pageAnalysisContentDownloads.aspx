<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Analysis.PageAnalysisContentDownloads" %>
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
            <div id="Download" style="height: 400px; width: 100%; display: inline-block"></div>
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
                        var newChart = ec.init(document.getElementById('Download'));
                        //x array
                        var xArrayDownload = [];
                        //y array
                        var yArrayDownload = [];
                        //title
                        var DownloadTitle = "文件下载量";

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
                                    "name": DownloadTitle,
                                    "type": "bar",
                                    "data": []
                                }
                            ]
                        };
                        //前10条
                        option.xAxis[0].data = xArrayDownload;
                        //点击量
                        option.series[0].data = yArrayDownload;

                        newChart.setOption(option);
                    }
                    );
            </script>
        </div>

        <table class="table table-bordered table-hover">
            <tr class="info thead">
                <td>内容标题(点击查看)</td>
                <td>所属栏目</td>
                <td>附件地址</td>
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
                        <td>
                            <asp:Literal ID="ltlFileUrl" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <bairong:SqlPager ID="SpContents" runat="server" class="table table-pager" />

    </form>
</body>
</html>
